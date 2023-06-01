using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.Models.Types
{
    public class Section
    {
        public string id { get; set; }
        public string name { get; set; }
    }
    public class School
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<Section> sections { get; set; }

    }

    public class DefaultPermissionsJson
    {
        public List<int> permissions { get; set; }
    }

    public class DistrictAdmin
    {
        public int roleId { get; set; }
        public string roleName { get; set; }
        public List<School> schools { get; set; }
        public string proctorCode { get; set; }
        public DefaultPermissionsJson defaultPermissionsJson { get; set; }
        public string cleverRoleName { get; set; }
    }
    public class Teacher
    {
        public int roleId { get; set; }
        public string roleName { get; set; }
        public List<School> schools { get; set; }
        public string proctorCode { get; set; }
        public DefaultPermissionsJson defaultPermissionsJson { get; set; }
        public string cleverRoleName { get; set; }
    }
    public class Staff
    {
        public int roleId { get; set; }
        public string roleName { get; set; }
        public List<School> schools { get; set; }
        public string proctorCode { get; set; }
        public DefaultPermissionsJson defaultPermissionsJson { get; set; }
        public string cleverRoleName { get; set; }
    }
    public class Roles
    {
        public DistrictAdmin district_admin { get; set; }
        public Teacher teacher { get; set; }
        public Staff staff { get; set; }
    }

    public class RosterRolesElevate
    {
        public string userId { get; set; }
        public string userExternalId { get; set; }
        public string customerId { get; set; }
        public string districtId { get; set; }
        public string districtExternalId { get; set; }
        public string districtName { get; set; }
        public string email { get; set; }
        public Roles roles { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public bool isRespondus { get; set; }
    }
}
