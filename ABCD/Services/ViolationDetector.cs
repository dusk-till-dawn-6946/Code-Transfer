using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ATTENDANCE_AUTOMAILER.Helpers;
using ATTENDANCE_AUTOMAILER.Models;

namespace ATTENDANCE_AUTOMAILER.Services
{
    /// <summary>
    /// Applies the attendance violation rules of the BRD on top of the resolved attendance
    /// segments. The thresholds and type codes are the ones used by frmTimekeeper
    /// (VIOTYPE1 = >= high hours, VIOTYPE4 = medium..high hours, VIOTYPE2 = more than one shift in
    /// the same attendance day, VIOTYPE3 = consecutive working days), plus VIOTYPE5/VIOTYPE6 for
    /// missing In / Out punches.
    /// </summary>
    public class ViolationDetector
    {
        private readonly double _highMinutes;
        private readonly double _mediumMinutes;
        private readonly int _weeklyOffDays;
        private readonly int _highRiskDays;
        private readonly double _highHours;
        private readonly double _mediumHours;

        public ViolationDetector()
        {
            _highHours = AppSettings.OverstayHighHours;
            _mediumHours = AppSettings.OverstayMediumHours;
            _highMinutes = _highHours * 60.0;
            _mediumMinutes = _mediumHours * 60.0;
            _weeklyOffDays = AppSettings.ConsecutiveWeeklyOffDays;
            _highRiskDays = AppSettings.ConsecutiveHighRiskDays;
        }

        public ViolationResult Detect(
            IList<AttendanceRow> segments,
            IList<MissingPunchRow> missingIn,
            IList<StreakRow> streaks,
            ShiftInfo shift,
            string locationCode,
            DateTime attendanceDate)
        {
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
                else if (!hasOpenSegment && minutes >= _highMinutes)
                {
                    ViolationRow overstay = CreateRow(
                        first,
                        ViolationRow.OverstayHigh,
                        string.Format(CultureInfo.InvariantCulture, "Overstay - inside premises {0:0.##} hrs or more", _highHours),
                        ViolationSeverity.High,
                        "Enquire the reason for extended stay, verify that the Out punch is correct and take corrective action",
                        string.Format(CultureInfo.InvariantCulture, "Total hours inside premises {0}", MinutesToHHmm(minutes)),
                        minutes / 60.0,
                        inTime,
                        outTime,
                        attendanceDate);
                    result.Violations.Add(overstay);
                }
                else if (!hasOpenSegment && minutes >= _mediumMinutes)
                {
                    ViolationRow overstay = CreateRow(
                        first,
                        ViolationRow.OverstayMedium,
                        string.Format(CultureInfo.InvariantCulture, "Overstay - inside premises between {0:0.##} and {1:0.##} hrs", _mediumHours, _highHours),
                        ViolationSeverity.Medium,
                        "Monitor and record the reason in the attendance register",
                        string.Format(CultureInfo.InvariantCulture, "Total hours inside premises {0}", MinutesToHHmm(minutes)),
                        minutes / 60.0,
                        inTime,
                        outTime,
                        attendanceDate);
                    result.Violations.Add(overstay);
                }

                if (hasOpenSegment && !outTime.HasValue)
                {
                    ViolationRow missingOut = CreateRow(
                        first,
                        ViolationRow.MissingOutPunch,
                        "Missing Out punch",
                        ViolationSeverity.High,
                        "Get the Out punch regularised by the Time Office with the actual exit time and CA/CO confirmation",
                        "Attendance segment left open - no Out punch after the In punch",
                        0,
                        inTime,
                        null,
                        attendanceDate);
                    result.Violations.Add(missingOut);
                }
            }

            // ---------- Consecutive working days ----------
            foreach (StreakRow streak in streaks)
            {
                if (!allShifts && !workersInShift.Contains(streak.SafetyPassNo))
                {
                    continue;
                }

                bool highRisk = streak.ConsecutiveDays >= _highRiskDays;
                ViolationRow row = new ViolationRow();
                row.ViolationType = ViolationRow.ConsecutiveDays;
                row.ViolationName = highRisk
                    ? string.Format(CultureInfo.InvariantCulture, "Continuous working {0} or more days without weekly off", _highRiskDays)
                    : string.Format(CultureInfo.InvariantCulture, "Continuous working more than {0} days without weekly off", _weeklyOffDays);
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
                // These workers have no resolved attendance segment (their only punch is an Out
                // punch), so the shift is derived from the punch time instead.
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
}
