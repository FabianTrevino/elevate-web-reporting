using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.ServiceClient.DmServices.Models
{
    public class UserIdentityObject
    {
        public string sub { get; set; }

        [JsonProperty("custom:user_roles")]
        public string CustomUserRoles { get; set; }
        public bool email_verified { get; set; }

        [JsonProperty("custom:contact_id")]
        public object CustomContactId { get; set; }
        public string iss { get; set; }

        [JsonProperty("cognito:username")]
        public string CognitoUsername { get; set; }

        [JsonProperty("custom:district")]
        public object CustomDistrict { get; set; }

        [JsonProperty("custom:proctor_code")]
        public object CustomProctorCode { get; set; }
        public string aud { get; set; }
        public string event_id { get; set; }
        public string token_use { get; set; }
        public int auth_time { get; set; }

        [JsonProperty("custom:status")]
        public string CustomStatus { get; set; }
        public string name { get; set; }

        [JsonProperty("custom:internal_id")]
        public object CustomInternalId { get; set; }

        [JsonProperty("custom:init_pwd_created")]
        public string CustomInitPwdCreated { get; set; }

        [JsonProperty("custom:last_login")]
        public object CustomLastLogin { get; set; }
        public int exp { get; set; }

        [JsonProperty("custom:role")]
        public string CustomRole { get; set; }
        public int iat { get; set; }
        public string email { get; set; }
    }
}
