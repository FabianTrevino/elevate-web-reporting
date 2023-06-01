using DM.WR.Models.Config;
using DM.WR.ServiceClient.DmServices.Models;
using Flurl.Http;
using Newtonsoft.Json;
using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DM.WR.ServiceClient.DmServices
{
    public class UserApiClient : IUserApiClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<UserDetails> GetUserDetails(string token, string contractInstance, string dmUrl, string usersDetailUrl, bool isAdaptive)
        {
            var parameters = new { Source = "WebReport5.0", Token = token, IsAdaptive = isAdaptive };
            var url = $"{dmUrl}{usersDetailUrl}";

            try
            {
                var retJson = url.PostJsonAsync(parameters).ReceiveJson();
                return await url.PostJsonAsync(parameters).ReceiveJson<UserDetails>();
            }
            catch (FlurlHttpTimeoutException)
            {
                Logger.Error(FlurlHttpTimeoutMessage(url, parameters));
                throw;
            }
            catch (FlurlHttpException ex)
            {
                Logger.Error(FlurlHttpExceptionMessage(url, parameters, ex.Message, ex.Call.HttpStatus));
                throw;
            }
        }

        public async Task<UserDetailsElevate> GetUserDetailsElevate(string userName, string pwd)
        {
            var parameters = new { username = userName, password = pwd};
            var url = ConfigSettings.ElevateApiUrl+ConfigSettings.AuthUrl; //Example:"https://qa.elevate.riverside-insights.com/identity-api/api/Authentication";
            try
            {
                var retJson = url.PostJsonAsync(parameters).ReceiveJson();
                return await url.PostJsonAsync(parameters).ReceiveJson<UserDetailsElevate>();
            }
            catch (FlurlHttpTimeoutException)
            {
                Logger.Error(FlurlHttpTimeoutMessage(url, parameters));
                throw;
            }
            catch (FlurlHttpException ex)
            {
                Logger.Error(FlurlHttpExceptionMessage(url, parameters, ex.Message, ex.Call.HttpStatus));
                throw;
            }
        }

        public async Task<UserDetailsElevate> GetUserDetailsElevate(HttpCookieCollection cookies)
        {
            var url = ConfigSettings.ElevateBaseUrl+ ConfigSettings.ElevateValidation;
            UserDetailsElevate userDetailsElevate = new UserDetailsElevate();
            try
            {
                ////var idTokenCookie = cookies.Get("idToken");
                //var accessTokenCookie = cookies.Get("accessToken");
                //var refreshTokenCookie = cookies.Get("refreshToken");

                var idTokenCookie = cookies.Get("idToken");

                //var idTokenWithHttpUtility = HttpUtility.UrlEncode(idTokenCookie.Value);
                //var accessTokenCookieWithHttpUtility = HttpUtility.UrlEncode(accessTokenCookie.Value);
                //var refreshTokenCookieWithHttpUtility = HttpUtility.UrlEncode(refreshTokenCookie.Value);

                //if (idTokenCookie == null || accessTokenCookie == null || refreshTokenCookie == null)
                //    return null;

                //if (accessTokenCookie == null || refreshTokenCookie == null)
                //    return null;


                var idTokenWithHttpUtility = HttpUtility.UrlEncode(idTokenCookie.Value);
                if (idTokenCookie == null)
                {
                    return null;
                }

                #region Receive Json Code
                //return await url.WithCookie("idToken", idTokenWithHttpUtility)
                //    .WithCookie("accessToken", accessTokenCookieWithHttpUtility)
                //    .WithCookie("refreshToken", refreshTokenCookieWithHtttpUtility)
                //    .PostJsonAsync().ReceiveJson<UserDetailsElevate>();
                #endregion

                //var data = await url.WithCookie("idToken", idTokenWithHttpUtility)
                //    .WithCookie("accessToken", accessTokenCookieWithHttpUtility)
                //    .WithCookie("refreshToken", refreshTokenCookieWithHtttpUtility)
                //    .AllowAnyHttpStatus()
                //    .PostJsonAsync(null);

                //var data = await url.WithCookie("accessToken", accessTokenCookieWithHttpUtility)
                //  .WithCookie("refreshToken", refreshTokenCookieWithHttpUtility)
                //  .AllowAnyHttpStatus()
                //  .PostJsonAsync(null);

                //var data = await url.WithCookie("idToken", idTokenWithHttpUtility).WithCookie("accessToken", accessTokenCookieWithHttpUtility)
                // .WithCookie("refreshToken", refreshTokenCookieWithHttpUtility)
                // .AllowAnyHttpStatus()
                // .PostJsonAsync(null);


                var data = await url.WithCookie("idToken", idTokenWithHttpUtility)
                .AllowAnyHttpStatus()
                .PostJsonAsync(null);

                var content = data.Content.ReadAsStringAsync().Result;
                //userDetailsElevate = JsonConvert.DeserializeObject<UserDetailsElevate>(content);
                return userDetailsElevate;
            }
            catch (FlurlHttpTimeoutException)
            {
                Logger.Error(FlurlHttpTimeoutMessage(url, cookies));
                throw;
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Message.Contains("status code 401"))
                    return null;

                Logger.Error(FlurlHttpExceptionMessage(url, cookies, ex.Message, ex.Call.HttpStatus));
                throw;
            }
        }

        public async Task<string> GetUserDetailsElevateRostering(string userName, string role, string idToken)
        {
            var url = ConfigSettings.ElevateApiUrl + ConfigSettings.RosterUrl + $"?emailId={HttpUtility.UrlEncode(userName)}&getSchools=true&getTeacherSections=true";

            try
            {
                //var rosteringRoles = url.WithHeader("Authorization", idToken).GetStringAsync().Result;
                var checkpoint = "";



                //var rosteringRoles = url.WithHeaders(new { accept = "*/*", Authorization = idToken }).GetAsync();
                //var headers = rosteringRoles.Result.Headers;
                //string content = rosteringRoles.Result.Content.ReadAsStringAsync().Result;

                var content = await GetRosteringString(url, idToken);

                //await url.WithHeaders(new { Accept = "text/plain", User_Agent = "Flurl" }).GetJsonAsync();
                return content;
            }
            catch (FlurlHttpTimeoutException)
            {
                Logger.Error(FlurlHttpTimeoutMessage(url, userName));
                throw;
            }
            catch (FlurlHttpException ex)
            {
                return ex.Call.HttpStatus.ToString();
            }
        }

        private string FlurlHttpTimeoutMessage(string endpoint, object parameters)
        {
            return $"User/Details API Client :: Timeout :: Endpoint: {endpoint} :: Parameters: {JsonConvert.SerializeObject(parameters, Formatting.Indented)}";
        }

        private string FlurlHttpExceptionMessage(string endpoint, object parameters, string message, HttpStatusCode? httpStatus)
        {
            return $"User/Details API Client :: Exception :: Endpoint: {endpoint} :: Parameters: {JsonConvert.SerializeObject(parameters, Formatting.Indented)} :: Message: {message} :: HTTP Status: {httpStatus}";
        }
        private async Task<string> GetRosteringString( string url, string idToken)
        {
            var contentString = "";
            using (var client = new HttpClient())
            {
               
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", idToken);
                //GET Method
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    contentString = response.Content.ReadAsStringAsync().Result;
                    return contentString;
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                    return contentString;
                }
            }
            
        }
    }
}