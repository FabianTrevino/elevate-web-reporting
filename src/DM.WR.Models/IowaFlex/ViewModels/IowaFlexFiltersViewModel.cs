using System.Collections.Generic;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class IowaFlexFiltersViewModel
    {
        public List<Filter> Filters { get; set; }
        public List<BreadCrumb> LocationsBreadCrumbs { get; set; }
        public string GraphqlQuery { get; set; }
        public string RootLocationLevel { get; set; }
        public bool HasDifferentiatedKto1Report { get; set; }
        public bool IsKto1 { get; set; }
    }
}