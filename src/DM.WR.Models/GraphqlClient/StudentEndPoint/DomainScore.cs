using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.StudentEndPoint
{
    public class DomainScore
    {
        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("performanceLevels")]
        public List<PerformanceLevel> PerformanceLevels { get; set; }
    }
}
