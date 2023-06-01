using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class TestScore
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("standardScore")]
        public int StandardScore { get; set; }

        [JsonProperty("scores")]
        public List<Score> Scores { get; set; }
    }
}