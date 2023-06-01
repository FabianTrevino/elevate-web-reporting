using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.StudentEndPoint
{
    public class TestScore
    {
        [JsonProperty("id")]
        public object Id { get; set; }

        [JsonProperty("scores")]
        public List<Score> Scores { get; set; }

        [JsonProperty("standardScore")]
        public int StandardScore { get; set; }
    }
}
