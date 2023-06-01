using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.ServiceClient.DmServices.Models
{
    public class UserDetailsElevate
    {
        public string idToken { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public DateTime? issuedTime { get; set; }
        public DateTime? expirationTime { get; set; }
    }

}
