using Flurl.Http.Configuration;
using System.Net.Http;

namespace DM.WR.Web.App_Start
{
    public class UntrustedCertClientFactory : DefaultHttpClientFactory
    {
        //only use for localhost debugging!! -- ONLY 
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            };
        }
    }
}