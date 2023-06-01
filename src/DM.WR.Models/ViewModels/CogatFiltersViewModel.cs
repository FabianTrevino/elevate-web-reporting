using DM.WR.Models.CogAt;
using System.Collections.Generic;

namespace DM.WR.Models.ViewModels
{
    public class CogatFiltersViewModel
    {
        public List<Filter> Filters { get; set; }
        public string RootNodeName { get; set; }
        public string RootNodeType { get; set; }
        public string ErrorMessage { get; set; }
        public string Battery { get; set; }
        public string TestFamilyGroupCode { get; set; }
    }
}