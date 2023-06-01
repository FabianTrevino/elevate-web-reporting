using DM.WR.BL.Providers;
using System.Web.Mvc;

namespace DM.WR.Web.ActionFilters
{
    public class BuildMainMenuAttribute : FilterAttribute, IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var providerFunctions = new CommonProviderFunctions();

            filterContext.Controller.ViewBag.MagicMenuModel = providerFunctions.GetMainMenu();
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}