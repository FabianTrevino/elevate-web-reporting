using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM.WR.ServiceClient.DmServices.Models;
using Newtonsoft.Json;
namespace DM.WR.BL.Managers
{
    public interface IEncryptionManagerElevate
    {
        UserDetailsInformation JwtTokenDecryptionUserName(string stream);
        UserIdentityObject JwtTokenDecryptionIdentityObejct(string stream);
    }
    public class EncryptionManagerElevate : IEncryptionManagerElevate
    {
        public UserDetailsInformation JwtTokenDecryptionUserName(string stream)
        {
            var userdetails = new UserDetailsInformation();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            userdetails.username = tokenS.Claims.First(x => x.Type == "email").Value;
            userdetails.password = tokenS.Claims.First(x => x.Type == "password").Value;
            return userdetails;
        }
        public UserIdentityObject JwtTokenDecryptionIdentityObejct(string stream)
        {
            var userdetails = new UserIdentityObject();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            try
            {
                userdetails.sub = tokenS.Claims.First(x => x.Type == "sub").Value == null ? "" : tokenS.Claims.First(x => x.Type == "sub").Value;
                userdetails.CustomUserRoles = tokenS.Claims.First(x => x.Type == "custom:user_roles").Value == null ? "" : tokenS.Claims.First(x => x.Type == "custom:user_roles").Value;
                userdetails.email_verified = tokenS.Claims.First(x => x.Type == "email_verified") == null ? false : Convert.ToBoolean(tokenS.Claims.First(x => x.Type == "email_verified").Value);
                userdetails.iss = tokenS.Claims.First(x => x.Type == "iss").Value == null ? "" : tokenS.Claims.First(x => x.Type == "iss").Value;
                userdetails.email = tokenS.Claims.First(x => x.Type == "email").Value == null ? "" : tokenS.Claims.First(x => x.Type == "email").Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userdetails;
        }
    }
}
