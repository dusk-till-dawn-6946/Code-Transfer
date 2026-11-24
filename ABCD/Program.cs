using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using ATTENDANCE_AUTOMAILER.Sql;
using ClosedXML.Excel;

namespace ATTENDANCE_AUTOMAILER
{
    // =========================================================================================
    //  ATTENDANCE_AUTOMAILER
    //
    //  Command line batch that runs from Windows Task Scheduler after every shift closes:
    //    1. works out which shift / attendance day the run belongs to (current time by default),
    //    2. reads the resolved attendance segments of that attendance day from HRACE,
    //    3. applies the attendance violation rules of the BRD,
    //    4. writes the Excel report and mails it to the stakeholders.
    //
    //  Everything except the SQL text lives in this file on purpose: one file to copy, one file
    //  to change. Sql\SqlQueries.cs holds the queries ported from frmTimekeeper.
    //
    //  Exit codes: 0 = success, 1 = error (Windows Task Scheduler alarms on it).
    // =========================================================================================
    internal class Program
    {
        private const int ExitSuccess = 0;
        private const int ExitFailure = 1;

        private static int Main(string[] args)
        {
            Logger.Initialize();

            try
            {
                Options options = Options.Parse(args);

                if (options.ShowHelp)
                {
                    PrintUsage();
                    return ExitSuccess;
                }

                return Run(options);
            }
            catch (Exception ex)
            {
                Logger.Error("Batch failed.", ex);
                return ExitFailure;
            }
        }

        // ---------------------------------------------------------------------------------
        //  Orchestration
        // ---------------------------------------------------------------------------------

        private static int Run(Options options)
        {
            DateTime now = DateTime.Now;

            // Shift: --shift when given, otherwise the shift that has just ended at "now".
            ShiftInfo shift;
            if (options.AllShifts)
            {
                shift = new ShiftInfo { Code = "ALL", Name = "All shifts", WindowStart = TimeSpan.Zero, WindowEnd = TimeSpan.Zero };
            }
            else
            {
                shift = options.ShiftCode == null ? ShiftResolver.ResolveForNow(now) : ShiftResolver.Find(options.ShiftCode);
                if (shift == null)
                {
                    Logger.Error("Unknown shift code: " + options.ShiftCode + ". Valid values: A, B, C, G, U, ALL.", null);
                    return ExitFailure;
                }
            }

            // Attendance day: --date when given, otherwise the day the shift belongs to.
            DateTime attendanceDate = options.AttendanceDate.HasValue
                ? options.AttendanceDate.Value.Date
                : ShiftResolver.ResolveAttendanceDate(shift, now);

            // Location: --location when given, otherwise App.config.
            string locationCode = string.IsNullOrEmpty(options.LocationCode) ? AppSettings.LocationCode : options.LocationCode;
            if (string.IsNullOrEmpty(locationCode))
            {
                Logger.Error("LocationCode is not configured (App.config) and --location was not passed.", null);
                return ExitFailure;
            }

            Logger.Info("======================================================================");
            Logger.Info(string.Format(
                CultureInfo.InvariantCulture,
                "Attendance automailer started. Location {0}, shift {1}, attendance date {2:dd-MM-yyyy}, run at {3:dd-MM-yyyy HH:mm}",
                locationCode,
                shift.Code,
                attendanceDate,
                now));

            if (shift.Code != "ALL")
            {
                Logger.Info(string.Format(
                    CultureInfo.InvariantCulture,
                    "Shift punch window {0}, shift ends {1:HH:mm}",
                    shift.WindowText,
                    ShiftResolver.ShiftEnd(shift, attendanceDate)));
            }

            AttendanceRepository repository = new AttendanceRepository(locationCode);

            List<AttendanceRow> segments = repository.GetAttendanceSegments(attendanceDate);
            Logger.Info("Resolved attendance segments: " + segments.Count.ToString(CultureInfo.InvariantCulture));

            List<StreakRow> streaks = repository.GetContinuousStreaks(
                attendanceDate,
                Math.Min(AppSettings.ConsecutiveWeeklyOffDays + 1, AppSettings.ConsecutiveHighRiskDays));
            Logger.Info("Workers on a consecutive working streak: " + streaks.Count.ToString(CultureInfo.InvariantCulture));

            List<MissingPunchRow> missingIn = repository.GetMissingInPunches(attendanceDate);
            Logger.Info("Workers with a missing In punch: " + missingIn.Count.ToString(CultureInfo.InvariantCulture));

            segments = ApplyScopeFilters(segments, streaks, missingIn);
            if (AppSettings.VendorCode.Length > 0 || AppSettings.DepartmentCodes.Length > 0)
            {
                streaks = ApplyScopeFilters(streaks);
                missingIn = ApplyScopeFilters(missingIn);
            }

            ViolationResult result = ViolationDetector.Detect(segments, missingIn, streaks, shift, locationCode, attendanceDate);

            Logger.Info(string.Format(
                CultureInfo.InvariantCulture,
                "Violations: {0} total ({1} high concern). {2}",
                result.Violations.Count,
                result.HighRiskCount,
                DescribeCounts(result)));

            if (segments.Count == 0)
            {
                Logger.Warn("No attendance segment was found for this shift / date. Check the shift window and the punch data.");
            }

            string outputFolder = string.IsNullOrEmpty(options.OutputFolder) ? AppSettings.OutputDirectory : options.OutputFolder;
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            string attachmentPath = ExcelReportBuilder.Build(result, outputFolder);
            Logger.Info("Excel report written: " + attachmentPath);

            if (options.NoMail)
            {
                Logger.Info("Mail skipped (--no-mail).");
                return ExitSuccess;
            }

            if (!AppSettings.MailEnabled)
            {
                Logger.Info("Mail skipped (MailEnabled = false in App.config).");
                return ExitSuccess;
            }

            if (!result.HasViolations && !options.AlwaysMail)
            {
                Logger.Info("No violation for this shift - mail not sent (use --always-mail to send it anyway).");
                return ExitSuccess;
            }

            List<string> to = SplitAddresses(options.To ?? AppSettings.MailTo);
            List<string> cc = SplitAddresses(options.Cc ?? AppSettings.MailCc);
            foreach (string address in EmailSender.ResolveDepartmentRecipients(result))
            {
                if (!cc.Contains(address, StringComparer.OrdinalIgnoreCase))
                {
                    cc.Add(address);
                }
            }

            string status = EmailSender.Send(result, attachmentPath, to, cc);
            Logger.Info(status);
            Logger.Info("Attendance automailer finished successfully.");

            return ExitSuccess;
        }

        // Optional VendorCode / DepartmentCodes restriction from App.config.
        private static List<AttendanceRow> ApplyScopeFilters(List<AttendanceRow> rows, List<StreakRow> streaks, List<MissingPunchRow> missing)
        {
            string vendorCode = AppSettings.VendorCode;
            string departmentCodes = AppSettings.DepartmentCodes;

            if (vendorCode.Length == 0 && departmentCodes.Length == 0)
            {
                return rows;
            }

            HashSet<string> allowed = null;
            if (departmentCodes.Length > 0)
            {
                allowed = new HashSet<string>(
                    departmentCodes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.Trim()),
                    StringComparer.OrdinalIgnoreCase);
            }

            if (vendorCode.Length > 0)
            {
                Logger.Info("Vendor filter applied: " + vendorCode);
            }

            if (allowed != null)
            {
                Logger.Info("Department filter applied: " + departmentCodes);
            }

            return rows
                .Where(r => vendorCode.Length == 0 || string.Equals(r.VendorCode, vendorCode, StringComparison.OrdinalIgnoreCase))
                .Where(r => allowed == null || allowed.Contains(r.DepartmentCode))
                .ToList();
        }

        private static List<StreakRow> ApplyScopeFilters(List<StreakRow> rows)
        {
            string vendorCode = AppSettings.VendorCode;
            string departmentCodes = AppSettings.DepartmentCodes;

            HashSet<string> allowed = null;
            if (departmentCodes.Length > 0)
            {
                allowed = new HashSet<string>(
                    departmentCodes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.Trim()),
                    StringComparer.OrdinalIgnoreCase);
            }

            return rows
                .Where(r => vendorCode.Length == 0 || string.Equals(r.VendorCode, vendorCode, StringComparison.OrdinalIgnoreCase))
                .Where(r => allowed == null || allowed.Contains(r.DepartmentCode))
                .ToList();
        }

        private static List<MissingPunchRow> ApplyScopeFilters(List<MissingPunchRow> rows)
        {
            string vendorCode = AppSettings.VendorCode;
            string departmentCodes = AppSettings.DepartmentCodes;

            HashSet<string> allowed = null;
            if (departmentCodes.Length > 0)
            {
                allowed = new HashSet<string>(
                    departmentCodes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.Trim()),
                    StringComparer.OrdinalIgnoreCase);
            }

            return rows
                .Where(r => vendorCode.Length == 0 || string.Equals(r.VendorCode, vendorCode, StringComparison.OrdinalIgnoreCase))
                .Where(r => allowed == null || allowed.Contains(r.DepartmentCode))
                .ToList();
        }

        private static string DescribeCounts(ViolationResult result)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Overstay high {0}, overstay medium {1}, continuous shift {2}, consecutive days {3}, missing In {4}, missing Out {5}",
                result.CountOf(ViolationRow.OverstayHigh),
                result.CountOf(ViolationRow.OverstayMedium),
                result.CountOf(ViolationRow.ContinuousShift),
                result.CountOf(ViolationRow.ConsecutiveDays),
                result.CountOf(ViolationRow.MissingInPunch),
                result.CountOf(ViolationRow.MissingOutPunch));
        }

        private static List<string> SplitAddresses(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new List<string>();
            }

            return value.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .Where(a => a.Length > 0)
                .ToList();
        }

        private static void PrintUsage()
        {
            Console.WriteLine("ATTENDANCE_AUTOMAILER - attendance violation automailer");
            Console.WriteLine();
            Console.WriteLine("Usage: ATTENDANCE_AUTOMAILER.exe [options]");
            Console.WriteLine();
            Console.WriteLine("With no option at all the batch reports the shift that has just ended, for the");
            Console.WriteLine("attendance day of the current time, for the LocationCode in App.config, and mails");
            Console.WriteLine("the report to MailTo. Every one of those can be overridden:");
            Console.WriteLine();
            Console.WriteLine("  --shift <A|B|C|G|U|ALL>  Shift to report. Default: the shift that just ended.");
            Console.WriteLine("  --date <dd-MM-yyyy>      Attendance day. Default: derived from the shift and the run time.");
            Console.WriteLine("  --location <code>        Location / company code. Default: LocationCode in App.config.");
            Console.WriteLine("  --to <addresses>         Override MailTo (comma or semicolon separated).");
            Console.WriteLine("  --cc <addresses>         Extra CC recipients.");
            Console.WriteLine("  --out <folder>           Folder for the Excel report. Default: OutputDirectory.");
            Console.WriteLine("  --no-mail                Build the report only, do not send any mail.");
            Console.WriteLine("  --always-mail            Send the mail even when there is no violation.");
            Console.WriteLine("  --help                   Show this help.");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  ATTENDANCE_AUTOMAILER.exe");
            Console.WriteLine("      current time -> shift, its attendance day, configured location");
            Console.WriteLine("  ATTENDANCE_AUTOMAILER.exe --shift A --date 20-11-2026 --no-mail");
            Console.WriteLine("      report for shift A of 20-11-2026, Excel only");
            Console.WriteLine("  ATTENDANCE_AUTOMAILER.exe --shift B --location 4600 --to boss@tatasteel.com");
            Console.WriteLine();
            Console.WriteLine("Expected daily schedule (Task Scheduler, 15 minutes after each shift end):");
            Console.WriteLine(ShiftResolver.DescribeSchedule());
        }

        internal class Options
        {
            public bool ShowHelp { get; set; }
            public string ShiftCode { get; set; }
            public DateTime? AttendanceDate { get; set; }
            public string LocationCode { get; set; }
            public string To { get; set; }
            public string Cc { get; set; }
            public string OutputFolder { get; set; }
            public bool NoMail { get; set; }
            public bool AlwaysMail { get; set; }

            public bool AllShifts
            {
                get { return ShiftCode != null && ShiftCode.Equals("ALL", StringComparison.OrdinalIgnoreCase); }
            }

            public static Options Parse(string[] args)
            {
                Options options = new Options();

                for (int index = 0; index < args.Length; index++)
                {
                    string argument = args[index] ?? string.Empty;
                    string value = index + 1 < args.Length ? args[index + 1] : null;

                    switch (argument.Trim().ToLowerInvariant())
                    {
                        case "--help":
                        case "-h":
                        case "/?":
                            options.ShowHelp = true;
                            break;
                        case "--shift":
                        case "-s":
                            options.ShiftCode = value;
                            index++;
                            break;
                        case "--date":
                        case "-d":
                            options.AttendanceDate = ParseDate(value);
                            index++;
                            break;
                        case "--location":
                        case "-l":
                            options.LocationCode = value;
                            index++;
                            break;
                        case "--to":
                            options.To = value;
                            index++;
                            break;
                        case "--cc":
                            options.Cc = value;
                            index++;
                            break;
                        case "--out":
                            options.OutputFolder = value;
                            index++;
                            break;
                        case "--no-mail":
                            options.NoMail = true;
                            break;
                        case "--always-mail":
                            options.AlwaysMail = true;
                            break;
                        default:
                            throw new ArgumentException("Unknown argument: " + argument + ". Use --help for usage.");
                    }
                }

                return options;
            }

            private static DateTime? ParseDate(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("--date requires a value (dd-MM-yyyy).");
                }

                string[] formats = { "dd-MM-yyyy", "d-M-yyyy", "yyyy-MM-dd", "dd/MM/yyyy" };
                DateTime parsed;
                if (DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                {
                    return parsed.Date;
                }

                throw new ArgumentException("Invalid --date value: " + value + ". Expected dd-MM-yyyy.");
            }
        }
    }

    // =========================================================================================
    //  Settings (App.config appSettings)
    // =========================================================================================

    internal static class AppSettings
    {
        public static string LocationCode { get { return GetString("LocationCode", string.Empty); } }
        public static string DepartmentCodes { get { return GetString("DepartmentCodes", string.Empty); } }
        public static string VendorCode { get { return GetString("VendorCode", string.Empty); } }

        public static int CutoffHour { get { return GetInt("CutoffHour", 5); } }
        public static int PunchGapHours { get { return GetInt("PunchGapHours", 9); } }

        public static double OverstayHighHours { get { return GetDouble("OverstayHighHours", 12); } }
        public static double OverstayMediumHours { get { return GetDouble("OverstayMediumHours", 9); } }
        public static int ConsecutiveWeeklyOffDays { get { return GetInt("ConsecutiveWeeklyOffDays", 6); } }
        public static int ConsecutiveHighRiskDays { get { return GetInt("ConsecutiveHighRiskDays", 10); } }

        public static string OutputDirectory { get { return GetFolder("OutputDirectory", "Output"); } }
        public static string LogDirectory { get { return GetFolder("LogDirectory", "Logs"); } }

        public static bool MailEnabled { get { return GetBool("MailEnabled", true); } }
        public static string MailFrom { get { return GetString("MailFrom", string.Empty); } }
        public static string MailTo { get { return GetString("MailTo", string.Empty); } }
        public static string MailCc { get { return GetString("MailCc", string.Empty); } }
        public static string DepartmentMailMap { get { return GetString("DepartmentMailMap", string.Empty); } }
        public static string MailSubjectPrefix { get { return GetString("MailSubjectPrefix", "Attendance Violation"); } }
        public static int MaxViolationRowsInMail { get { return GetInt("MaxViolationRowsInMail", 40); } }

        public static string SmtpHost { get { return GetString("SmtpHost", string.Empty); } }
        public static int SmtpPort { get { return GetInt("SmtpPort", 25); } }
        public static bool SmtpEnableSsl { get { return GetBool("SmtpEnableSsl", false); } }
        public static string SmtpUser { get { return GetString("SmtpUser", string.Empty); } }

        /// <summary>
        /// Password comes from the ATT_MAIL_PASSWORD environment variable first, so it does not
        /// have to be stored in App.config on the server.
        /// </summary>
        public static string SmtpPassword
        {
            get
            {
                string fromEnvironment = Environment.GetEnvironmentVariable("ATT_MAIL_PASSWORD");
                if (!string.IsNullOrEmpty(fromEnvironment))
                {
                    return fromEnvironment;
                }

                return GetString("SmtpPassword", string.Empty);
            }
        }

        public static string BaseDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        /// <summary>Resolves a possibly relative folder against the exe folder and creates it.</summary>
        public static string GetFolder(string key, string defaultFolder)
        {
            string configured = GetString(key, defaultFolder);
            string path = Path.IsPathRooted(configured) ? configured : Path.Combine(BaseDirectory, configured);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static string GetString(string key, string fallback)
        {
            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        public static int GetInt(string key, int fallback)
        {
            int parsed;
            string value = GetString(key, string.Empty);
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed))
            {
                return parsed;
            }

            return fallback;
        }

        public static double GetDouble(string key, double fallback)
        {
            double parsed;
            string value = GetString(key, string.Empty);
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
            {
                return parsed;
            }

            return fallback;
        }

        public static bool GetBool(string key, bool fallback)
        {
            bool parsed;
            string value = GetString(key, string.Empty);
            if (bool.TryParse(value, out parsed))
            {
                return parsed;
            }

            return fallback;
        }
    }

    // =========================================================================================
    //  Logging - console + one file per day, never throws
    // =========================================================================================

    internal static class Logger
    {
        private static readonly object SyncRoot = new object();
        private static string _logFilePath;

        public static string LogFilePath { get { return _logFilePath; } }

        public static void Initialize()
        {
            string folder = AppSettings.GetFolder("LogDirectory", "Logs");
            _logFilePath = Path.Combine(
                folder,
                "ATTENDANCE_AUTOMAILER_" + DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".log");
        }

        public static void Info(string message)
        {
            Write("INFO ", message, null);
        }

        public static void Warn(string message)
        {
            Write("WARN ", message, null);
        }

        public static void Error(string message, Exception ex)
        {
            Write("ERROR", message, ex);
        }

        private static void Write(string level, string message, Exception ex)
        {
            string line = string.Format(
                CultureInfo.InvariantCulture,
                "{0} [{1}] {2}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                level,
                message);

            lock (SyncRoot)
            {
                Console.WriteLine(line);

                if (ex != null)
                {
                    Console.WriteLine(ex.ToString());
                }

                if (string.IsNullOrEmpty(_logFilePath))
                {
                    return;
                }

                try
                {
                    StringBuilder builder = new StringBuilder(line);
                    if (ex != null)
                    {
                        builder.AppendLine();
                        builder.Append(ex.ToString());
                    }

                    File.AppendAllText(_logFilePath, builder.ToString() + Environment.NewLine);
                }
                catch (Exception)
                {
                    // Logging must never break the batch run.
                }
            }
        }
    }

    // =========================================================================================
    //  Models
    // =========================================================================================

    internal class ShiftInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }

        /// <summary>First valid "In" punch time of day of the shift.</summary>
        public TimeSpan WindowStart { get; set; }

        /// <summary>Last valid "In" punch time of day of the shift.</summary>
        public TimeSpan WindowEnd { get; set; }

        /// <summary>Time of day at which the duty ends (used for the scheduler hints).</summary>
        public TimeSpan ShiftEnd { get; set; }

        /// <summary>True when the duty ends on the next calendar day.</summary>
        public bool CrossesMidnight { get; set; }

        public string WindowText
        {
            get { return WindowStart.ToString(@"hh\:mm") + " - " + WindowEnd.ToString(@"hh\:mm"); }
        }

        public override string ToString()
        {
            return Code;
        }
    }

    /// <summary>One resolved attendance segment (in -> out) from final_attd.</summary>
    internal class AttendanceRow
    {
        public string LocationCode { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string SafetyPassNo { get; set; }
        public string WorkerName { get; set; }
        public string CardNo { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string NatureOfWork { get; set; }
        public string WorkOrderNo { get; set; }
        public string CaCoName { get; set; }

        public DateTime AttendanceDay { get; set; }
        public int SequenceNo { get; set; }
        public string ShiftCode { get; set; }

        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
        public double DisplayMinutes { get; set; }
        public string DisplayHours { get; set; }
        public string DisplayHHmm { get; set; }

        /// <summary>OPEN when the segment has no "Out" punch yet.</summary>
        public string WorkStatus { get; set; }
        public int SegmentCount { get; set; }
    }

    /// <summary>Worker whose first punch of the day is an "Out" punch, i.e. the In punch is missing.</summary>
    internal class MissingPunchRow
    {
        public string LocationCode { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string SafetyPassNo { get; set; }
        public string WorkerName { get; set; }
        public string CardNo { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string WorkOrderNo { get; set; }
        public DateTime FirstPunchTime { get; set; }
    }

    /// <summary>Consecutive working day streak ending on the attendance date.</summary>
    internal class StreakRow
    {
        public string LocationCode { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string SafetyPassNo { get; set; }
        public string WorkerName { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ConsecutiveDays { get; set; }
    }

    /// <summary>
    /// One attendance violation as it appears in the mail and the Excel attachment. The type codes
    /// are the ones already stored by frmTimekeeper in HRACE.T_CEMP_VIOLATION_ACTION.
    /// </summary>
    internal class ViolationRow
    {
        public const string OverstayHigh = "VIOTYPE1";
        public const string ContinuousShift = "VIOTYPE2";
        public const string ConsecutiveDays = "VIOTYPE3";
        public const string OverstayMedium = "VIOTYPE4";
        public const string MissingInPunch = "VIOTYPE5";
        public const string MissingOutPunch = "VIOTYPE6";

        public string ViolationType { get; set; }
        public string ViolationName { get; set; }
        public string Severity { get; set; }
        public string RequiredAction { get; set; }

        public string LocationCode { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string SafetyPassNo { get; set; }
        public string WorkerName { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string NatureOfWork { get; set; }
        public string WorkOrderNo { get; set; }
        public string CaCoName { get; set; }

        public DateTime AttendanceDate { get; set; }
        public string ShiftCode { get; set; }
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
        public double TotalHours { get; set; }
        public int TotalDays { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remarks { get; set; }

        public string InTimeText { get { return InTime.HasValue ? InTime.Value.ToString("HH:mm") : string.Empty; } }
        public string OutTimeText { get { return OutTime.HasValue ? OutTime.Value.ToString("HH:mm") : string.Empty; } }
    }

    internal static class ViolationSeverity
    {
        public const string High = "High";
        public const string Medium = "Medium";
        public const string Low = "Low";
    }

    /// <summary>Everything the batch produced for one shift of one attendance day.</summary>
    internal class ViolationResult
    {
        public ViolationResult()
        {
            Violations = new List<ViolationRow>();
            PunchCountByShift = new Dictionary<string, int>();
        }

        public string LocationCode { get; set; }
        public DateTime AttendanceDate { get; set; }
        public ShiftInfo Shift { get; set; }
        public DateTime GeneratedOn { get; set; }

        public int SegmentsRead { get; set; }
        public int WorkersInShift { get; set; }
        public int TotalWorkers { get; set; }

        public Dictionary<string, int> PunchCountByShift { get; private set; }
        public List<ViolationRow> Violations { get; private set; }

        public bool HasViolations { get { return Violations.Count > 0; } }

        public int HighRiskCount
        {
            get { return Violations.Count(v => v.Severity == ViolationSeverity.High); }
        }

        public int CountOf(string violationType)
        {
            return Violations.Count(v => v.ViolationType == violationType);
        }

        public IEnumerable<ViolationRow> OfType(string violationType)
        {
            return Violations.Where(v => v.ViolationType == violationType);
        }
    }

    // =========================================================================================
    //  Shift definitions - punch windows identical to the shift_category CASE of frmTimekeeper
    // =========================================================================================

    internal static class ShiftResolver
    {
        private static readonly List<ShiftInfo> ShiftList = new List<ShiftInfo>
        {
            new ShiftInfo { Code = "A", Name = "Shift A", WindowStart = new TimeSpan(5, 0, 0), WindowEnd = new TimeSpan(7, 0, 0), ShiftEnd = new TimeSpan(14, 0, 0) },
            new ShiftInfo { Code = "B", Name = "Shift B", WindowStart = new TimeSpan(13, 0, 0), WindowEnd = new TimeSpan(15, 0, 0), ShiftEnd = new TimeSpan(22, 0, 0) },
            new ShiftInfo { Code = "C", Name = "Shift C", WindowStart = new TimeSpan(21, 0, 0), WindowEnd = new TimeSpan(23, 0, 0), ShiftEnd = new TimeSpan(6, 0, 0), CrossesMidnight = true },
            new ShiftInfo { Code = "G", Name = "General Shift (G)", WindowStart = new TimeSpan(7, 1, 0), WindowEnd = new TimeSpan(10, 0, 0), ShiftEnd = new TimeSpan(17, 0, 0) },
            new ShiftInfo { Code = "U", Name = "Unassigned", WindowStart = TimeSpan.Zero, WindowEnd = TimeSpan.Zero, ShiftEnd = TimeSpan.Zero }
        };

        public static IEnumerable<ShiftInfo> All
        {
            get { return ShiftList; }
        }

        public static ShiftInfo Find(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return null;
            }

            return ShiftList.FirstOrDefault(s => string.Equals(s.Code, code.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Shift whose duty has just ended at <paramref name="now"/>. Used when the scheduler does
        /// not pass --shift, so a plain run reports the current time's shift.
        /// </summary>
        public static ShiftInfo ResolveForNow(DateTime now)
        {
            int hour = now.Hour;

            if (hour >= 6 && hour < 14)
            {
                return Find("C");   // C ends 06:00
            }

            if (hour >= 14 && hour < 17)
            {
                return Find("A");   // A ends 14:00
            }

            if (hour >= 17 && hour < 22)
            {
                return Find("G");   // G ends 17:00
            }

            return Find("B");       // B ends 22:00
        }

        /// <summary>
        /// Attendance day (punch date - cutoff hour) the shift belongs to: shift C started the
        /// previous evening, everything else is the current day.
        /// </summary>
        public static DateTime ResolveAttendanceDate(ShiftInfo shift, DateTime now)
        {
            if (shift != null && shift.Code == "C" && now.Hour < 12)
            {
                return now.Date.AddDays(-1);
            }

            if (now.Hour < AppSettings.CutoffHour)
            {
                return now.Date.AddDays(-1);
            }

            return now.Date;
        }

        public static DateTime ShiftStart(ShiftInfo shift, DateTime attendanceDate)
        {
            return attendanceDate.Date + shift.WindowStart;
        }

        public static DateTime ShiftEnd(ShiftInfo shift, DateTime attendanceDate)
        {
            DateTime end = attendanceDate.Date + shift.ShiftEnd;
            return shift.CrossesMidnight ? end.AddDays(1) : end;
        }

        /// <summary>Shift code whose punch window contains the given time of day.</summary>
        public static string ResolveByPunchWindow(TimeSpan timeOfDay)
        {
            int minutes = (int)timeOfDay.TotalMinutes;

            if (minutes >= 300 && minutes <= 420)
            {
                return "A";
            }

            if (minutes >= 421 && minutes <= 600)
            {
                return "G";
            }

            if (minutes >= 780 && minutes <= 900)
            {
                return "B";
            }

            if (minutes >= 1260 && minutes <= 1380)
            {
                return "C";
            }

            return "U";
        }

        /// <summary>
        /// Best guess of the shift of a punch-only worker (an Out punch without an In punch produces
        /// no attendance segment): assume an 8 hour duty before the punch.
        /// </summary>
        public static string InferShiftFromOutPunch(DateTime punchTime)
        {
            TimeSpan assumedStart = punchTime.TimeOfDay.Subtract(TimeSpan.FromHours(8));
            if (assumedStart < TimeSpan.Zero)
            {
                assumedStart = assumedStart.Add(TimeSpan.FromHours(24));
            }

            return ResolveByPunchWindow(assumedStart);
        }

        /// <summary>The four daily runs that cover every shift.</summary>
        public static string DescribeSchedule()
        {
            List<string> lines = new List<string>();
            foreach (ShiftInfo shift in ShiftList.Where(s => s.Code != "U"))
            {
                DateTime runAt = shift.CrossesMidnight
                    ? DateTime.Today.AddDays(1).Add(shift.ShiftEnd).AddMinutes(15)
                    : DateTime.Today.Add(shift.ShiftEnd).AddMinutes(15);

                lines.Add(string.Format(
                    CultureInfo.InvariantCulture,
                    "  --shift {0}  (punch window {1}, shift ends {2:hh\\:mm}, run at {3:HH:mm})",
                    shift.Code,
                    shift.WindowText,
                    shift.ShiftEnd,
                    runAt));
            }

            return string.Join(Environment.NewLine, lines);
        }
    }

    // =========================================================================================
    //  Data access
    // =========================================================================================

    /// <summary>Runs the queries of SqlQueries and maps the result sets to the models.</summary>
    internal class AttendanceRepository
    {
        private readonly string _locationCode;

        public AttendanceRepository(string locationCode)
        {
            _locationCode = locationCode;
        }

        public List<AttendanceRow> GetAttendanceSegments(DateTime attendanceDate)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("p_startdate", ToOracleDate(attendanceDate));
            parameters.Add("p_locationcode", _locationCode);
            parameters.Add("p_gap_hours", AppSettings.PunchGapHours);
            parameters.Add("p_cutoff_hour", AppSettings.CutoffHour);

            DataTable table = Execute("attendance details", SqlQueries.AttendanceDetails, parameters);
            List<AttendanceRow> rows = new List<AttendanceRow>();

            foreach (DataRow row in table.Rows)
            {
                AttendanceRow item = new AttendanceRow();
                item.LocationCode = Text(row, "location_code");
                item.DepartmentCode = Text(row, "dept_code");
                item.DepartmentName = Text(row, "department_name");
                item.SafetyPassNo = Text(row, "safety_pass_no");
                item.CardNo = Text(row, "card_no");
                item.WorkerName = Text(row, "worker_name");
                item.VendorCode = Text(row, "vendor_code");
                item.VendorName = Text(row, "vendor_name");
                item.NatureOfWork = Text(row, "nature_of_work");
                item.WorkOrderNo = Text(row, "wo_no");
                item.CaCoName = Text(row, "ca_co_name");
                item.AttendanceDay = DateValue(row, "attendance_day", attendanceDate);
                item.SequenceNo = (int)NumberValue(row, "seq_no", 0);
                item.ShiftCode = Text(row, "shift_code");
                item.InTime = NullableDate(row, "in_time");
                item.OutTime = NullableDate(row, "out_time");
                item.DisplayMinutes = NumberValue(row, "display_minutes", 0);
                item.DisplayHours = Text(row, "display_hours");
                item.DisplayHHmm = Text(row, "total_hours");
                item.WorkStatus = Text(row, "work_status");
                item.SegmentCount = (int)NumberValue(row, "seg_count", 0);
                rows.Add(item);
            }

            return rows;
        }

        public List<MissingPunchRow> GetMissingInPunches(DateTime attendanceDate)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("p_startdate", ToOracleDate(attendanceDate));
            parameters.Add("p_locationcode", _locationCode);
            parameters.Add("p_cutoff_hour", AppSettings.CutoffHour);

            DataTable table = Execute("missing In punch", SqlQueries.MissingInPunches, parameters);
            List<MissingPunchRow> rows = new List<MissingPunchRow>();

            foreach (DataRow row in table.Rows)
            {
                MissingPunchRow item = new MissingPunchRow();
                item.LocationCode = Text(row, "location_code");
                item.DepartmentCode = Text(row, "dept_code");
                item.DepartmentName = Text(row, "department_name");
                item.SafetyPassNo = Text(row, "safety_pass_no");
                item.CardNo = Text(row, "card_no");
                item.WorkerName = Text(row, "worker_name");
                item.VendorCode = Text(row, "vendor_code");
                item.VendorName = Text(row, "vendor_name");
                item.WorkOrderNo = Text(row, "wo_no");
                item.FirstPunchTime = DateValue(row, "first_punch_time", attendanceDate);
                rows.Add(item);
            }

            return rows;
        }

        public List<StreakRow> GetContinuousStreaks(DateTime attendanceDate, int minDays)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("p_enddate", ToOracleDate(attendanceDate));
            parameters.Add("p_locationcode", _locationCode);
            parameters.Add("p_min_days", minDays);

            DataTable table = Execute("consecutive days", SqlQueries.ContinuousStreaks, parameters);
            List<StreakRow> rows = new List<StreakRow>();

            foreach (DataRow row in table.Rows)
            {
                StreakRow item = new StreakRow();
                item.LocationCode = Text(row, "location_code");
                item.DepartmentCode = Text(row, "dept_code");
                item.DepartmentName = Text(row, "department_name");
                item.SafetyPassNo = Text(row, "safety_pass_no");
                item.WorkerName = Text(row, "worker_name");
                item.VendorCode = Text(row, "vendor_code");
                item.VendorName = Text(row, "vendor_name");
                item.StartDate = DateValue(row, "start_date", attendanceDate);
                item.EndDate = DateValue(row, "end_date", attendanceDate);
                item.ConsecutiveDays = (int)NumberValue(row, "consecutive_days", 0);
                rows.Add(item);
            }

            return rows;
        }

        private static string ToOracleDate(DateTime value)
        {
            return value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Runs one statement and returns its rows. Oracle errors are re-thrown with the query name
        /// so the log says which statement failed on the server.
        /// </summary>
        private static DataTable Execute(string queryName, string sql, Dictionary<string, object> parameters)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OraConnGatepass"].ConnectionString;

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                using (OracleCommand command = new OracleCommand(sql, connection))
                {
                    command.CommandTimeout = 900;

                    // System.Data.OracleClient binds by parameter name, so a bind may be repeated
                    // any number of times in the statement with a single parameter.
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        OracleParameter oracleParameter = new OracleParameter();
                        oracleParameter.ParameterName = parameter.Key;
                        oracleParameter.Value = parameter.Value;
                        command.Parameters.Add(oracleParameter);
                    }

                    connection.Open();

                    DataTable table = new DataTable();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }

                    return table;
                }
            }
            catch (OracleException ex)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Oracle error {0} while running the '{1}' query: {2}",
                        ex.Code,
                        queryName,
                        ex.Message),
                    ex);
            }
        }

        private static string Text(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column))
            {
                return string.Empty;
            }

            object value = row[column];
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static DateTime? NullableDate(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column))
            {
                return null;
            }

            object value = row[column];
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            return ToDate(value);
        }

        private static DateTime DateValue(DataRow row, string column, DateTime fallback)
        {
            DateTime? parsed = NullableDate(row, column);
            return parsed.HasValue ? parsed.Value : fallback;
        }

        private static double NumberValue(DataRow row, string column, double fallback)
        {
            if (!row.Table.Columns.Contains(column))
            {
                return fallback;
            }

            object value = row[column];
            if (value == null || value == DBNull.Value)
            {
                return fallback;
            }

            try
            {
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return fallback;
            }
        }

        /// <summary>
        /// OracleClient hands back DATE and TIMESTAMP columns as DateTime; TO_CHAR columns would
        /// arrive as string, so both are accepted.
        /// </summary>
        private static DateTime? ToDate(object value)
        {
            DateTime date;
            if (value is DateTime)
            {
                return (DateTime)value;
            }

            if (value is OracleDateTime)
            {
                return ((OracleDateTime)value).Value;
            }

            if (DateTime.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date;
            }

            return null;
        }
    }

    // =========================================================================================
    //  Violation rules (BRD) applied on top of the resolved attendance segments
    // =========================================================================================

    internal static class ViolationDetector
    {
        public static ViolationResult Detect(
            IList<AttendanceRow> segments,
            IList<MissingPunchRow> missingIn,
            IList<StreakRow> streaks,
            ShiftInfo shift,
            string locationCode,
            DateTime attendanceDate)
        {
            double highHours = AppSettings.OverstayHighHours;
            double mediumHours = AppSettings.OverstayMediumHours;
            double highMinutes = highHours * 60.0;
            double mediumMinutes = mediumHours * 60.0;
            int weeklyOffDays = AppSettings.ConsecutiveWeeklyOffDays;
            int highRiskDays = AppSettings.ConsecutiveHighRiskDays;

            ViolationResult result = new ViolationResult();
            result.LocationCode = locationCode;
            result.AttendanceDate = attendanceDate;
            result.Shift = shift;
            result.GeneratedOn = DateTime.Now;
            result.SegmentsRead = segments.Count;

            bool allShifts = shift == null || string.Equals(shift.Code, "ALL", StringComparison.OrdinalIgnoreCase);
            string shiftCode = shift == null ? string.Empty : shift.Code;

            Dictionary<string, HashSet<string>> workersPerShift = new Dictionary<string, HashSet<string>>();
            HashSet<string> allWorkers = new HashSet<string>();

            foreach (AttendanceRow row in segments)
            {
                allWorkers.Add(row.SafetyPassNo);

                if (!workersPerShift.ContainsKey(row.ShiftCode))
                {
                    workersPerShift[row.ShiftCode] = new HashSet<string>();
                }

                workersPerShift[row.ShiftCode].Add(row.SafetyPassNo);
            }

            foreach (KeyValuePair<string, HashSet<string>> pair in workersPerShift)
            {
                result.PunchCountByShift[pair.Key] = pair.Value.Count;
            }

            result.TotalWorkers = allWorkers.Count;

            HashSet<string> workersInShift = new HashSet<string>();
            if (!allShifts && workersPerShift.ContainsKey(shiftCode))
            {
                workersInShift = workersPerShift[shiftCode];
            }

            result.WorkersInShift = allShifts ? allWorkers.Count : workersInShift.Count;

            // ---------- Overstay / continuous shift / missing Out punch ----------
            foreach (IGrouping<string, AttendanceRow> worker in segments.GroupBy(r => r.SafetyPassNo))
            {
                List<AttendanceRow> rows = worker.OrderBy(r => r.SequenceNo).ToList();
                List<string> workerShifts = rows.Select(r => r.ShiftCode).Distinct().ToList();

                if (!allShifts && !workerShifts.Contains(shiftCode))
                {
                    continue;
                }

                AttendanceRow first = rows[0];
                DateTime? inTime = rows.Where(r => r.InTime.HasValue).Select(r => r.InTime).Min();
                DateTime? outTime = rows.Where(r => r.OutTime.HasValue).Select(r => r.OutTime).Max();
                double minutes = rows.Sum(r => r.DisplayMinutes);
                string shiftText = string.Join("/", workerShifts);
                bool hasOpenSegment = rows.Any(r => string.Equals(r.WorkStatus, "OPEN", StringComparison.OrdinalIgnoreCase));

                if (workerShifts.Count > 1)
                {
                    ViolationRow continuous = CreateRow(
                        first,
                        ViolationRow.ContinuousShift,
                        "Continuous shift duty (more than one shift in the same attendance day)",
                        ViolationSeverity.High,
                        "Verify the shift mapping / punches with the Time Office and correct the attendance before payroll cut-off",
                        string.Format(CultureInfo.InvariantCulture, "{0} shifts in the day ({1}); total hours {2}", workerShifts.Count, shiftText, MinutesToHHmm(minutes)),
                        minutes / 60.0,
                        inTime,
                        outTime,
                        attendanceDate);
                    continuous.ShiftCode = shiftText;
                    result.Violations.Add(continuous);
                }
                else if (!hasOpenSegment && minutes >= highMinutes)
                {
                    result.Violations.Add(CreateRow(
                        first,
                        ViolationRow.OverstayHigh,
                        string.Format(CultureInfo.InvariantCulture, "Overstay - inside premises {0:0.##} hrs or more", highHours),
                        ViolationSeverity.High,
                        "Enquire the reason for extended stay, verify that the Out punch is correct and take corrective action",
                        string.Format(CultureInfo.InvariantCulture, "Total hours inside premises {0}", MinutesToHHmm(minutes)),
                        minutes / 60.0,
                        inTime,
                        outTime,
                        attendanceDate));
                }
                else if (!hasOpenSegment && minutes >= mediumMinutes)
                {
                    result.Violations.Add(CreateRow(
                        first,
                        ViolationRow.OverstayMedium,
                        string.Format(CultureInfo.InvariantCulture, "Overstay - inside premises between {0:0.##} and {1:0.##} hrs", mediumHours, highHours),
                        ViolationSeverity.Medium,
                        "Monitor and record the reason in the attendance register",
                        string.Format(CultureInfo.InvariantCulture, "Total hours inside premises {0}", MinutesToHHmm(minutes)),
                        minutes / 60.0,
                        inTime,
                        outTime,
                        attendanceDate));
                }

                if (hasOpenSegment && !outTime.HasValue)
                {
                    result.Violations.Add(CreateRow(
                        first,
                        ViolationRow.MissingOutPunch,
                        "Missing Out punch",
                        ViolationSeverity.High,
                        "Get the Out punch regularised by the Time Office with the actual exit time and CA/CO confirmation",
                        "Attendance segment left open - no Out punch after the In punch",
                        0,
                        inTime,
                        null,
                        attendanceDate));
                }
            }

            // ---------- Consecutive working days ----------
            foreach (StreakRow streak in streaks)
            {
                if (!allShifts && !workersInShift.Contains(streak.SafetyPassNo))
                {
                    continue;
                }

                bool highRisk = streak.ConsecutiveDays >= highRiskDays;
                ViolationRow row = new ViolationRow();
                row.ViolationType = ViolationRow.ConsecutiveDays;
                row.ViolationName = highRisk
                    ? string.Format(CultureInfo.InvariantCulture, "Continuous working {0} or more days without weekly off", highRiskDays)
                    : string.Format(CultureInfo.InvariantCulture, "Continuous working more than {0} days without weekly off", weeklyOffDays);
                row.Severity = highRisk ? ViolationSeverity.High : ViolationSeverity.Medium;
                row.RequiredAction = highRisk
                    ? "Immediate intervention - give a weekly off in the next roster cycle and record the deviation"
                    : "Plan a weekly off in the roster and record the deviation";
                row.LocationCode = streak.LocationCode;
                row.DepartmentCode = streak.DepartmentCode;
                row.DepartmentName = streak.DepartmentName;
                row.SafetyPassNo = streak.SafetyPassNo;
                row.WorkerName = streak.WorkerName;
                row.VendorCode = streak.VendorCode;
                row.VendorName = streak.VendorName;
                row.AttendanceDate = attendanceDate;
                row.ShiftCode = string.Empty;
                row.TotalDays = streak.ConsecutiveDays;
                row.StartDate = streak.StartDate;
                row.EndDate = streak.EndDate;
                row.Remarks = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} consecutive working days from {1:dd-MM-yyyy} to {2:dd-MM-yyyy}",
                    streak.ConsecutiveDays,
                    streak.StartDate,
                    streak.EndDate);

                result.Violations.Add(row);
            }

            // ---------- Missing In punch ----------
            foreach (MissingPunchRow punch in missingIn)
            {
                // These workers produce no attendance segment (their only punch is an Out punch),
                // so the shift is derived from the punch time.
                string punchShift = ShiftResolver.InferShiftFromOutPunch(punch.FirstPunchTime);
                if (!allShifts && !string.Equals(punchShift, shiftCode, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                ViolationRow row = new ViolationRow();
                row.ViolationType = ViolationRow.MissingInPunch;
                row.ViolationName = "Missing In punch";
                row.Severity = ViolationSeverity.High;
                row.RequiredAction = "Get the In punch regularised by the Time Office with the actual entry time and CA/CO confirmation";
                row.LocationCode = punch.LocationCode;
                row.DepartmentCode = punch.DepartmentCode;
                row.DepartmentName = punch.DepartmentName;
                row.SafetyPassNo = punch.SafetyPassNo;
                row.WorkerName = punch.WorkerName;
                row.VendorCode = punch.VendorCode;
                row.VendorName = punch.VendorName;
                row.AttendanceDate = attendanceDate;
                row.ShiftCode = punchShift;
                row.OutTime = punch.FirstPunchTime;
                row.Remarks = string.Format(
                    CultureInfo.InvariantCulture,
                    "First punch of the day is an Out punch at {0:HH:mm}; no In punch recorded",
                    punch.FirstPunchTime);

                result.Violations.Add(row);
            }

            // High concern first, then by type and worker.
            List<ViolationRow> ordered = result.Violations
                .OrderBy(v => SeverityRank(v.Severity))
                .ThenBy(v => v.ViolationType)
                .ThenBy(v => v.SafetyPassNo)
                .ToList();

            result.Violations.Clear();
            result.Violations.AddRange(ordered);

            return result;
        }

        private static int SeverityRank(string severity)
        {
            if (severity == ViolationSeverity.High)
            {
                return 0;
            }

            if (severity == ViolationSeverity.Medium)
            {
                return 1;
            }

            return 2;
        }

        private static ViolationRow CreateRow(
            AttendanceRow source,
            string type,
            string name,
            string severity,
            string requiredAction,
            string remarks,
            double hours,
            DateTime? inTime,
            DateTime? outTime,
            DateTime attendanceDate)
        {
            ViolationRow row = new ViolationRow();
            row.ViolationType = type;
            row.ViolationName = name;
            row.Severity = severity;
            row.RequiredAction = requiredAction;
            row.LocationCode = source.LocationCode;
            row.DepartmentCode = source.DepartmentCode;
            row.DepartmentName = source.DepartmentName;
            row.SafetyPassNo = source.SafetyPassNo;
            row.WorkerName = source.WorkerName;
            row.VendorCode = source.VendorCode;
            row.VendorName = source.VendorName;
            row.NatureOfWork = source.NatureOfWork;
            row.WorkOrderNo = source.WorkOrderNo;
            row.CaCoName = source.CaCoName;
            row.AttendanceDate = attendanceDate;
            row.ShiftCode = source.ShiftCode;
            row.InTime = inTime;
            row.OutTime = outTime;
            row.TotalHours = Math.Round(hours, 2);
            row.Remarks = remarks;
            return row;
        }

        private static string MinutesToHHmm(double minutes)
        {
            int total = (int)Math.Round(minutes);
            if (total < 0)
            {
                total = 0;
            }

            return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}", total / 60, total % 60);
        }
    }

    // =========================================================================================
    //  Excel attachment
    // =========================================================================================

    internal static class ExcelReportBuilder
    {
        private static readonly string[] Headers =
        {
            "Location", "Department", "Safety Pass No", "Worker Name", "Vendor Code", "Vendor Name",
            "WO No", "CA/CO Name", "Date", "Shift", "Violation Type", "Severity",
            "In Time", "Out Time", "Total Hours", "No. of Days", "Start Date", "End Date",
            "Remarks", "Required Action"
        };

        public static string Build(ViolationResult result, string outputFolder)
        {
            string fileName = string.Format(
                CultureInfo.InvariantCulture,
                "Attendance_Violation_{0}_Shift{1}_{2:yyyyMMdd_HHmmss}.xlsx",
                result.LocationCode,
                result.Shift == null ? "ALL" : result.Shift.Code,
                result.AttendanceDate);

            string path = Path.Combine(outputFolder, fileName);

            using (XLWorkbook workbook = new XLWorkbook())
            {
                WriteSummary(workbook, result);
                WriteSheet(workbook, "Overstay High", result.OfType(ViolationRow.OverstayHigh).ToList());
                WriteSheet(workbook, "Overstay Medium", result.OfType(ViolationRow.OverstayMedium).ToList());
                WriteSheet(workbook, "Continuous Shift", result.OfType(ViolationRow.ContinuousShift).ToList());
                WriteSheet(workbook, "Consecutive Days", result.OfType(ViolationRow.ConsecutiveDays).ToList());
                WriteSheet(workbook, "Missing In Punch", result.OfType(ViolationRow.MissingInPunch).ToList());
                WriteSheet(workbook, "Missing Out Punch", result.OfType(ViolationRow.MissingOutPunch).ToList());
                workbook.SaveAs(path);
            }

            return path;
        }

        private static void WriteSummary(XLWorkbook workbook, ViolationResult result)
        {
            IXLWorksheet sheet = workbook.Worksheets.Add("Summary");

            List<KeyValuePair<string, string>> info = new List<KeyValuePair<string, string>>();
            info.Add(new KeyValuePair<string, string>("Location", result.LocationCode));
            info.Add(new KeyValuePair<string, string>("Attendance Date", result.AttendanceDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)));
            info.Add(new KeyValuePair<string, string>("Shift", result.Shift == null ? "ALL" : result.Shift.Code + " (" + result.Shift.WindowText + ")"));
            info.Add(new KeyValuePair<string, string>("Generated On", result.GeneratedOn.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture)));
            info.Add(new KeyValuePair<string, string>("Workers in shift", result.WorkersInShift.ToString(CultureInfo.InvariantCulture)));
            info.Add(new KeyValuePair<string, string>("Total workers in attendance", result.TotalWorkers.ToString(CultureInfo.InvariantCulture)));
            info.Add(new KeyValuePair<string, string>("Attendance segments read", result.SegmentsRead.ToString(CultureInfo.InvariantCulture)));
            info.Add(new KeyValuePair<string, string>("Total violations", result.Violations.Count.ToString(CultureInfo.InvariantCulture)));
            info.Add(new KeyValuePair<string, string>("High concern violations", result.HighRiskCount.ToString(CultureInfo.InvariantCulture)));

            int row = 1;
            foreach (KeyValuePair<string, string> pair in info)
            {
                sheet.Cell(row, 1).Value = pair.Key;
                sheet.Cell(row, 1).Style.Font.Bold = true;
                sheet.Cell(row, 2).Value = pair.Value;
                row++;
            }

            row++;
            sheet.Cell(row, 1).Value = "Violation Type";
            sheet.Cell(row, 2).Value = "Code";
            sheet.Cell(row, 3).Value = "Count";
            HeaderStyle(sheet.Range(row, 1, row, 3));
            row++;

            string[] types =
            {
                ViolationRow.OverstayHigh, ViolationRow.OverstayMedium, ViolationRow.ContinuousShift,
                ViolationRow.ConsecutiveDays, ViolationRow.MissingInPunch, ViolationRow.MissingOutPunch
            };

            foreach (string type in types)
            {
                sheet.Cell(row, 1).Value = ViolationName(type);
                sheet.Cell(row, 2).Value = type;
                sheet.Cell(row, 3).Value = result.CountOf(type);
                row++;
            }

            sheet.Columns().AdjustToContents();
        }

        private static void WriteSheet(XLWorkbook workbook, string sheetName, List<ViolationRow> rows)
        {
            IXLWorksheet sheet = workbook.Worksheets.Add(sheetName);

            for (int column = 0; column < Headers.Length; column++)
            {
                sheet.Cell(1, column + 1).Value = Headers[column];
            }

            HeaderStyle(sheet.Range(1, 1, 1, Headers.Length));

            int row = 2;
            foreach (ViolationRow item in rows)
            {
                sheet.Cell(row, 1).Value = item.LocationCode;
                sheet.Cell(row, 2).Value = item.DepartmentName;
                sheet.Cell(row, 3).Value = item.SafetyPassNo;
                sheet.Cell(row, 4).Value = item.WorkerName;
                sheet.Cell(row, 5).Value = item.VendorCode;
                sheet.Cell(row, 6).Value = item.VendorName;
                sheet.Cell(row, 7).Value = item.WorkOrderNo;
                sheet.Cell(row, 8).Value = item.CaCoName;
                sheet.Cell(row, 9).Value = item.AttendanceDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                sheet.Cell(row, 10).Value = item.ShiftCode;
                sheet.Cell(row, 11).Value = ViolationName(item.ViolationType);
                sheet.Cell(row, 12).Value = item.Severity;
                sheet.Cell(row, 13).Value = item.InTimeText;
                sheet.Cell(row, 14).Value = item.OutTimeText;
                sheet.Cell(row, 15).Value = item.TotalHours;
                sheet.Cell(row, 16).Value = item.TotalDays;
                sheet.Cell(row, 17).Value = item.StartDate.HasValue ? item.StartDate.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) : string.Empty;
                sheet.Cell(row, 18).Value = item.EndDate.HasValue ? item.EndDate.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) : string.Empty;
                sheet.Cell(row, 19).Value = item.Remarks;
                sheet.Cell(row, 20).Value = item.RequiredAction;
                row++;
            }

            if (rows.Count > 0)
            {
                sheet.Range(1, 1, row - 1, Headers.Length).SetAutoFilter();
            }

            sheet.SheetView.FreezeRows(1);
            sheet.Columns().AdjustToContents();
        }

        private static void HeaderStyle(IXLRange range)
        {
            range.Style.Font.Bold = true;
            range.Style.Font.FontColor = XLColor.White;
            range.Style.Fill.BackgroundColor = XLColor.FromArgb(31, 56, 100);
            range.Style.Alignment.WrapText = true;
        }

        public static string ViolationName(string violationType)
        {
            switch (violationType)
            {
                case ViolationRow.OverstayHigh:
                    return "Overstay - high concern";
                case ViolationRow.OverstayMedium:
                    return "Overstay - medium";
                case ViolationRow.ContinuousShift:
                    return "Continuous shift duty";
                case ViolationRow.ConsecutiveDays:
                    return "Continuous working days";
                case ViolationRow.MissingInPunch:
                    return "Missing In punch";
                case ViolationRow.MissingOutPunch:
                    return "Missing Out punch";
                default:
                    return violationType;
            }
        }
    }

    // =========================================================================================
    //  Mail
    // =========================================================================================

    internal static class EmailSender
    {
        public static string BuildSubject(ViolationResult result)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "[{0}] {1} - Shift {2} - {3:dd-MM-yyyy} - {4} case(s)",
                AppSettings.MailSubjectPrefix,
                result.LocationCode,
                result.Shift == null ? "ALL" : result.Shift.Code,
                result.AttendanceDate,
                result.Violations.Count);
        }

        public static string BuildBody(ViolationResult result)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<html><body style=\"font-family:Segoe UI,Arial,sans-serif;font-size:13px;color:#222;\">");
            html.Append("<p>Dear Sir/Madam,</p>");
            html.Append("<p>Attendance validation for the shift mentioned below has been completed. ");
            html.Append("The violations identified under the attendance violation rules are listed for corrective action.</p>");

            html.Append("<table cellspacing=\"0\" cellpadding=\"6\" style=\"border-collapse:collapse;font-size:13px;margin-bottom:14px;\">");
            html.Append(Row("Location", result.LocationCode));
            html.Append(Row("Attendance Date", result.AttendanceDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)));
            html.Append(Row("Shift", result.Shift == null ? "ALL" : result.Shift.Code + " (" + result.Shift.WindowText + ")"));
            html.Append(Row("Workers in this shift", result.WorkersInShift.ToString(CultureInfo.InvariantCulture)));
            html.Append(Row("Total violations", result.Violations.Count.ToString(CultureInfo.InvariantCulture)));
            html.Append(Row("High concern", result.HighRiskCount.ToString(CultureInfo.InvariantCulture)));
            html.Append(Row("Generated on", result.GeneratedOn.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture)));
            html.Append("</table>");

            html.Append("<h4 style=\"margin:18px 0 6px 0;color:#1F3864;\">Violation summary</h4>");
            html.Append("<table cellspacing=\"0\" cellpadding=\"6\" style=\"border-collapse:collapse;font-size:13px;border:1px solid #d0d0d0;\">");
            html.Append("<tr style=\"background:#1F3864;color:#fff;\"><th align=\"left\">Violation Type</th><th align=\"left\">Code</th><th align=\"left\">Count</th></tr>");

            string[] types =
            {
                ViolationRow.OverstayHigh, ViolationRow.OverstayMedium, ViolationRow.ContinuousShift,
                ViolationRow.ConsecutiveDays, ViolationRow.MissingInPunch, ViolationRow.MissingOutPunch
            };

            foreach (string type in types)
            {
                html.Append("<tr><td style=\"border:1px solid #e0e0e0;\">").Append(ExcelReportBuilder.ViolationName(type))
                    .Append("</td><td style=\"border:1px solid #e0e0e0;\">").Append(type)
                    .Append("</td><td style=\"border:1px solid #e0e0e0;\">").Append(result.CountOf(type).ToString(CultureInfo.InvariantCulture))
                    .Append("</td></tr>");
            }
            html.Append("</table>");

            if (result.PunchCountByShift.Count > 0)
            {
                html.Append("<h4 style=\"margin:18px 0 6px 0;color:#1F3864;\">Punches received in the shift(s)</h4>");
                html.Append("<table cellspacing=\"0\" cellpadding=\"6\" style=\"border-collapse:collapse;font-size:13px;border:1px solid #d0d0d0;\">");
                html.Append("<tr style=\"background:#1F3864;color:#fff;\"><th align=\"left\">Shift</th><th align=\"left\">Workers</th></tr>");
                foreach (KeyValuePair<string, int> pair in result.PunchCountByShift.OrderBy(p => p.Key))
                {
                    html.Append("<tr><td style=\"border:1px solid #e0e0e0;\">").Append(pair.Key)
                        .Append("</td><td style=\"border:1px solid #e0e0e0;\">").Append(pair.Value.ToString(CultureInfo.InvariantCulture))
                        .Append("</td></tr>");
                }
                html.Append("</table>");
            }

            int limit = AppSettings.MaxViolationRowsInMail;
            List<ViolationRow> shown = result.Violations.Take(limit).ToList();

            if (shown.Count > 0)
            {
                html.Append("<h4 style=\"margin:18px 0 6px 0;color:#1F3864;\">Violation details</h4>");
                html.Append("<table cellspacing=\"0\" cellpadding=\"5\" style=\"border-collapse:collapse;font-size:12px;border:1px solid #d0d0d0;\">");
                html.Append("<tr style=\"background:#1F3864;color:#fff;\">");

                string[] headers =
                {
                    "Safety Pass No", "Employee Name", "Department", "Date", "Shift",
                    "Violation Type", "In", "Out", "Hours", "Remarks", "Required Action"
                };

                foreach (string header in headers)
                {
                    html.Append("<th align=\"left\">").Append(header).Append("</th>");
                }
                html.Append("</tr>");

                bool alternate = false;
                foreach (ViolationRow row in shown)
                {
                    html.Append("<tr").Append(alternate ? " style=\"background:#f5f7fb;\"" : string.Empty).Append(">");
                    alternate = !alternate;
                    AppendCell(html, row.SafetyPassNo);
                    AppendCell(html, row.WorkerName);
                    AppendCell(html, row.DepartmentName);
                    AppendCell(html, row.AttendanceDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture));
                    AppendCell(html, row.ShiftCode);
                    AppendCell(html, ExcelReportBuilder.ViolationName(row.ViolationType) + " (" + row.Severity + ")");
                    AppendCell(html, row.InTimeText);
                    AppendCell(html, row.OutTimeText);
                    AppendCell(html, row.ViolationType == ViolationRow.ConsecutiveDays
                        ? row.TotalDays + " days"
                        : row.TotalHours.ToString("0.00", CultureInfo.InvariantCulture));
                    AppendCell(html, row.Remarks);
                    AppendCell(html, row.RequiredAction);
                    html.Append("</tr>");
                }
                html.Append("</table>");

                if (result.Violations.Count > shown.Count)
                {
                    html.Append("<p style=\"font-size:12px;color:#555;\">")
                        .Append(string.Format(CultureInfo.InvariantCulture, "{0} more violation(s) are available in the attached Excel report.", result.Violations.Count - shown.Count))
                        .Append("</p>");
                }
            }
            else
            {
                html.Append("<p style=\"padding:10px;background:#e8f5e9;border-left:4px solid #2e7d32;\">No attendance violation found for this shift.</p>");
            }

            html.Append("<p style=\"font-size:12px;color:#555;margin-top:16px;\">The complete violation list is attached as an Excel report. ");
            html.Append("Please take corrective action and update the Time Office.</p>");
            html.Append("<p style=\"font-size:12px;color:#777;\">This is a system generated mail from the Attendance Automailer batch (ACE). Please do not reply.</p>");
            html.Append("</body></html>");
            return html.ToString();
        }

        /// <summary>Sends the mail and returns a short status text for the log.</summary>
        public static string Send(ViolationResult result, string attachmentPath, List<string> to, List<string> cc)
        {
            if (to.Count == 0)
            {
                throw new InvalidOperationException("No mail recipients configured (MailTo / --to).");
            }

            if (string.IsNullOrEmpty(AppSettings.MailFrom))
            {
                throw new InvalidOperationException("MailFrom is not configured.");
            }

            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(AppSettings.MailFrom);
                foreach (string address in to)
                {
                    message.To.Add(address);
                }

                foreach (string address in cc)
                {
                    if (!to.Contains(address, StringComparer.OrdinalIgnoreCase))
                    {
                        message.CC.Add(address);
                    }
                }

                message.Subject = BuildSubject(result);
                message.SubjectEncoding = Encoding.UTF8;
                message.Body = BuildBody(result);
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;

                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    message.Attachments.Add(new Attachment(attachmentPath));
                }

                using (SmtpClient client = new SmtpClient())
                {
                    if (string.IsNullOrEmpty(AppSettings.SmtpHost))
                    {
                        string dropFolder = Path.Combine(AppSettings.GetFolder("OutputDirectory", "Output"), "MailDrop");
                        if (!Directory.Exists(dropFolder))
                        {
                            Directory.CreateDirectory(dropFolder);
                        }

                        client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                        client.PickupDirectoryLocation = dropFolder;
                        client.Send(message);
                        return "SMTP not configured - mail written to " + dropFolder;
                    }

                    client.Host = AppSettings.SmtpHost;
                    client.Port = AppSettings.SmtpPort;
                    client.EnableSsl = AppSettings.SmtpEnableSsl;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    if (!string.IsNullOrEmpty(AppSettings.SmtpUser))
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new System.Net.NetworkCredential(AppSettings.SmtpUser, AppSettings.SmtpPassword);
                    }

                    client.Send(message);
                    return "Mail sent to " + string.Join("; ", to) + (cc.Count > 0 ? " (cc " + string.Join("; ", cc) + ")" : string.Empty);
                }
            }
        }

        /// <summary>Department wise CC recipients from DepartmentMailMap.</summary>
        public static List<string> ResolveDepartmentRecipients(ViolationResult result)
        {
            List<string> addresses = new List<string>();
            string map = AppSettings.DepartmentMailMap;
            if (string.IsNullOrEmpty(map))
            {
                return addresses;
            }

            Dictionary<string, List<string>> parsed = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (string entry in map.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int separator = entry.IndexOf('=');
                if (separator <= 0)
                {
                    continue;
                }

                string departmentCode = entry.Substring(0, separator).Trim();
                string mailList = entry.Substring(separator + 1);
                parsed[departmentCode] = mailList.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim())
                    .Where(a => a.Length > 0)
                    .ToList();
            }

            HashSet<string> departments = new HashSet<string>(
                result.Violations.Select(v => (v.DepartmentCode ?? string.Empty).Trim()),
                StringComparer.OrdinalIgnoreCase);

            foreach (string department in departments)
            {
                List<string> mapped;
                if (parsed.TryGetValue(department, out mapped))
                {
                    foreach (string address in mapped)
                    {
                        if (!addresses.Contains(address, StringComparer.OrdinalIgnoreCase))
                        {
                            addresses.Add(address);
                        }
                    }
                }
            }

            return addresses;
        }

        private static string Row(string label, string value)
        {
            return "<tr><td style=\"padding:4px 10px 4px 0;color:#555;white-space:nowrap;\">" + label +
                   "</td><td style=\"font-weight:600;\">" + value + "</td></tr>";
        }

        private static void AppendCell(StringBuilder html, string value)
        {
            html.Append("<td style=\"border:1px solid #e0e0e0;vertical-align:top;\">")
                .Append(string.IsNullOrEmpty(value) ? "&nbsp;" : value)
                .Append("</td>");
        }
    }
}
