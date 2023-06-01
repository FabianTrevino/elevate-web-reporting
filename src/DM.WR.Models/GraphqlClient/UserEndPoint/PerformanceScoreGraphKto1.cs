using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class PerformanceScoreGraphKto1
    {
        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("PLDStages")]
        public List<PldStageKto1> PldStages { get; set; }
    }
}