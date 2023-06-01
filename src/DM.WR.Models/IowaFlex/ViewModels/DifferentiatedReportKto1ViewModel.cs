using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class DifferentiatedReportKto1ViewModel
    {
        [JsonProperty("graph_ql_query")]
        public string GraphQlQuery { get; set; }
        [JsonProperty("values")]
        public DifferentiatedReportKto1ReportValues Values { get; set; }
    }

    public class DifferentiatedReportKto1ReportValues
    {
        public string Grade { get; set; }
        public string TestEventName { get; set; }
        public string TestEventDate { get; set; }
        public int? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string Subject { get; set; }
        public List<DifferentiatedReportKto1PldBuilding> Buildings { get; set; }
    }

    public class DifferentiatedReportKto1PldBuilding
    {
        public int? BuildingId { get; set; }
        public string BuildingName { get; set; }
        public List<DifferentiatedReportKto1PldStage> PldStages { get; set; }
    }

    public class DifferentiatedReportKto1PldStage
    {
        public string PldStageName { get; set; }
        public int PldStageNum { get; set; }
        public string PldStageDescriptorText { get; set; }
        public List<DifferentiatedReportKto1PldLevel> PldLevels { get; set; }
    }

    public class DifferentiatedReportKto1PldLevel
    {
        public int PldLevelNum { get; set; }
        public string PldLevelName { get; set; }
        public string CanStatement { get; set; }
        public string NeedPracticeStatement { get; set; }
        public string ReadyStatement { get; set; }
        public string CanDescriptor { get; set; }
        public string NeedPracticeDescriptor { get; set; }
        public string ReadyDescriptor { get; set; }
        public List<DifferentiatedReportKto1PldClass> Classes { get; set; }
    }

    public class DifferentiatedReportKto1PldClass
    {
        public string ClassName { get; set; }
        public string ClassId { get; set; }
        public List<string> StudentNames { get; set; }
    }
}