using System.Collections.Generic;
using DM.WR.Models.GraphqlClient.RangeEndPoint;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class User
    {
        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("standardScoreRange")]
        public StandardScoreRange StandardScoreRange { get; set; }

        [JsonProperty("testEvents")]
        public List<TestEvent> TestEvents { get; set; }
    }
}