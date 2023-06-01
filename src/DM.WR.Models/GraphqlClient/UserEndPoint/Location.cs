using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class Location
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("averageScore")]
        public int AverageScore { get; set; }

        [JsonProperty("nprAverageScore")]
        public int NprAverageScore { get; set; }

        [JsonProperty("domainScores")]
        public List<DomainScore> DomainScores { get; set; }
    }
}