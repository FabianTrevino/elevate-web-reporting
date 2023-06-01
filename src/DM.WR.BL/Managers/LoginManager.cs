using DM.WR.BL.Builders;
using DM.WR.Data.Logging;
using DM.WR.Data.Logging.Types;
using DM.WR.Data.Repository;
using DM.WR.Models.Config;
using DM.WR.Models.Types;
using DM.WR.Models.ViewModels;
using DM.WR.ServiceClient.DmServices.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DM.WR.BL.Managers
{
    public class LoginManager : ILoginManager
    {
        private readonly IUserDataManager _userDataManager;
        private readonly ISessionManager _sessionManager;

        private readonly IDbClient _dbClient;
        private readonly IDbLogger _dbLogger;

        private readonly DbTypesMapper _typesMapper;
        private readonly IEncryptionManagerElevate _encryptionManagerElevate;

        public LoginManager(IDbClient dbClient, IDbLogger dbLogger, IUserDataManager userDataManager, ISessionManager sessionManager, IEncryptionManagerElevate encryptionManagerElevate)
        {
            _userDataManager = userDataManager;
            _sessionManager = sessionManager;

            _dbClient = dbClient;
            _dbLogger = dbLogger;
            _encryptionManagerElevate = encryptionManagerElevate;

            _typesMapper = new DbTypesMapper();

        }

        public SiteEntryModel ProcessSiteEntry(UserDetails userDetails, string loginSource, string contractInstance)
        {
            _sessionManager.ClearAllSession();

            var isBackDoorEntry = loginSource == Constants.LoginSourceGuid;

            if (loginSource == Constants.LoginSourceMainMenu && string.IsNullOrEmpty(userDetails.ContractInstances) && !userDetails.IsAdaptive ||
                loginSource == Constants.LoginSourceTestEvent && string.IsNullOrEmpty(contractInstance) && !userDetails.IsAdaptive)
                return new SiteEntryModel { RedirectToUrl = ConfigSettings.DmLink.AddReportingKey };

            if (string.IsNullOrEmpty(userDetails.LocationGuids) && !isBackDoorEntry)
                return new SiteEntryModel { ErrorMessage = "Location GUID was not passed from DM." };
            var logEntry = $"Web Reporting 5.0: Attempted entry | User Id: {userDetails.UserId} | GUIDs: {userDetails.LocationGuids} | Contract Instance: {contractInstance} | User Details Contract Instances: {userDetails.ContractInstances} | Role Id: {userDetails.RoleId}";
            var guids = userDetails.LocationGuids.Split(',');
            var contractInstances = EncryptionManager.Decrypt(contractInstance) ?? userDetails.ContractInstances;
            var customerInfoList = new List<CustomerInfo>();

            if (userDetails.IsAdaptive)
            {
                var locationIds = userDetails.LocationIds.Replace(" ", "").Split(',');

                foreach (var locationId in locationIds)
                    customerInfoList.Add(new CustomerInfo { Guid = userDetails.LocationGuids, NodeType = userDetails.LocationLevel, NodeId = locationId });
            }
            else
            {
                foreach (var guid in guids)
                {
                    var dbCustomerInfo = _dbClient.GetCustomerInfo(guid, contractInstances);

                    if (dbCustomerInfo != null)
                        customerInfoList.Add(_typesMapper.Map<CustomerInfo>(dbCustomerInfo));
                }
            }

            if (!customerInfoList.Any())
                return new SiteEntryModel { ErrorMessage = $"LoginManager :: ProcessSiteEntry :: Could not retrieve any records from SDR based on data received from DM. GUIDs:{userDetails.LocationGuids} Contract Instances:{contractInstances} User:{userDetails.UserDisplayName} User ID: {userDetails.UserId}" };

            var loggingInfo = _dbLogger.GetLoggingInfo();

            var userData = new UserData
            {
                CustomerInfoList = customerInfoList,
                CurrentGuid = customerInfoList.Select(i => i.Guid).First(),
                ContractInstances = contractInstances,
                RoleId = isBackDoorEntry ? -1 : userDetails.RoleId,
                UserId = isBackDoorEntry && userDetails.IsAdaptive ? userDetails.UserId : isBackDoorEntry ? Constants.GuidUser : EncryptionManager.Decrypt(HttpUtility.HtmlDecode(userDetails.UserId)),
                IsAdaptive = userDetails.IsAdaptive,
                LoggingFlag = loggingInfo.LoggingFlag,
                ComponentLoggingString = loggingInfo.ComponentLoggingString,
                ContractId = userDetails.ContractId,
                IsNelsonCustomer = userDetails.IsNelsonCustomer,
                IsCogATCustomer = userDetails.IsCogATCustomer,
                IsOnlineTestingEnabled = userDetails.IsOnlineTestingEnabled,
                IsDemo = userDetails.IsDemo,
                UserDisplayName = isBackDoorEntry ? Constants.GuidUser : userDetails.UserDisplayName,
                ImpersonatorId = userDetails.ImpersonatorId,
                DmUrlAuth = userDetails.DmUrlAuth,
                DmUserToken = userDetails.DmUserToken,
                MainMenu = userDetails.MainMenu,
                FooterMenu = userDetails.FooterMenu
            };

            var reportsItem = userData.MainMenu?.FirstOrDefault(m => m.Text == "Reports");

            if (reportsItem != null)
            {
                reportsItem.HtmlClass = $"{reportsItem.HtmlClass} current-page";
            }

            userData.LogSessionId = isBackDoorEntry ?
                                    _dbLogger.LogSessionStart(Constants.LoginSourceGuid, userData.CurrentGuid, null) :
                                    _dbLogger.LogSessionStart(loginSource, userData.UserId, userData.RoleId);

            _userDataManager.StoreUserData(userData);

            return new SiteEntryModel();
        }

        public SiteEntryModel ProcessReEntry(string IdToken)
        {
            var userData = _userDataManager.GetUserData();
            userData.ElevateIdToken = IdToken;

            _userDataManager.StoreUserData(userData);

            return new SiteEntryModel();
        }
        public SiteEntryModel ProcessSiteEntryElevate(UserIdentityObject userIdentityObject, string rosteringRoles, string loginSource, string contractInstance, string role, string idToken)
        {
            var userData = new UserData();
            
            List<int> permissions=null;
            var elevateRole = string.Empty;
            int totalLevel = 1;
            int nodeLevel = 1;

            var elevateCustomerDetails = JsonConvert.DeserializeObject<UserDetailsRosteringRoles.Root>(userIdentityObject.CustomUserRoles);
            var elevateRostering = JsonConvert.DeserializeObject<RosterRolesElevate>(rosteringRoles);                     
            if (role == "teacher")
            {
               
                elevateRole = "Teacher";
                totalLevel = 1;
                nodeLevel = 1;
                
                permissions = elevateRostering.roles.teacher.defaultPermissionsJson.permissions;
                
            }
            else if (role == "staff")
            {
                
                elevateRole = "Staff";
                totalLevel = 1;
                nodeLevel = 2;
              
                permissions = elevateRostering.roles.staff.defaultPermissionsJson.permissions;
               
            }
            else if (role == "district_admin")
            {
               
                elevateRole = "district_admin";
                totalLevel = 2;
                nodeLevel = 3;
               
                permissions = elevateRostering.roles.district_admin.defaultPermissionsJson.permissions;
                
            }

            _sessionManager.ClearAllSession();
            var isBackDoorEntry = loginSource == Constants.LoginSourceGuid;
            var customerInfoList = new List<CustomerInfo>();
            var customerInfoItem = new CustomerInfo();

            #region Elevate Customer
            customerInfoItem.CustomerId = elevateCustomerDetails.CustomerId;
            customerInfoItem.NodeId = elevateCustomerDetails.DistrictId;
            customerInfoItem.NodeName = userIdentityObject.name;
            customerInfoItem.DmUserId = 0;
            customerInfoItem.TotalLevels = totalLevel;
            customerInfoItem.Guid = elevateCustomerDetails.UserId;
            customerInfoItem.NodeLevel = nodeLevel;
            customerInfoList.Add(customerInfoItem);
            #endregion

            #region User Data Creation
            userData.ElevateIdToken = idToken;
            userData.ParticipatedLocations = elevateRostering.roles;
            userData.CustomerInfoList = customerInfoList;
            userData.CurrentGuid = customerInfoList.Select(i => i.Guid).First();
            userData.RoleId = nodeLevel;
            userData.UserId = elevateCustomerDetails.UserId;
            userData.IsAdaptive = false;
            userData.LoggingFlag = true;
            userData.ComponentLoggingString = "0101";
            userData.ContractId = 0;
            userData.IsNelsonCustomer = false;
            userData.IsCogATCustomer = false;
            userData.IsOnlineTestingEnabled = false;
            userData.IsDemo = false;
            userData.ElevateCustomerId = elevateCustomerDetails.CustomerId;
            userData.ElevateDistrictId = elevateCustomerDetails.DistrictId;
            userData.ElevateRole = elevateRole;
            userData.ElevateUserId = elevateCustomerDetails.UserId;
            userData.UserDisplayName = isBackDoorEntry ? Constants.GuidUser : userIdentityObject.name;
            userData.ImpersonatorId = null;
            userData.DmUrlAuth = null;
            userData.DmUserToken = null;
            userData.MainMenu = null;
            userData.FooterMenu = null;
            userData.PermissionIds = permissions;
            #endregion
            var reportsItem = userData.MainMenu?.FirstOrDefault(m => m.Text == "Reports");
            if (reportsItem != null)
            {
                reportsItem.HtmlClass = $"{reportsItem.HtmlClass} current-page";
            }
            _userDataManager.StoreUserData(userData);
            
            return new SiteEntryModel();
        }
    }
}