using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ATTENDANCE_AUTOMAILER.Helpers;
using ATTENDANCE_AUTOMAILER.Models;
using ATTENDANCE_AUTOMAILER.Services;

namespace ATTENDANCE_AUTOMAILER
{
    /// <summary>
    /// Attendance violation automailer batch.
    ///
    /// Runs after the closure of one shift, validates the attendance of that shift for one
    /// attendance day, applies the violation rules and mails the report (with an Excel attachment).
    ///
    /// Exit codes: 0 = success, 1 = error (the scheduler can alarm on it).
    /// </summary>
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

        private static int Run(Options options)
        {
            DateTime now = DateTime.Now;

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

            DateTime attendanceDate = options.AttendanceDate.HasValue
                ? options.AttendanceDate.Value.Date
                : ShiftResolver.ResolveAttendanceDate(shift, now);

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
                    "Shift window {0}, shift end {1:HH:mm}",
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

            string vendorCode = AppSettings.VendorCode;
            if (!string.IsNullOrEmpty(vendorCode))
            {
                segments = segments.Where(s => string.Equals(s.VendorCode, vendorCode, StringComparison.OrdinalIgnoreCase)).ToList();
                streaks = streaks.Where(s => string.Equals(s.VendorCode, vendorCode, StringComparison.OrdinalIgnoreCase)).ToList();
                missingIn = missingIn.Where(s => string.Equals(s.VendorCode, vendorCode, StringComparison.OrdinalIgnoreCase)).ToList();
                Logger.Info("Vendor filter applied: " + vendorCode);
            }

            string departmentCodes = AppSettings.DepartmentCodes;
            if (!string.IsNullOrEmpty(departmentCodes))
            {
                HashSet<string> allowed = new HashSet<string>(
                    departmentCodes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.Trim()),
                    StringComparer.OrdinalIgnoreCase);

                segments = segments.Where(s => allowed.Contains(s.DepartmentCode)).ToList();
                streaks = streaks.Where(s => allowed.Contains(s.DepartmentCode)).ToList();
                missingIn = missingIn.Where(s => allowed.Contains(s.DepartmentCode)).ToList();
                Logger.Info("Department filter applied: " + departmentCodes);
            }

            ViolationDetector detector = new ViolationDetector();
            ViolationResult result = detector.Detect(segments, missingIn, streaks, shift, locationCode, attendanceDate);

            Logger.Info(string.Format(
                CultureInfo.InvariantCulture,
                "Violations: {0} total ({1} high concern). {2}",
                result.Violations.Count,
                result.HighRiskCount,
                DescribeCounts(result)));

            if (segments.Count == 0)
            {
                Logger.Warn("No attendance segments were found for this shift / date. Check the shift window and the attendance data.");
            }

            string outputFolder = string.IsNullOrEmpty(options.OutputFolder)
                ? AppSettings.OutputDirectory
                : options.OutputFolder;

            if (!System.IO.Directory.Exists(outputFolder))
            {
                System.IO.Directory.CreateDirectory(outputFolder);
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
                Logger.Info("No violations for this shift - mail not sent (use --always-mail to send it anyway).");
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
            Console.WriteLine("Expected daily schedule:");
            Console.WriteLine(ShiftResolver.DescribeSchedule());
        }

        private class Options
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
}
