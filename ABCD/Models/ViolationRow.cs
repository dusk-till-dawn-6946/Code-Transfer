using System;

namespace ATTENDANCE_AUTOMAILER.Models
{
    /// <summary>
    /// One attendance violation as it will be shown in the mail and the Excel attachment.
    /// Violation type codes are the ones already stored by frmTimekeeper in
    /// HRACE.T_CEMP_VIOLATION_ACTION (CVA_VIOLATION_TYPE).
    /// </summary>
    public class ViolationRow
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

        public string InTimeText { get { return FormatTime(InTime); } }
        public string OutTimeText { get { return FormatTime(OutTime); } }

        private static string FormatTime(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("HH:mm") : string.Empty;
        }
    }
}
