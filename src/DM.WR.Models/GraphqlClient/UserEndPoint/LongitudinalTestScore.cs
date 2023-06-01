using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class LongitudinalTestScore
    {
        [JsonProperty("testEventId")]
        public int TestEventId { get; set; }

        [JsonProperty("testEventName")]
        public string TestEventName { get; set; }

        [JsonProperty("standardScore")]
        public int StandardScore { get; set; }
    }
}