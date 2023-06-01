using DM.WR.Models.BackgroundReport;
using System.Collections.Generic;

namespace DM.WR.Models.ViewModels
{
    public class LibraryPageViewModel
    {
        
        public LibraryPageViewModel()
        {
            Reports = new List<ReportMeta>();
        }

        public string ActuateGenerateUrl { get; set; }

        public string ActuateWebLocation { get; set; }

        public string UserID { get; set; }

        public string Password { get; set; }

        public bool IsGuidUser { get; set; }

        public string IsTelerikReportFeatureEnabled { get; set; }

        public string QueryString { get; set; }
        public string appPath { get; set; }
        public IEnumerable<ReportMeta> Reports { get; set; }
    }
}