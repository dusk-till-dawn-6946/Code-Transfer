using System;

namespace ATTENDANCE_AUTOMAILER.Models
{
    /// <summary>
    /// A streak of consecutive working days (gap-and-islands over daily In punches) ending on the
    /// attendance date, as used by the "consecutive working days" violation of frmTimekeeper.
    /// </summary>
    public class StreakRow
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
}
