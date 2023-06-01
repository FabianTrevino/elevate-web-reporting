using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class LongitudinalRosterModel
    {
        [JsonProperty("roster_type")]
        public string RosterType { get; set; }

        [JsonProperty("roster_level")]
        public string RosterLevel { get; set; }

        [JsonProperty("bands")]
        public List<Band> Bands { get; set; }

        [JsonProperty("columns")]
        public List<LongitudinalRosterColumn> Columns { get; set; }

        [JsonProperty("values")]
        public List<LongitudinalRosterValue> Values { get; set; }

        [JsonProperty("graph_ql_query")]
        public string GraphQlQuery { get; set; }
    }
}