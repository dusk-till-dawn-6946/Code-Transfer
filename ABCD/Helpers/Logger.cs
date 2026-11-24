using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ATTENDANCE_AUTOMAILER.Helpers
{
    /// <summary>
    /// Minimal thread safe file + console logger for the batch. One file per day.
    /// </summary>
    public static class Logger
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
}
