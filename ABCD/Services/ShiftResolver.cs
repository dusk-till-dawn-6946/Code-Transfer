using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ATTENDANCE_AUTOMAILER.Helpers;
using ATTENDANCE_AUTOMAILER.Models;

namespace ATTENDANCE_AUTOMAILER.Services
{
    /// <summary>
    /// Shift definitions and the "which shift / which attendance day is this run for" logic.
    /// Punch windows and shift codes are identical to the shift_category CASE in
    /// frmTimekeeper's Get_Final_Attd_query.
    /// </summary>
    public static class ShiftResolver
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
        /// Shift whose duty has just ended at <paramref name="now"/>; used when the scheduler does
        /// not pass --shift explicitly.
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
        /// Attendance day (punch_date - cutoff hour) the shift belongs to.
        /// Shift C started the previous evening, everything else is the current day.
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

        /// <summary>
        /// Shift code whose punch window contains the given time of day, using exactly the windows
        /// of the shift_category CASE in the timekeeper query.
        /// </summary>
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
        /// Best guess of the shift of a punch-only worker (for example an Out punch without an In
        /// punch, which produces no attendance segment): assume an 8 hour duty before the punch.
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

        /// <summary>
        /// Scheduler hints: the four daily runs that cover every shift.
        /// </summary>
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
                    "--shift {0}  (shift {1}-{2}, run at {3:HH:mm})",
                    shift.Code,
                    shift.WindowText,
                    shift.ShiftEnd.ToString(@"hh\:mm"),
                    runAt));
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
