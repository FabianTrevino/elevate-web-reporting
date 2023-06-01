using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class LongitudinalTestEvent
    {
        [JsonProperty("testEventId")]
        public int TestEventId { get; set; }

        [JsonProperty("testEventName")]
        public string TestEventName { get; set; }

        [JsonProperty("testEventDate")]
        public DateTime TestEventDate { get; set; }

        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("performanceBands")]
        public List<PerformanceBand> PerformanceBands { get; set; }
    }
}