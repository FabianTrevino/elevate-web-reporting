using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class LongitudinalGain
    {
        [JsonProperty("testEventId")]
        public int TestEventId { get; set; }

        [JsonProperty("testEventName")]
        public string TestEventName { get; set; }

        [JsonProperty("districtAvgSS")]
        public int? DistrictAvgSs { get; set; }

        [JsonProperty("schoolAvgSS")]
        public int? SchoolAvgSs { get; set; }

        [JsonProperty("classAvgSS")]
        public int? ClassAvgSs { get; set; }
    }
}