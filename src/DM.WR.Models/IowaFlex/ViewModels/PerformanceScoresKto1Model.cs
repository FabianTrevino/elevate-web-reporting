using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class PerformanceScoresKto1Model
    {
        [JsonProperty("graph_ql_query")]
        public string GraphQlQuery { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("is_longitudinal")]
        public bool IsLongitudinal { get; set; }

        [JsonProperty("is_cogat")]
        public bool IsCogat { get; set; }

        [JsonProperty("values")]
        public List<PerformanceScoresLevelKto1> PldValues { get; set; }
    }
}