using System;
using System.Collections.Generic;
using System.Web;
using DM.WR.Models.Config;
using DM.WR.BL.Managers;
using DM.WR.Models.Types;
using DM.WR.Models.ViewModels;
using DM.WR.ServiceClient;
using DM.WR.ServiceClient.BackgroundReport;
using System.Threading.Tasks;

namespace DM.WR.BL.Providers
{
    public class LibraryProvider : ILibraryProvider
    {
        private readonly IActuateServiceClient _actuateServiceClient;
        private readonly ICriteriaManager _criteriaManager;
        private readonly IReportingBackgroundRepository _reportingBackgroundRepository;
        private readonly ISessionManager _sessionManager;

        private readonly UserData _userData;

        private readonly CommonProviderFunctions _apiCommon;

        public LibraryProvider(IActuateServiceClient actuateServiceClient, IUserDataManager userDataManager, ICriteriaManager criteriaManager, IReportingBackgroundRepository reportingBackgroundRepository, ISessionManager sessionManager)
        {
            _actuateServiceClient = actuateServiceClient;
            _criteriaManager = criteriaManager;
            _reportingBackgroundRepository = reportingBackgroundRepository;
            _sessionManager = sessionManager;

            _userData = userDataManager.GetUserData();

            _apiCommon = new CommonProviderFunctions();
        }

        public async Task<LibraryPageViewModel> BuildModelAsync()
        {
            //Create a user if it does not exist on the Actuate iServer
            _actuateServiceClient.CreateUser(_userData.UserId, _userData.ActuateUserId, ConfigSettings.AcBpVolume, ConfigSettings.AcBpUserName, ConfigSettings.AcBpPassword, out string error);

            var reportCenterUrl = new List<string>
            {
                $"{ConfigSettings.AcWebLocation}selectjobs.do?fromDashboard=true",
                $"userid={_userData.ActuateUserId}",
                "showBanner=false",
                "forceLogin=true"
            };

            
            return new LibraryPageViewModel
            {
                UserID = _userData.UserId + "_" + _userData.CurrentGuid,
                Password = "",
                ActuateGenerateUrl = ConfigSettings.AcGeneratedReportUrlUi.ToString(),
                ActuateWebLocation = ConfigSettings.AcWebLocation.ToString(),
                IsGuidUser = _apiCommon.IsGuidUser(_userData),
                IsTelerikReportFeatureEnabled = ConfigSettings.IsTelerilEnabled,
                QueryString = string.Join("&", reportCenterUrl)

            };
        }

        //public async Task<byte[]> GetReportAsync(string reportId)
        //{
        //    return await _reportingBackgroundRepository.GetReportAsync(_userData.CurrentGuid, reportId);
        //}

        public void UpdateOptions(string parameters)
        {
            var query = HttpUtility.ParseQueryString(parameters);

            if (query["criteriaID"] == null)
                throw new Exception($"LibraryPageManager :: UpdateOptions :: Could not retrieve 'criteriaID' from passed parameters: {parameters}");

            //Saved with IRM 4.0
            //EXAMPLE:  "https://qareports.riversidedatamanager.com/IRM40/ReportCenter/LoadReport_48923"
            if (!int.TryParse(query["criteriaID"], out int criteriaId))
            {
                criteriaId = int.Parse(query["criteriaID"].Split('_')[1]);
                query["criteriaID"] = criteriaId.ToString();
                parameters = query.ToString();
            }

            var invalidGroup = _criteriaManager.LoadCriteria(criteriaId, false);

            _sessionManager.Store(parameters, SessionKey.ReportLibraryParams);
        }
        public void DeletdataExporterSession()
        {
            _sessionManager.Delete(SessionKey.ReportLibraryParams);
        }
    }
}