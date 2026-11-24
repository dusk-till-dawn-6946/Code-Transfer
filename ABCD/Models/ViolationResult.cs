using System;
using System.Collections.Generic;
using System.Linq;

namespace ATTENDANCE_AUTOMAILER.Models
{
    /// <summary>
    /// Everything the batch produced for one shift of one attendance day.
    /// </summary>
    public class ViolationResult
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

        /// <summary>Distinct workers per shift category for the attendance day (all shifts).</summary>
        public Dictionary<string, int> PunchCountByShift { get; private set; }

        public List<ViolationRow> Violations { get; private set; }

        public int CountOf(string violationType)
        {
            return Violations.Count(v => v.ViolationType == violationType);
        }

        public IEnumerable<ViolationRow> OfType(string violationType)
        {
            return Violations.Where(v => v.ViolationType == violationType);
        }

        public bool HasViolations
        {
            get { return Violations.Count > 0; }
        }

        public int HighRiskCount
        {
            get
            {
                return Violations.Count(v => v.Severity == ViolationSeverity.High);
            }
        }
    }

    public static class ViolationSeverity
    {
        public const string High = "High";
        public const string Medium = "Medium";
        public const string Low = "Low";
    }
}
