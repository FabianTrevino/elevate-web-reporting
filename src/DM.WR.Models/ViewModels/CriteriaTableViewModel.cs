using System.Collections.Generic;
using DM.UI.Library.Models;
using DM.WR.Models.Types;

namespace DM.WR.Models.ViewModels
{
    public class CriteriaTableViewModel
    {
        public CriteriaTableViewModel()
        {
            AssessmentsDropdownItems = new List<DropdownItem>();
            ReportTypeDropdownItems = new List<DropdownItem>();
            CriteriaList = new List<CriteriaInfo>();
        }

        public List<DropdownItem> AssessmentsDropdownItems { get; set; }

        public List<DropdownItem> ReportTypeDropdownItems { get; set; }

        public List<CriteriaInfo> CriteriaList { get; set; }
    }
}