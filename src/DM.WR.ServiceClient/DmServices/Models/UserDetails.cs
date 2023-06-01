using System.Collections.Generic;
using DM.UI.Library.Models;

namespace DM.WR.ServiceClient.DmServices.Models
{
    public class UserDetails
    {
        public string UserId { get; set; }
        public string LocationGuids { get; set; }
        public string ContractInstances { get; set; }
        public string LocationIds { get; set; } //adaptive
        public string LocationLevel { get; set; } //adaptive
        public int RoleId { get; set; }
        public int ContractId { get; set; }
        public bool IsAdaptive { get; set; }
        public bool IsNelsonCustomer { get; set; }
        public bool IsCogATCustomer { get; set; }
        public bool IsOnlineTestingEnabled { get; set; }
        public bool IsDemo { get; set; }
        public string UserDisplayName { get; set; }
        public string ImpersonatorId { get; set; }
        public string DmUrlAuth { get; set; }
        public string DmUserToken { get; set; }


        public bool TestEvents { get; set; }
        public bool Proctoring { get; set; }
        public bool Scanning { get; set; }
        public bool Administration { get; set; }
        public bool ManageStudents { get; set; }
        public bool ManageStaff { get; set; }
        public bool ManageLocations { get; set; }
        public bool ManageRostering { get; set; }
        public bool ManageOMR { get; set; }
        public bool ManageReports { get; set; }
        public bool ManageTesting { get; set; }
        public bool DirectionsForAdministration { get; set; }
        public bool ViewLicenses { get; set; }
        public bool UserGuides { get; set; }
        public bool ProductResources { get; set; }

        public List<MagicMenuItem> MainMenu { get; set; }
        public MagicFooterViewModel FooterMenu { get; set; }
    }
}
