using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.RangeEndPoint
{
    public class StandardScoreRange
    {
        [JsonProperty("plevel")]
        public int PerformanceLevel { get; set; }

        [JsonProperty("lower")]
        public int Lower { get; set; }

        [JsonProperty("upper")]
        public int Upper { get; set; }

        [JsonProperty("ranges")]
        public List<StandardScoreRange> Ranges { get; set; }
    }
}