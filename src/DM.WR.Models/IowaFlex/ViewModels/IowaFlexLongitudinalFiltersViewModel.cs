using System.Collections.Generic;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class IowaFlexLongitudinalFiltersViewModel
    {
        public List<Filter> Filters { get; set; }
        public string GraphqlQuery { get; set; }
        public List<BreadCrumb> LocationsBreadCrumbs { get; set; }
    }
}