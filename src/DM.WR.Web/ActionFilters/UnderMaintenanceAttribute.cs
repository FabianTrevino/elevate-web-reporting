using System.Web.Mvc;
using System.Web.Routing;
using DM.WR.Models.Config;

namespace DM.WR.Web.ActionFilters
{
    public class UnderMaintenanceAttribute : ActionFilterAttribute
    {
        public bool Disable { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Disable || !ConfigSettings.UnderMaintenance) return;

            var routeDictionary = new RouteValueDictionary {
                    { "controller", MVC.Utility.Name },
                    { "action", MVC.Utility.ActionNames.UnderMaintenance}
                };

            filterContext.Result = new RedirectToRouteResult(routeDictionary);
        }
    }
}