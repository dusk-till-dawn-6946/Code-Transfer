using System;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace ATTENDANCE_AUTOMAILER.Helpers
{
    /// <summary>
    /// Strongly typed access to the App.config appSettings used by the batch.
    /// </summary>
    public static class AppSettings
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
        /// Password is taken from the ATT_MAIL_PASSWORD environment variable first so it does not
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

        /// <summary>
        /// Resolves a possibly relative folder against the executable folder and creates it.
        /// </summary>
        public static string GetFolder(string key, string defaultFolder)
        {
            string configured = GetString(key, defaultFolder);
            string path = Path.IsPathRooted(configured)
                ? configured
                : Path.Combine(BaseDirectory, configured);

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
}
