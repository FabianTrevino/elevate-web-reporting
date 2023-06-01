using Newtonsoft.Json;
using System.Collections.Generic;

namespace DM.WR.Models.GraphqlClient.DomainEndPoint
{
    public class SubjectGradeDomains
    {

        [JsonProperty("subject")]
        public Subject Subject { get; set; }

        [JsonProperty("domains")]
        public List<Domain> Domains { get; set; }

        [JsonProperty("performanceLevels")]
        public List<PerformanceLevel> PerformanceLevels { get; set; }
    }
}