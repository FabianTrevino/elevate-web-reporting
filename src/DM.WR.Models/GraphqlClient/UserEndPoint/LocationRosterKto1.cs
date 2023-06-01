using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class LocationRosterKto1
    {
        [JsonProperty("PLDStage")]
        public string PldStage { get; set; }

        [JsonProperty("PLDLevel")]
        public string PldLevel { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("rosterList")]
        public List<RosterListKto1> RosterList { get; set; }
    }
}