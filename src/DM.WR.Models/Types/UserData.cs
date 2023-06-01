using System.Collections.Generic;
using System.Linq;
using DM.UI.Library.Models;


namespace DM.WR.Models.Types
{
    public class UserData
    {
        public List<CustomerInfo> CustomerInfoList;
        public string CurrentGuid { get; set; }
        public CustomerInfo CurrentCustomerInfo { get { return CustomerInfoList.FirstOrDefault(i => i.Guid == CurrentGuid); } }
        public List<Assessment> Assessments { get; set; }
        public Roles ParticipatedLocations { get; set; }
        public string ElevateIdToken { get; set; }
        public string ElevateCustomerId { get; set; }
        public string ElevateDistrictId { get; set; }
        public string ElevateUserId { get; set; }
        public string ElevateRole { get; set; }

        public List<string> BuildingIds { get; set; }
        public bool IsAdaptive { get; set; }
        public string UserId { get; set; }
        public string ContractInstances { get; set; }
        public int RoleId { get; set; }
        public List<int> PermissionIds { get; set; }
        public int ContractId { get; set; }
        public bool IsNelsonCustomer { get; set; }
        public bool IsCogATCustomer { get; set; }
        public bool IsOnlineTestingEnabled { get; set; }
        public bool IsDemo { get; set; }
        public string ActuateUserId => $"{UserId}_{CurrentGuid}";
        public bool IsForAllTestEvents { get; set; }

        public bool LoggingFlag { get; set; }
        public string ComponentLoggingString { get; set; }
        public int LogSessionId { get; set; }

        public string DmUrlAuth { get; set; }
        public string DmUserToken { get; set; }
        public string ImpersonatorId { get; set; }
        public string UserDisplayName { get; set; }


        public bool HasTestEvents { get; set; }
        public bool HasProctoring { get; set; }
        public bool HasAdminHome { get; set; }
        public bool HasAdminManageStudents { get; set; }
        public bool HasAdminManageStaff { get; set; }
        public bool HasAdminManageLocations { get; set; }
        public bool HasAdminManageRostering { get; set; }
        public bool HasAdminManageOmr { get; set; }
        public bool HasAdminManageReportsAccess { get; set; }
        public bool HasAdminManageTestingActivity { get; set; }
        public bool HasDirectionsForAdministration { get; set; }
        public bool HasAdminViewLicenses { get; set; }
        public bool HasUserGuides { get; set; }
        public bool HasProductResources { get; set; }


        public bool HasAssignments { get; set; }
        public bool HasOmr { get; set; }

        public List<MagicMenuItem> MainMenu { get; set; }
        public MagicFooterViewModel FooterMenu { get; set; }
    }
}