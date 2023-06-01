//Dmitriy - keeping this file as a quick example of how to make configurable feature attributes

//using DM.WR.Models.Config;
//using System.Web.Mvc;
//using System.Web.Routing;

//namespace DM.WR.Web.ActionFilters
//{
//    public class CogatDashboardFeatureEnabledAttribute : ActionFilterAttribute
//    {
//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            var isFeatureEnabled = ConfigSettings.IsCogatDashboardFeatureEnabled;

//            if (isFeatureEnabled) return;

//            var routeDictionary = new RouteValueDictionary {
//                    { "controller", MVC.Options.Name },
//                    { "action", MVC.Options.ActionNames.Index }
//                };

//            filterContext.Result = new RedirectToRouteResult(routeDictionary);
//        }
//    }
//}