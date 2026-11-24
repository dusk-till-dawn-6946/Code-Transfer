using System;

namespace ATTENDANCE_AUTOMAILER.Models
{
    /// <summary>
    /// Worker whose first punch of the attendance day is an "Out" punch, i.e. the In punch is missing.
    /// </summary>
    public class MissingPunchRow
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
}
