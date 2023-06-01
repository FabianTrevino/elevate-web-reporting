using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class PerformanceBand
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("lower")]
        public int Lower { get; set; }

        [JsonProperty("upper")]
        public int Upper { get; set; }

        [JsonProperty("nOfStudents")]
        public int NumberOfStudents { get; set; }

        [JsonProperty("percent")]
        public int Percent { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("npr")]
        public int? Npr { get; set; }

        [JsonProperty("standardScore")]
        public int? StandardScore { get; set; }
    }
}