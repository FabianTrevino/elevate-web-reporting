using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class DomainScore
    {
        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        //[JsonProperty("score")]
        //public object Score { get; set; }

        [JsonProperty("performanceLevels")]
        public List<PerformanceLevel> PerformanceLevels { get; set; }
    }
}