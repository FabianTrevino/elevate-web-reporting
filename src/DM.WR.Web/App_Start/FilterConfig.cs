using DM.WR.Web.ActionFilters;
using System.Web.Mvc;

namespace DM.WR.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            GlobalFilters.Filters.Add(new UnderMaintenanceAttribute());
        }
    }
}
