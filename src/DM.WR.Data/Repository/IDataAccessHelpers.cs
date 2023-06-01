using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DM.WR.Data.Repository
{
    public interface IDataAccessHelpers
    {
        string BuildFancyErrorMessage(OracleCommand cmd, string functionName, string exceptionMessage);
        bool ColumnExists(IDataReader dataReader, string columnName);
        string GetFullPackageName(string storedProcedureName);
        void ValidateOracleDataReaderWithFancyMessage(OracleDataReader odr, OracleCommand cmd, string functionName);
    }
}