using Newtonsoft.Json;
using System.Collections.Generic;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class LongitudinalNode
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("locationName")]
        public string LocationName { get; set; }

        [JsonProperty("testScores")]
        public List<LongitudinalTestScore> TestScores { get; set; }
    }
}