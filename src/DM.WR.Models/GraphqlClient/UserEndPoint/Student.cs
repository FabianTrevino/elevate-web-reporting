using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class Student
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("testScore")]
        public int TestScore { get; set; }

        [JsonProperty("npr")]
        public int Npr { get; set; }

        [JsonProperty("domainScores")]
        public List<DomainScore> DomainScores { get; set; }
    }
}