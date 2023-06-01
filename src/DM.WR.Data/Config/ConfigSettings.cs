using HandyStuff;
using System.Web.Configuration;

namespace DM.WR.Data.Config
{
    internal static class ConfigSettings
    {
        public static bool IsWebReportingLiteFeatureEnabled => WebConfigurationManager.AppSettings["IsWebReportingLiteFeatureEnabled"].ToBoolean();

        public static string ConnectionString => WebConfigurationManager.AppSettings["ConnectionString"];
        public static string SdrPackage => WebConfigurationManager.AppSettings["SdrPackage"];
        public static bool IsDbLoggingOn => WebConfigurationManager.AppSettings["DbLogging"].ToBoolean();
        public static string SystemName => WebConfigurationManager.AppSettings["SystemName"];
    }
}