using System;

namespace ATTENDANCE_AUTOMAILER.Models
{
    /// <summary>
    /// One resolved attendance segment (in -> out) coming from the final_attd part of the
    /// frmTimekeeper query.
    /// </summary>
    public class AttendanceRow
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
        public string DisplayBucket { get; set; }

        /// <summary>OPEN when the segment has no "Out" punch yet.</summary>
        public string WorkStatus { get; set; }
        public int SegmentCount { get; set; }
    }
}
