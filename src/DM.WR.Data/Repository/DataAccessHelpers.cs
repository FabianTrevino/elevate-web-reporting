using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Text;
using DM.WR.Data.Config;

namespace DM.WR.Data.Repository
{
    public class DataAccessHelpers : IDataAccessHelpers
    {
        public string GetFullPackageName(string storedProcedureName)
        {
            var fullPackageName = new StringBuilder(ConfigSettings.SdrPackage ?? "SDR_CATALOG_MENU_PKG");
            return fullPackageName.Append(".").Append(storedProcedureName).ToString();
        }

        public bool ColumnExists(IDataReader dataReader, string columnName)
        {
            for (int i = 0; i < dataReader.FieldCount; i++)
                if (dataReader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        public void ValidateOracleDataReaderWithFancyMessage(OracleDataReader odr, OracleCommand cmd, string functionName)
        {
            if (odr == null || !odr.HasRows)
            {
                var message = BuildFancyErrorMessage(cmd, functionName, "OracleDataReader returned 0 rows.");
                throw new Exception($"Data not found: {message}.");
            }
        }

        public string BuildFancyErrorMessage(OracleCommand cmd, string functionName, string exceptionMessage)
        {
            var message = new StringBuilder($">>>>>>>>>> Function Name: {functionName}; ");
            message.Append($"Stored Procedure: {cmd.CommandText}; ");
            message.Append("Parameters: ");

            for (int c = 0; c < cmd.Parameters.Count; ++c)
                message.Append($"{cmd.Parameters[c].ParameterName}: {cmd.Parameters[c].Value}; ");

            message.Append($"Original Error Message: {exceptionMessage} <<<<<<<<<<");

            return message.ToString();
        }
    }
}