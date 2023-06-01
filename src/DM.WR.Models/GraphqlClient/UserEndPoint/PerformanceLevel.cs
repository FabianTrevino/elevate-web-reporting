using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class PerformanceLevel
    {
        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nOfStudents")]
        public int NumberOfStudents { get; set; }

        [JsonProperty("percent")]
        public int Percent { get; set; }

        //[JsonProperty("score")]
        //public object Score { get; set; }
    }
}