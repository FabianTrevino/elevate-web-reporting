using DM.WR.Web.App_Start;
using Flurl.Http;
using System.Web.Http;

namespace DM.WR.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
#if DEBUG
            FlurlHttp.ConfigureClient("https://localhost:5001", cli =>
                cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
#endif

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
