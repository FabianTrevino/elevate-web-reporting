using DM.WR.Models.GraphqlClient.PerformanceLevelDescriptorsEndPoint;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class RosterKto1Model
    {
        [JsonProperty("graph_ql_query")]
        public string GraphQlQuery { get; set; }

        [JsonProperty("roster_type")]
        public string RosterType { get; set; }

        [JsonProperty("roster_level")]
        public string RosterLevel { get; set; }

        [JsonProperty("columns")]
        public List<RosterKto1Column> Columns { get; set; }

        [JsonProperty("values")]
        public object Values { get; set; }

        [JsonProperty("performance_level_descriptor")]
        public PerformanceLevelDescriptor PerformanceLevelDescriptor { get; set; }

        [JsonProperty("performance_level_statement")]
        public PerformanceLevelStatement PerformanceLevelStatement { get; set; }
    }
}