using DM.WR.Data.Config;
using DM.WR.Data.Logging.Types;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;

namespace DM.WR.Data.Logging
{
    public class DbLogger : IDbLogger
    {
        private readonly string _systemName = ConfigSettings.SystemName;

        // Get logging info
        public LoggingInfo GetLoggingInfo()
        {
            var logging = new LoggingInfo();
            if (!ConfigSettings.IsDbLoggingOn)
                return logging;

            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand("sdr_logging_pkg.get_logging_info", con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("in_outputsystem_name", OracleDbType.Varchar2, 8)).Value = _systemName;
                cmd.Parameters.Add(new OracleParameter("out_outsys_logging_flag", OracleDbType.Double)).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new OracleParameter("out_component_logging_string", OracleDbType.Varchar2, 40)).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                if (cmd.Parameters["out_outsys_logging_flag"].Value != null)
                    logging.LoggingFlag = !((OracleDecimal)cmd.Parameters["out_outsys_logging_flag"].Value).IsZero;

                if (cmd.Parameters["out_component_logging_string"].Value != null)
                    logging.ComponentLoggingString = cmd.Parameters["out_component_logging_string"].Value.ToString();
            }
            return logging;
        }

        public decimal LogRequestBegin(int sessionId, string reportCode, int customerId, string locationGuid, bool runInBackground, string query)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand("SDR_LOGGING_PKG.LOG_IRMMENU_REQUEST_BEGIN", con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_IRMSESSION_ID", OracleDbType.Int32, 20)).Value = sessionId;
                cmd.Parameters.Add(new OracleParameter("IN_OUTPUTSYSTEM_NAME", OracleDbType.Varchar2, 20)).Value = _systemName;
                cmd.Parameters.Add(new OracleParameter("IN_IRMREPORT_CODE", OracleDbType.Varchar2, 6)).Value = reportCode;
                cmd.Parameters.Add(new OracleParameter("IN_REQUESTING_CUSTOMER_ID", OracleDbType.Double)).Value = customerId;
                cmd.Parameters.Add(new OracleParameter("IN_RQST_USER_LOCATION_GUID", OracleDbType.Varchar2, 60)).Value = locationGuid;
                cmd.Parameters.Add(new OracleParameter("IN_RUN_IN_BACKGROUND", OracleDbType.Int32)).Value = runInBackground ? 1 : 0;
                cmd.Parameters.Add(new OracleParameter("IN_ACTUATE_REQUEST_STRING", OracleDbType.Varchar2, 2000)).Value = query;
                cmd.Parameters.Add(new OracleParameter("OUT_SDR_REQUEST_ID", OracleDbType.Double)).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                if (!((OracleDecimal)cmd.Parameters["OUT_SDR_REQUEST_ID"].Value).IsNull)
                    return ((OracleDecimal)cmd.Parameters["OUT_SDR_REQUEST_ID"].Value).Value;
            }
            return -1;
        }

        public void LogMessageToDb(int? customerId, string userName, string logType, string message)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand("SDR_LOGGING_PKG.LOG_IRM_MENU_ENTRY", con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_OUTPUTSYSTEM_NAME", OracleDbType.Varchar2, 30)).Value = _systemName;
                cmd.Parameters.Add(new OracleParameter("IN_REQUESTING_CUSTOMER_ID", OracleDbType.Int64, 12)).Value = customerId;
                cmd.Parameters.Add(new OracleParameter("IN_RQST_USER_LOCATION_GUID", OracleDbType.Varchar2, 60)).Value = userName;
                cmd.Parameters.Add(new OracleParameter("IN_LOGGING_TYPE", OracleDbType.Varchar2, 1)).Value = logType;
                cmd.Parameters.Add(new OracleParameter("IN_LOGGING_MESSAGE", OracleDbType.Varchar2, 4000)).Value = message;
                cmd.Parameters.Add(new OracleParameter("OUT_SDR_IRM_MENU_LOGGING_ID", OracleDbType.Int64)).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
            }
        }

        // Log the report output string. This is the actuate url along with the parameters
        //SCWO-270 - deprecated, no created/update happens in 1 proc - LogRequestBegin function
        public void UpdateOutputString(decimal requestId, string outputString)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand("SDR_LOGGING_PKG.UPDT_REQUEST_ACT_RQST_STRING", con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("in_sdr_request_id", OracleDbType.Double)).Value = requestId;
                cmd.Parameters.Add(new OracleParameter("in_actuate_request_string", OracleDbType.Varchar2, 2000)).Value = outputString;
                cmd.ExecuteNonQuery();
            }
        }

        public int LogSessionStart(string loginSource, string requestingUserName, int? requestingUserRoleId)
        {
            using (OracleConnection con = new OracleConnection(ConfigSettings.ConnectionString))
            using (OracleCommand cmd = new OracleCommand("SDR_LOGGING_PKG.LOG_IRMSESSION", con))
            {
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("IN_OUTPUTSYSTEM_NAME", OracleDbType.Varchar2, 20)).Value = _systemName;
                cmd.Parameters.Add(new OracleParameter("IN_IRMLOGGINSOURCE_CODE", OracleDbType.Varchar2, 20)).Value = loginSource;
                cmd.Parameters.Add(new OracleParameter("IN_REQUESTING_USER_NAME", OracleDbType.Varchar2, 100)).Value = requestingUserName;
                cmd.Parameters.Add(new OracleParameter("IN_REQUESTING_USER_ROLE_ID", OracleDbType.Int32)).Value = requestingUserRoleId;
                cmd.Parameters.Add(new OracleParameter("OUT_IRMSESSION_ID", OracleDbType.Int32)).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                return Convert.ToInt32(cmd.Parameters["OUT_IRMSESSION_ID"].Value.ToString());
            }
        }
    }
}