using System.Web.Mvc;
using System.Web.Routing;

namespace DM.WR.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "dashboard_iowaflex",
                url: "dashboard/iowaflex",
                defaults: new { controller = MVC.DashboardIowaFlex.Name, action = MVC.DashboardIowaFlex.ActionNames.Index }
            );

            routes.MapRoute(
                name: "dashboard_iowaflex_print",
                url: "dashboard/iowaflex/print/{reportType}",
                defaults: new { controller = MVC.DashboardIowaFlex.Name, action = MVC.DashboardIowaFlex.ActionNames.Print, reportType = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "dashboard_iowaflex_longitudinal",
                url: "dashboard/iowaflex/longitudinal",
                defaults: new { controller = MVC.IowaFlexLongitudinal.Name, action = MVC.IowaFlexLongitudinal.ActionNames.Index }
            );

            routes.MapRoute(
                name: "dashboard_cogat",
                url: "dashboard/cogat",
                defaults: new { controller = MVC.DashboardCogat.Name, action = MVC.DashboardCogat.ActionNames.Index }
            );

            routes.MapRoute(
                name: "dashboard_cogat_print",
                url: "dashboard/cogat/print/{reportType}",
                defaults: new { controller = MVC.DashboardCogat.Name, action = MVC.DashboardCogat.ActionNames.Print, reportType = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = MVC.BackDoor.Name, action = MVC.BackDoor.ActionNames.Index, id = UrlParameter.Optional }
            );
        }
    }
}