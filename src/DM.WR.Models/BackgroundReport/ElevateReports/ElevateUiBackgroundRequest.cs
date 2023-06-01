using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.WR.Models.BackgroundReport.ElevateReports
{
  public class QueryParameters
    {
        public List<string> DistrictIds { get; set; }
        public List<string> BuildingIds { get; set; }
        public List<string> ClassIds { get; set; }
        public List<string> StudentIds { get; set; }
        public List<int> GradeLevels { get; set; }
        public List<int> ContentIds { get; set; }
        public List<string> Scores { get; set; }
        public string ReportCriteria { get; set; }
        public bool includeAbilityProfile { get; set; }
        public  string homeReporting { get; set; }
    }

    public class ElevateUiBackgroundRequest
    {
        public string FileName { get; set; }
        public string ExportTemplate { get; set; }
        public string ExportFormat { get; set; }
        public string ReportTemplate { get; set; }
        public string SuppressProgramLabel { get; set; }
        public string CogatComposite { get; set; }
        public string RankingDirection { get; set; }
        public string GraphType { get; set; }
        public string RankingSubtest { get; set; }
        public string RankingScore { get; set; }
        public string BuildingLabel { get; set; }
        public string ClassLabel { get; set; }
        public string DistrictLabel { get; set; }
        public string ReportGrouping { get; set; }
        public QueryParameters QueryParameters { get; set; }
    }
}
