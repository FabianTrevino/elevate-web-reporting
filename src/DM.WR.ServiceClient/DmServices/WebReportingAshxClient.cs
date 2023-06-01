using System.Net;
using System.Text;
using DM.WR.ServiceClient.DmServices.Models;
using Newtonsoft.Json;

namespace DM.WR.ServiceClient.DmServices
{
    public class WebReportingAshxClient : IWebReportingClient
    {
        private const string SaveUserKeyAction = "/WebReporting.ashx?action=saveUserKey";

        public SaveWebKeyResponse SaveWebKey(string dmBasServicesUrl, string dmUserId, string dmImpersonatorId, string reportingKey)
        {
            using (WebClient client = new WebClient())
            {
                var requestObject = new SaveWebKeyRequest
                {
                    userID = dmUserId,
                    impersonatorID = dmImpersonatorId,
                    key = reportingKey
                };

                var requestParams = new System.Collections.Specialized.NameValueCollection
                {
                    {"jsonData", JsonConvert.SerializeObject(requestObject)}
                };
                var url = $"{dmBasServicesUrl}{SaveUserKeyAction}";

                byte[] responseBytes = client.UploadValues(url, "POST", requestParams);

                return JsonConvert.DeserializeObject<SaveWebKeyResponse>(Encoding.UTF8.GetString(responseBytes));
            }
        }
    }
}