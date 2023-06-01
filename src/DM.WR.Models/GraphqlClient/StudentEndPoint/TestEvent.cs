using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.StudentEndPoint
{
    public class TestEvent
    {
        [JsonProperty("testEventId")]
        public int TestEventId { get; set; }

        [JsonProperty("testEventName")]
        public string TestEventName { get; set; }

        [JsonProperty("testDate")]
        public string TestDate { get; set; }

        [JsonProperty("grade")]
        public Grade Grade { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("testScore")]
        public TestScore TestScore { get; set; }
    }
}