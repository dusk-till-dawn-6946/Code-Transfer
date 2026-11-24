using System;

namespace ATTENDANCE_AUTOMAILER.Models
{
    /// <summary>
    /// One operational shift. The punch windows are exactly the ones used by frmTimekeeper.aspx.vb
    /// (shift_category CASE in Get_Final_Attd_query).
    /// </summary>
    public class ShiftInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }

        /// <summary>First valid "In" punch time of day for this shift.</summary>
        public TimeSpan WindowStart { get; set; }

        /// <summary>Last valid "In" punch time of day for this shift.</summary>
        public TimeSpan WindowEnd { get; set; }

        /// <summary>Time of day at which the shift duty ends (used for the scheduler hints).</summary>
        public TimeSpan ShiftEnd { get; set; }

        /// <summary>True when the shift end falls on the next calendar day.</summary>
        public bool CrossesMidnight { get; set; }

        public override string ToString()
        {
            return Code;
        }

        public string WindowText
        {
            get { return WindowStart.ToString(@"hh\:mm") + " - " + WindowEnd.ToString(@"hh\:mm"); }
        }
    }
}
