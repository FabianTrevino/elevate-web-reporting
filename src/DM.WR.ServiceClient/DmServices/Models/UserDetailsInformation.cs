using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.ServiceClient.DmServices.Models
{
    public class UserDetailsInformation
    {
        public string username { get; set; }
        public string password { get; set; }
        public int iat { get; set; }
    }
}
