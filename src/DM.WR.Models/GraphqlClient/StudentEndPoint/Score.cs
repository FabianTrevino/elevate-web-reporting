using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.StudentEndPoint
{
    public class Score
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("performanceBands")]
        public List<PerformanceBand> PerformanceBands { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }
}
