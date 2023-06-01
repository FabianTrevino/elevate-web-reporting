using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class RosterCard
    {
        [JsonProperty("performanceScoreGraph")]
        public PerformanceScoreGraphKto1 PerformanceScoreGraph { get; set; }

        [JsonProperty("performanceLevelDonuts")]
        public List<PerformanceLevelDonutKto1> PerformanceLevelDonuts { get; set; }

        [JsonProperty("roster")]
        public LocationRosterKto1 Roster { get; set; }
    }
}