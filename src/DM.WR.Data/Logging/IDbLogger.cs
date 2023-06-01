using DM.WR.Data.Logging.Types;
using DM.WR.Data.Repository.Types;
using Oracle.ManagedDataAccess.Client;

namespace DM.WR.Data.Logging
{
    public interface IDbLogger
    {
        LoggingInfo GetLoggingInfo();
        decimal LogRequestBegin(int sessionId, string reportCode, int customerId, string locationGuid, bool runInBackground, string query);
        void LogMessageToDb(int? customerId, string userName, string logType, string message);
        void UpdateOutputString(decimal requestId, string outputString);
        int LogSessionStart(string loginSource, string requestingUserName, int? requestingUserRoleId);
    }
}