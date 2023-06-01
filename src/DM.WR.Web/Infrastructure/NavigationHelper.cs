using DM.UI.Library.Models;
using DM.WR.Web.Models;
using System.Collections.Generic;
using System.Web;

namespace DM.WR.Web.Infrastructure
{
    public static class NavigationHelper
    {
        public static List<TabItem> Tabs(TabsNavigationEnum selectedTab)
        {
            var appPath = HttpContext.Current.Request.ApplicationPath == "/" ? "" : HttpContext.Current.Request.ApplicationPath;

            return new List<TabItem>
            {
                new TabItem{ Text = "Create a Report", Link = $"{appPath}/Options" , IsSelected = selectedTab == TabsNavigationEnum.Options },
                new TabItem{ Text = "Saved Criteria", Link = $"{appPath}/Criteria" , IsSelected = selectedTab == TabsNavigationEnum.Criteria },
                new TabItem{ Text = "Report Library", Link = $"{appPath}/Library" , IsSelected = selectedTab == TabsNavigationEnum.Library },
            };
        }
    }
}