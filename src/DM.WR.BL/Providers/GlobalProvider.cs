using AutoMapper;
using DM.UI.Library.Models;
using DM.WR.BL.Managers;
using DM.WR.Data.Logging;
using DM.WR.Data.Logging.Types;
using DM.WR.Models.Config;
using DM.WR.Models.Types;
using DM.WR.Models.ViewModels;
using DM.WR.ServiceClient.DmServices;
using DM.WR.ServiceClient.DmServices.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DM.WR.BL.Email;
using DM.WR.Models.Email;

namespace DM.WR.BL.Providers
{
    public interface IGlobalProvider
    {
        EntranceViewModel BuildBackDoorModel();
        Task<EntranceViewModel> ProcessBackdoorSiteEntry(EntranceViewModel model);
        SiteEntryModel ProcessDemoSiteEntry(bool isAdaptive);
        Task<SiteEntryModel> ProcessDmSiteEntry(string token, string loginSource, string contractInstance, bool isAdaptive);
        Task<SiteEntryModel> ReprocessSiteEntryForAllTestEvents(bool alwaysReprocess);
        void LogToDatabase(Exception exc);
        ReportingKeyModalModel BuildReportingKeyModal();
        MainHeaderViewModel BuildHeaderModel(string appPath);
        List<MagicMenuItem> BuildMainMenu();
        MagicFooterViewModel BuildFooterModel();
        bool CheckOptionsExistence();
        Task<ReportingKeyModalModel> AddReportingKey(string reportingKey);
        string GetRedirectUrl(string link, string appPath);
        bool IsDemo();
        Task SendFeedback(FeedbackModel message);
        Task<SiteEntryModel> ProcessDmSiteEntryElevates(string token);
        Task<SiteEntryModel> ProcessDmSiteEntryElevate(HttpCookieCollection cookies);

        Task<SiteEntryModel> ProcessReEntry(HttpCookieCollection cookies);
    }

    public class GlobalProvider : IGlobalProvider
    {
        private readonly ILoginManager _loginManager;
        private readonly ISessionManager _sessionManager;

        private readonly IWebReportingClient _webReportingClient;
        private readonly IUserApiClient _userApiClient;

        private readonly ICogatFeedbackSender _cogatFeedback;

        private readonly IDbLogger _dbLogger;

        private readonly UserData _userData;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IEncryptionManagerElevate _encryptionManagerElevate;

        public GlobalProvider(ILoginManager loginManager, IUserDataManager userDataManager, IDbLogger dbLogger, IWebReportingClient webReportingClient, IUserApiClient userApiClient, ISessionManager sessionManager, ICogatFeedbackSender cogatFeedback, IEncryptionManagerElevate encryptionManagerElevate)
        {
            _loginManager = loginManager;
            _sessionManager = sessionManager;

            _webReportingClient = webReportingClient;
            _userApiClient = userApiClient;

            _cogatFeedback = cogatFeedback;

            _dbLogger = dbLogger;
            _encryptionManagerElevate = encryptionManagerElevate;
            _userData = userDataManager.GetUserData();

        }

        public EntranceViewModel BuildBackDoorModel()
        {
            var viewList = new List<DropdownItem>
            {
                new DropdownItem{Text = "Classic", Value = WebReportingView.Classic},
                new DropdownItem{Text = "CogAT Dashboard", Value = WebReportingView.Cogat, Selected = true},
                new DropdownItem{Text = "Iowa Flex Dashboard", Value = WebReportingView.IowaFlex},
            };

            return new EntranceViewModel
            {
                LocationGuids = ConfigSettings.DefaultGuid,
                ViewList = viewList,
                UserId = ConfigSettings.BackDoorLogin.UserId,
                LocationIds = ConfigSettings.BackDoorLogin.LocationIds,
                LocationLevel = ConfigSettings.BackDoorLogin.LocationLevel
            };
        }

      
        public SiteEntryModel ProcessDemoSiteEntry(bool isAdaptive)
        {
            var userDetails = new UserDetails
            {
                IsAdaptive = isAdaptive,
                IsDemo = true,
                UserId = ConfigSettings.Demo.IowaFlex.UserId,
                LocationIds = ConfigSettings.Demo.IowaFlex.LocationIds,
                LocationLevel = ConfigSettings.Demo.IowaFlex.LocationLevel,
                LocationGuids = "IOWAFLEXDEMO"
            };

            Logger.Info("Demo Site :: Entry");

            var siteEntry = _loginManager.ProcessSiteEntry(userDetails, Constants.LoginSourceGuid, null);

            if (!string.IsNullOrEmpty(siteEntry.ErrorMessage))
                Logger.Error($"Demo Site :: Entry Failed :: {siteEntry.ErrorMessage}");

            return siteEntry;
        }

        public async Task<SiteEntryModel> ProcessDmSiteEntry(string token, string loginSource, string contractInstance, bool isAdaptive)
        {
            var userDetails = await _userApiClient.GetUserDetails(token, contractInstance, ConfigSettings.DmUrl, ConfigSettings.DmLink.UserDetailsApi, isAdaptive);
            userDetails.IsAdaptive = isAdaptive;



#if DEBUG
            var reportsItem = userDetails.MainMenu.FirstOrDefault(i => i.Key == "ReportsLink");
            if (reportsItem != null)
                foreach (var item in reportsItem.Items)
                {
                    item.Link = item.Link.Remove(0, item.Link.IndexOf("/DmEntry", StringComparison.Ordinal));
                    item.Link = $"http://localhost:50000{item.Link}";
                }
#endif
            var siteEntry = _loginManager.ProcessSiteEntry(userDetails, loginSource, contractInstance);

            if (!string.IsNullOrEmpty(siteEntry.ErrorMessage))
                Logger.Error($"DM entry processing failed :: {siteEntry.ErrorMessage}");

            return siteEntry;
        }

        public async Task<EntranceViewModel> ProcessBackdoorSiteEntry(EntranceViewModel model)
        {
            try
            {
                var role = model.RoleName;
                dynamic rosterInfo = null;
                var loginSource = "Elevate";
                var config = new MapperConfiguration(cfg => { cfg.CreateMap<EntranceViewModel, UserDetails>(); });
                var mapper = config.CreateMapper();
                var userIdPass = mapper.Map<EntranceViewModel, UserDetails>(model);
                var userDetails = await _userApiClient.GetUserDetailsElevate(userIdPass.LocationGuids, userIdPass.ContractInstances);
                var userinfo = _encryptionManagerElevate.JwtTokenDecryptionIdentityObejct(userDetails.idToken);
                rosterInfo = await _userApiClient.GetUserDetailsElevateRostering(userinfo.email,role,"");
                var siteEntry = _loginManager.ProcessSiteEntryElevate(userinfo, rosterInfo, loginSource, null, role,"");
                model.RedirectToUrl = siteEntry.RedirectToUrl;
                model.ErrorMessage = siteEntry.ErrorMessage;
            }
            catch (Exception)
            {

                throw;
            }
            return model;
        }


        public async Task<SiteEntryModel> ProcessDmSiteEntryElevates(string token)
        {
            var role = "";
            dynamic rosterInfo = null;
            var loginSource = "Elevate";
            var userInfo = _encryptionManagerElevate.JwtTokenDecryptionUserName(token);
            var userDetails = await _userApiClient.GetUserDetailsElevate(userInfo.username, userInfo.password);
            var userinfo = _encryptionManagerElevate.JwtTokenDecryptionIdentityObejct(userDetails.idToken);
            rosterInfo = await _userApiClient.GetUserDetailsElevateRostering(userinfo.email, role,"");
            var siteEntry = _loginManager.ProcessSiteEntryElevate(userinfo, rosterInfo,loginSource,null,role,token);

            if (!string.IsNullOrEmpty(siteEntry.ErrorMessage))
                Logger.Error($"DM entry processing failed :: {siteEntry.ErrorMessage}");

            return siteEntry;
        }

        public async Task<SiteEntryModel> ProcessReEntry(HttpCookieCollection cookies)
        {

            dynamic rosterInfo = null;

            #region TOCALL: To support Async
            var idTokenCookie = cookies.Get("idToken");
            var role = cookies.Get("currentStaffRole");
            var userinfo = _encryptionManagerElevate.JwtTokenDecryptionIdentityObejct(idTokenCookie.Value);
            rosterInfo = await _userApiClient.GetUserDetailsElevateRostering(userinfo.email, role.Value, idTokenCookie.Value);
            #endregion


            var siteReEntry = _loginManager.ProcessReEntry(idTokenCookie.Value);


            return siteReEntry;


        }
        public async Task<SiteEntryModel> ProcessDmSiteEntryElevate(HttpCookieCollection cookies)
        {
            try
            {
                dynamic rosterInfo = null;
                var loginSource = "Elevate";
                var idTokenCookie = cookies.Get("idToken");
                var role = cookies.Get("currentStaffRole");

                var userinfo = _encryptionManagerElevate.JwtTokenDecryptionIdentityObejct(idTokenCookie.Value);
                rosterInfo = await _userApiClient.GetUserDetailsElevateRostering(userinfo.email, role.Value, idTokenCookie.Value);
                var siteEntry = _loginManager.ProcessSiteEntryElevate(userinfo, rosterInfo, loginSource, null, role.Value, idTokenCookie.Value);

                if (!string.IsNullOrEmpty(siteEntry.ErrorMessage))
                    Logger.Error($"DM entry processing failed :: {siteEntry.ErrorMessage}");

                return siteEntry;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return new SiteEntryModel
                {
                    ErrorMessage = msg
                };
            }
        }

        public async Task<SiteEntryModel> ReprocessSiteEntryForAllTestEvents(bool alwaysReprocess)
        {
            if (_userData.IsForAllTestEvents && !alwaysReprocess)
                return new SiteEntryModel();

            var userDetails = await _userApiClient.GetUserDetails(_userData.DmUserToken, null, ConfigSettings.DmUrl, ConfigSettings.DmLink.UserDetailsApi, _userData.IsAdaptive);

            var siteEntry = _loginManager.ProcessSiteEntry(userDetails, Constants.LoginSourceMainMenu, null);

            if (!string.IsNullOrEmpty(siteEntry.ErrorMessage))
                Logger.Error($"Reprocessing DM entry failed :: Switching from a specific Test Event to all failed :: {siteEntry.ErrorMessage}");

            return siteEntry;
        }

        public void LogToDatabase(Exception exc)
        {
            var noUserData = _userData == null;

            var customerId = noUserData || _userData.IsAdaptive ? null : _userData.CurrentCustomerInfo?.CustomerId;
            var currentGuid = noUserData ? "none" : _userData.CurrentGuid;
            var contractInstances = noUserData ? "" : _userData.ContractInstances;

            //_dbLogger.LogMessageToDb(customerId, currentGuid, DbLogType.Error, BuildErrorMessage(exc, contractInstances));
        }

        public bool CheckOptionsExistence()
        {
            return _sessionManager.Retrieve(SessionKey.OptionBookKey) == null;
        }

        public async Task<ReportingKeyModalModel> AddReportingKey(string reportingKey)
        {
            var response = _webReportingClient.SaveWebKey(ConfigSettings.DmLink.BasServices, _userData.UserId, _userData.ImpersonatorId, reportingKey);

            if (response.Success)
                await ReprocessSiteEntryForAllTestEvents(false);

            return BuildReportingKeyModal(reportingKey, response.Success);
        }

        public ReportingKeyModalModel BuildReportingKeyModal()
        {
            return BuildReportingKeyModal("", true);
        }

        public string GetRedirectUrl(string link, string appPath)
        {
            var dmLink = Enum.Parse(typeof(DmLinkName), link);

            switch (dmLink)
            {
                case DmLinkName.ProductResources:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.ProductResources);
                case DmLinkName.LogOut:
                    _sessionManager.ClearAllSession();
                    return MakeDmRedirectLink(ConfigSettings.DmLink.Logout);
                case DmLinkName.Overview:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.Overview);
                case DmLinkName.TestEvents:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.TestEvents);
                case DmLinkName.Assignments:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.Assignments);
                case DmLinkName.Proctoring:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.Proctoring);
                case DmLinkName.Omr:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.Omr);
                case DmLinkName.AdminHome:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.AdminHome);
                case DmLinkName.ManageStudents:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.ManageStudents);
                case DmLinkName.ManageStaff:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.ManageStaff);
                case DmLinkName.ManageLocations:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.ManageLocations);
                case DmLinkName.ManageRostering:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.ManageRostering);
                case DmLinkName.ManageOmr:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.ManageOmrScanningSettings);
                case DmLinkName.ManageReports:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.ManageReportAccess);
                case DmLinkName.ManageTesting:
                    string encodedQs = HttpUtility.UrlEncode($"?contractID={_userData.ContractId}&pageName=AdminHome");
                    return $"{MakeDmRedirectLink(ConfigSettings.DmLink.ManageTestingActivity)}{encodedQs}";
                case DmLinkName.ViewLicenses:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.ViewLicenses);
                case DmLinkName.UserGuides:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.UserGuides);
                case DmLinkName.WebReports:
                    return $"{appPath}/Utility/{DmLinkName.WebReports}";
                case DmLinkName.SettingsPreferences:
                    return MakeDmRedirectLink(ConfigSettings.DmLink.SettingsPreferences);
                default:
                    return "#";
            }
        }

        public MainHeaderViewModel BuildHeaderModel(string appPath)
        {
            return new MainHeaderViewModel
            {
                HelpLink = ConfigSettings.DmLink.Help,
                SignOutLink = MakeNavigateLink(DmLinkName.LogOut, appPath),
                ResourcesLink = MakeNavigateLink(DmLinkName.ProductResources, appPath),
                UserName = _userData.UserDisplayName
            };
        }

        public List<MagicMenuItem> BuildMainMenu()
        {
            if (_userData == null)
                throw new Exception("Global Site Api :: Menu Data is null. Could not build Main Menu.");

            if (_userData.MainMenu == null)
            {
                return new List<MagicMenuItem>();
            }

            return _userData.MainMenu;
        }


        public MagicFooterViewModel BuildFooterModel()
        {
            if (_userData?.FooterMenu == null)
                return new MagicFooterViewModel
                {
                    PrivacyPolicyLink = new MagicMenuItem
                    {
                        Link = ConfigSettings.DmLink.PrivacyPolicy
                    },
                    TermsOfUseLink = new MagicMenuItem
                    {
                        Link = ConfigSettings.DmLink.TermsOfUse
                    }
                };

            return _userData.FooterMenu;
        }

        public bool IsDemo()
        {
            return _userData != null && _userData.IsDemo;
        }

        public async Task SendFeedback(FeedbackModel feedback)
        {
            await _cogatFeedback.SendFeedback(feedback);
        }

        private string MakeNavigateLink(DmLinkName linkName, string appPath)
        {
            return $"{appPath}/Utility/Navigate?to={linkName}";
        }

        private string MakeDmRedirectLink(string link)
        {
            return $"{_userData.DmUrlAuth}?token={HttpUtility.UrlEncode(_userData.DmUserToken)}&returnUrl={link}";
        }

        private ReportingKeyModalModel BuildReportingKeyModal(string reportingKey, bool success)
        {
            return new ReportingKeyModalModel
            {
                Key = reportingKey,
                Success = success,
                HeaderText = "Add a Reporting Key",
                Buttons = new List<Button>
                {
                    new Button(ButtonType.Secondary, ButtonSize.Medium)
                    {
                        Id = "reporting-key-close-button", Text = "Close"
                    },
                    new Button(ButtonType.Primary, ButtonSize.Medium)
                    {
                        Id = "reporting-key-add-button", Text = "Add"
                    }
                }
            };
        }

        private string TrimBackdoor(string input)
        {
            return input == null ? "" : input.Replace(" ", "").Replace("\r", "").Replace("\n", "");
        }

        private string BuildErrorMessage(Exception exc, string contractInstances)
        {
            var message = new StringBuilder("==========================================");

            message.Append("\r\n");
            message.Append("Contract Instances: ");
            message.Append(contractInstances);
            message.Append("\r\n");
            message.Append("------------------------------------------");

            message.Append("\r\n");
            message.Append("Message: ");
            message.Append(exc.Message);
            message.Append("\r\n");

            message.Append("------------------------------------------");
            message.Append("\r\n");
            message.Append("Method: ");
            message.Append(exc.TargetSite.ReflectedType == null ? "_ :: " : $"{exc.TargetSite.ReflectedType.FullName} :: ");
            message.Append(exc.TargetSite.Name);
            message.Append("\r\n");

            message.Append("------------------------------------------");
            message.Append("\r\n");
            message.Append("Exception source: ");
            message.Append(exc.Source);
            message.Append("\r\n");

            message.Append("------------------------------------------");
            message.Append("\r\n");
            message.Append("Stack Trace: ");
            message.Append(exc.StackTrace);
            message.Append("\r\n");

            message.Append("==========================================");
            message.Append("\r\n");

            return message.ToString();
        }
    }
}