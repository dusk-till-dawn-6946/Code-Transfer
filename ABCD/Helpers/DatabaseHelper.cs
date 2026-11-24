using System.Configuration;
using System.Data.OracleClient;

namespace ATTENDANCE_AUTOMAILER.Helpers
{
    public static class DatabaseHelper
    {
        public static OracleConnection GetConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["OraConnGatepass"].ConnectionString;
            return new OracleConnection(connectionString);
        }
    }
}
