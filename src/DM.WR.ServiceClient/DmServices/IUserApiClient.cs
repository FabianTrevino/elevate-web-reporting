using System.Threading.Tasks;
using System.Web;
using DM.WR.ServiceClient.DmServices.Models;

namespace DM.WR.ServiceClient.DmServices
{
    public interface IUserApiClient
    {
        Task<UserDetails> GetUserDetails(string token, string contractInstance, string dmUrl, string usersDetailUrl, bool isAdaptive);
        Task<UserDetailsElevate> GetUserDetailsElevate(string userName, string pwd);
        Task<UserDetailsElevate> GetUserDetailsElevate(HttpCookieCollection cookies);
        Task<string> GetUserDetailsElevateRostering(string userName, string role, string idToken);
    }
}