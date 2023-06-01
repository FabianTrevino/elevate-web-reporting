using DM.WR.BL.Providers;
using System.Web.Mvc;
using System.Web.Routing;

namespace DM.WR.Web.ActionFilters
{
    public class RestrictAdaptiveAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var providerFunctions = new CommonProviderFunctions();

            if (!providerFunctions.IsAdaptive()) return;

            var routeDictionary = new RouteValueDictionary {
                    { "controller", MVC.DashboardIowaFlex.Name },
                    { "action", MVC.DashboardIowaFlex.ActionNames.Index }
                };

            filterContext.Result = new RedirectToRouteResult(routeDictionary);
        }
    }
}