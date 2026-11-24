using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Globalization;
using ATTENDANCE_AUTOMAILER.Helpers;
using ATTENDANCE_AUTOMAILER.Models;
using ATTENDANCE_AUTOMAILER.Sql;

namespace ATTENDANCE_AUTOMAILER.Services
{
    /// <summary>
    /// Runs the ported timekeeper queries and maps the result sets to models.
    /// </summary>
    public class AttendanceRepository
    {
        private readonly string _locationCode;

        public AttendanceRepository(string locationCode)
        {
            _locationCode = locationCode;
        }

        /// <summary>Resolved in/out segments of the attendance day (all shifts).</summary>
        public List<AttendanceRow> GetAttendanceSegments(DateTime attendanceDate)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("p_startdate", ToOracleDate(attendanceDate));
            parameters.Add("p_locationcode", _locationCode);
            parameters.Add("p_gap_hours", AppSettings.PunchGapHours);
            parameters.Add("p_cutoff_hour", AppSettings.CutoffHour);

            DataTable table = Execute(SqlQueries.AttendanceDetails, parameters);
            List<AttendanceRow> rows = new List<AttendanceRow>();

            foreach (DataRow row in table.Rows)
            {
                AttendanceRow item = new AttendanceRow();
                item.LocationCode = Text(row, "location_code");
                item.DepartmentCode = Text(row, "dept_code");
                item.DepartmentName = Text(row, "department_name");
                item.SafetyPassNo = Text(row, "safety_pass_no");
                item.CardNo = Text(row, "card_no");
                item.WorkerName = Text(row, "worker_name");
                item.VendorCode = Text(row, "vendor_code");
                item.VendorName = Text(row, "vendor_name");
                item.NatureOfWork = Text(row, "nature_of_work");
                item.WorkOrderNo = Text(row, "wo_no");
                item.CaCoName = Text(row, "ca_co_name");
                item.AttendanceDay = Value<DateTime>(row, "attendance_day", attendanceDate);
                item.SequenceNo = (int)Value<decimal>(row, "seq_no", 0);
                item.ShiftCode = Text(row, "shift_code");
                item.InTime = NullableDate(row, "in_time");
                item.OutTime = NullableDate(row, "out_time");
                item.DisplayMinutes = (double)Value<decimal>(row, "display_minutes", 0);
                item.DisplayHours = Text(row, "display_hours");
                item.DisplayHHmm = Text(row, "total_hours");
                item.WorkStatus = Text(row, "work_status");
                item.SegmentCount = (int)Value<decimal>(row, "seg_count", 0);
                rows.Add(item);
            }

            return rows;
        }

        /// <summary>Workers whose first punch of the day is an Out punch.</summary>
        public List<MissingPunchRow> GetMissingInPunches(DateTime attendanceDate)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("p_startdate", ToOracleDate(attendanceDate));
            parameters.Add("p_locationcode", _locationCode);
            parameters.Add("p_cutoff_hour", AppSettings.CutoffHour);

            DataTable table = Execute(SqlQueries.MissingInPunches, parameters);
            List<MissingPunchRow> rows = new List<MissingPunchRow>();

            foreach (DataRow row in table.Rows)
            {
                MissingPunchRow item = new MissingPunchRow();
                item.LocationCode = Text(row, "location_code");
                item.DepartmentCode = Text(row, "dept_code");
                item.DepartmentName = Text(row, "department_name");
                item.SafetyPassNo = Text(row, "safety_pass_no");
                item.CardNo = Text(row, "card_no");
                item.WorkerName = Text(row, "worker_name");
                item.VendorCode = Text(row, "vendor_code");
                item.VendorName = Text(row, "vendor_name");
                item.WorkOrderNo = Text(row, "wo_no");
                item.FirstPunchTime = Value<DateTime>(row, "first_punch_time", attendanceDate);
                rows.Add(item);
            }

            return rows;
        }

        /// <summary>Consecutive working day streaks ending on the attendance day.</summary>
        public List<StreakRow> GetContinuousStreaks(DateTime attendanceDate, int minDays)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("p_enddate", ToOracleDate(attendanceDate));
            parameters.Add("p_locationcode", _locationCode);
            parameters.Add("p_min_days", minDays);

            DataTable table = Execute(SqlQueries.ContinuousStreaks, parameters);
            List<StreakRow> rows = new List<StreakRow>();

            foreach (DataRow row in table.Rows)
            {
                StreakRow item = new StreakRow();
                item.LocationCode = Text(row, "location_code");
                item.DepartmentCode = Text(row, "dept_code");
                item.DepartmentName = Text(row, "department_name");
                item.SafetyPassNo = Text(row, "safety_pass_no");
                item.WorkerName = Text(row, "worker_name");
                item.VendorCode = Text(row, "vendor_code");
                item.VendorName = Text(row, "vendor_name");
                item.StartDate = Value<DateTime>(row, "start_date", attendanceDate);
                item.EndDate = Value<DateTime>(row, "end_date", attendanceDate);
                item.ConsecutiveDays = (int)Value<decimal>(row, "consecutive_days", 0);
                rows.Add(item);
            }

            return rows;
        }

        public static string ToOracleDate(DateTime value)
        {
            return value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
        }

        private static DataTable Execute(string sql, Dictionary<string, object> parameters)
        {
            using (OracleConnection connection = DatabaseHelper.GetConnection())
            {
                using (OracleCommand command = new OracleCommand(sql, connection))
                {
                    command.CommandTimeout = 900;

                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        OracleParameter oracleParameter = new OracleParameter();
                        oracleParameter.ParameterName = parameter.Key;
                        oracleParameter.Value = parameter.Value;
                        command.Parameters.Add(oracleParameter);
                    }

                    connection.Open();
                    DataTable table = new DataTable();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }

                    return table;
                }
            }
        }

        private static string Text(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column))
            {
                return string.Empty;
            }

            object value = row[column];
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static DateTime? NullableDate(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column))
            {
                return null;
            }

            object value = row[column];
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            DateTime parsed;
            if (value is DateTime)
            {
                return (DateTime)value;
            }

            if (DateTime.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                return parsed;
            }

            return null;
        }

        private static T Value<T>(DataRow row, string column, T fallback)
        {
            if (!row.Table.Columns.Contains(column))
            {
                return fallback;
            }

            object value = row[column];
            if (value == null || value == DBNull.Value)
            {
                return fallback;
            }

            try
            {
                if (typeof(T) == typeof(DateTime))
                {
                    DateTime? parsed = NullableDate(row, column);
                    if (parsed.HasValue)
                    {
                        return (T)(object)parsed.Value;
                    }

                    return fallback;
                }

                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return fallback;
            }
        }
    }
}
