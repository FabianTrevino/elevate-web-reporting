using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.ServiceClient.DmServices.Models
{
    public class UserDetailsRosteringRoles
    {
        public class DefaultPermissionsJson
        {
            public List<int> Permissions { get; set; }
        }

        public class Role
        {
            public object Schools { get; set; }
            public string ProctorCode { get; set; }
            public DefaultPermissionsJson DefaultPermissionsJson { get; set; }
        }

        public class Roles
        {
            public Role Teacher { get; set; }
            public Role district_admin { get; set; }
            public Role Student { get; set; }
            public Role Staff { get; set; }
        }

        public class Root
        {
            public string UserId { get; set; }
            public string CustomerId { get; set; }
            public string DistrictId { get; set; }
            public string DistrictName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public Roles Roles { get; set; }
            public string DOB { get; set; }
            public string DobFormat { get; set; }
            public object StateId { get; set; }
            public bool IsRespondus { get; set; }
        }
    }
}
