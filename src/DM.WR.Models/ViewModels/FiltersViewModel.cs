using DM.WR.Models.Dashboard;
using System.Collections.Generic;

namespace DM.WR.Models.ViewModels
{
    public class FiltersViewModel
    {
        public List<Filter> Filters { get; set; }
        public List<BreadCrumb> LocationsBreadCrumbs { get; set; }
        public string GraphqlQuery { get; set; }
    }
}