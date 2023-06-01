using HandyStuff;
using System;
using System.Web.Configuration;

namespace DM.WR.Models.Config
{
    public static class ConfigSettings
    {

        public static string SessionTimeOut => WebConfigurationManager.AppSettings["SessionTimeOut"];
        public static string TaskUrl => WebConfigurationManager.AppSettings["TaskUrl"];
        public static string FolderPath => WebConfigurationManager.AppSettings["folderPath"];
        public static string SampleJsonData => WebConfigurationManager.AppSettings["SampleJsonData"];
        public static string SamplePopulationJson => WebConfigurationManager.AppSettings["SamplePopulationJson"];
        public static string ConnectionString => WebConfigurationManager.AppSettings["ConnectionString"];
        public static string RosterUrl => WebConfigurationManager.AppSettings["RosterUrl"];
        public static string ElevateApiUrl => WebConfigurationManager.AppSettings["ElevateApiUrl"];
        public static string AuthUrl => WebConfigurationManager.AppSettings["AuthUrl"];
        public static string ReportList => WebConfigurationManager.AppSettings["ReportList"];
        public static string SystemName => WebConfigurationManager.AppSettings["SystemName"];
        public static string AlertTimeToSessionEnd => WebConfigurationManager.AppSettings["AlertTimeToSessionEnd"];
        public static string ElevateReportingEngineUrl => WebConfigurationManager.AppSettings["ElevateReportingEngineUrl"];
        public static string ReturnUrl => WebConfigurationManager.AppSettings["ReturnUrl"];
        public static string ElevateBaseUrl => WebConfigurationManager.AppSettings["ElevateBaseUrl"];
        public static string ElevateValidation => WebConfigurationManager.AppSettings["ElevateValidation"];
        public static int OptionsCriteriaVersion => int.Parse(WebConfigurationManager.AppSettings["OptionsCriteriaVersion"]);

        public static string XmlAbsolutePath => WebConfigurationManager.AppSettings["XmlAbsolutePath"];

        public static string ActuateVolume => WebConfigurationManager.AppSettings["ActuateVolume"];

        public static string AcWebLocation => WebConfigurationManager.AppSettings["AcWebLocation"];

        public static string AcCommandExecute => WebConfigurationManager.AppSettings["AcCommandExecute"];

        public static string AcCommandSubmit => WebConfigurationManager.AppSettings["AcCommandSubmit"];

        public static string AcGeneratedReportUrl => $"{WebConfigurationManager.AppSettings["AcGeneratedReportUrl"]}?userid={AcBpUserName}&password={AcBpPassword}&";

        public static string AcGeneratedReportUrlUi => WebConfigurationManager.AppSettings["AcGeneratedReportUrl"];

        public static string AcBpUserName => WebConfigurationManager.AppSettings["AcBp_UserName"];

        public static string AcBpPassword => WebConfigurationManager.AppSettings["AcBp_Password"];

        public static string AcBpVolume => WebConfigurationManager.AppSettings["AcBp_Volume"];

        public static string DefaultGuid => WebConfigurationManager.AppSettings["LoginGuid"];

        public static bool UnderMaintenance => WebConfigurationManager.AppSettings["UnderMaintenance"].ToBoolean();

        public static string DmSecurityKey => WebConfigurationManager.AppSettings["DmSecurityKey"];

        public static string BitmapPath => WebConfigurationManager.AppSettings["BitmapPath"];

        public static string DataExportDirectory => WebConfigurationManager.AppSettings["DataExportDirectory"];

        public static string DmUrl => WebConfigurationManager.AppSettings["DmUrl"];

        public static int GraphQlResponseCacheTime => int.Parse(WebConfigurationManager.AppSettings["GraphQlResponseCacheTimeInMinutes"]);

        public static string EntryAllowedUrls => WebConfigurationManager.AppSettings["EntryAllowedUrls"];

        public static string IsTelerilEnabled => WebConfigurationManager.AppSettings["IsTelerikReportFeatureEnabled"];

        public static bool IsTelerikReportFeatureEnabled
        {
            get
            {
                if (!bool.TryParse(WebConfigurationManager.AppSettings["IsTelerikReportFeatureEnabled"], out bool telerikReportFeatureFlag))
                {
                    throw new InvalidOperationException("Invalid IsTelerikReportFeatureEnabled in web.config");
                }

                return telerikReportFeatureFlag;
            }
        }


        public static bool IsIowaFlexKto1FeatureEnabled => WebConfigurationManager.AppSettings["IsIowaFlexKto1FeatureEnabled"].ToBoolean();
        public static bool IsWebReportingLiteFeatureEnabled => WebConfigurationManager.AppSettings["IsWebReportingLiteFeatureEnabled"].ToBoolean();
        public static bool TurnOnCogatPerformanceLogging => WebConfigurationManager.AppSettings["TurnOnCogatPerformanceLogging"].ToBoolean();

        public static bool IsIowaFlexCogatEnabled => WebConfigurationManager.AppSettings["IsIowaFlexCogatEnabled"].ToBoolean();

        public static string Environment => WebConfigurationManager.AppSettings["Environment"];

        public static bool IsEnvironmentProd => WebConfigurationManager.AppSettings["Environment"] == "PROD";


        public static class DmLink
        {
            public static string UserDetailsApi => WebConfigurationManager.AppSettings["DmLink_UserDetailsApi"];
            public static string BasServices => MakeDmLink("DmLink_BasServices");


            public static string AddReportingKey => MakeDmLink("DmLink_AddReportingKey");
            public static string ProductResources => MakeDmLink("DmLink_ProductResources");
            public static string Help => WebConfigurationManager.AppSettings["DmLink_Help"];
            public static string Logout => MakeDmLink("DmLink_Logout");
            public static string Overview => MakeDmLink("DmLink_Overview");
            public static string TestEvents => MakeDmLink("DmLink_TestEvents");
            public static string Assignments => MakeDmLink("DmLink_Assignments");
            public static string Proctoring => MakeDmLink("DmLink_Proctoring");
            public static string Omr => MakeDmLink("DmLink_Omr");
            public static string AdminHome => MakeDmLink("DmLink_AdminHome");
            public static string ManageStudents => MakeDmLink("DmLink_ManageStudents");
            public static string ManageStaff => MakeDmLink("DmLink_ManageStaff");
            public static string ManageLocations => MakeDmLink("DmLink_ManageLocations");
            public static string ManageRostering => MakeDmLink("DmLink_ManageRostering");
            public static string ManageOmrScanningSettings => MakeDmLink("DmLink_ManageOmrScanningSettings");
            public static string ManageReportAccess => MakeDmLink("DmLink_ManageReportAccess");
            public static string ManageTestingActivity => MakeDmLink("DmLink_ManageTestingActivity");
            public static string ViewLicenses => MakeDmLink("DmLink_ViewLicenses");
            public static string UserGuides => MakeDmLink("DmLink_UserGuides");



            public static string TermsOfUse => WebConfigurationManager.AppSettings["DmLink_TermsOfUse"];
            public static string PrivacyPolicy => WebConfigurationManager.AppSettings["DmLink_PrivacyPolicy"];



            public static string LoginPage => MakeDmLink("DmLink_LoginPage");
            public static string CompanyWebsite => WebConfigurationManager.AppSettings["DmLink_CompanyWebsite"];
            public static string SettingsPreferences => MakeDmLink("DmLink_SettingsPreferences");

            private static string MakeDmLink(string key)
            {
                return $"{WebConfigurationManager.AppSettings["DmUrl"]}{WebConfigurationManager.AppSettings[key]}";
            }

        }

        public static class AdaptiveDashboard
        {
            public static class Auth
            {
                public static string Url => WebConfigurationManager.AppSettings["AdaptiveDashboard_Auth_Url"];
                public static string ClientId => WebConfigurationManager.AppSettings["AdaptiveDashboard_Auth_ClientId"];
                public static string ClientSecret => WebConfigurationManager.AppSettings["AdaptiveDashboard_Auth_ClientSecret"];
            }
            public static string GraphqlUrl => WebConfigurationManager.AppSettings["AdaptiveDashboard_GraphqlUrl"];
        }

        public static class BackDoorLogin
        {
            public static string UserId => WebConfigurationManager.AppSettings["BackDoorLogin_UserId"];
            public static string LocationIds => WebConfigurationManager.AppSettings["BackDoorLogin_LocationIds"];
            public static string LocationLevel => WebConfigurationManager.AppSettings["BackDoorLogin_LocationLevel"];
        }

        public static class Dashboard
        {
            // ReSharper disable once InconsistentNaming
            public static class CogAT
            {
                public static string SmiApiUrl => WebConfigurationManager.AppSettings["Dashboard_CogAT_SmiApiUrl"];
            }
        }

        public static class Demo
        {
            public static class IowaFlex
            {
                public static string UserId => WebConfigurationManager.AppSettings["Demo_IowaFlex_UserId"];
                public static string LocationIds => WebConfigurationManager.AppSettings["Demo_IowaFlex_LocationIds"];
                public static string LocationLevel => WebConfigurationManager.AppSettings["Demo_IowaFlex_LocationLevel"];
            }
        }

        public static class RsLink
        {
            public static string IowaFlex => WebConfigurationManager.AppSettings["RsLink_IowaFlex"];
        }

        public static class Email
        {
            public static string SmtpUserName => WebConfigurationManager.AppSettings["Email_SmtpUserName"];
            public static string SmtpPassword => WebConfigurationManager.AppSettings["Email_SmtpPassword"];
            public static string SmtpHost => WebConfigurationManager.AppSettings["Email_SmtpHost"];
            public static int SmtpPort => Convert.ToInt32(WebConfigurationManager.AppSettings["Email_SmtpPort"]);
            public static string CogatFeedbackMailingList => WebConfigurationManager.AppSettings["Email_CogatFeedbackMailingList"];
        }
    }
}