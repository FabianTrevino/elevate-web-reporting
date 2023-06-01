using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.StudentEndPoint
{
    public class PerformanceBand
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("lower")]
        public int Lower { get; set; }

        [JsonProperty("nOfStudents")]
        public object NumberOfStudents { get; set; }

        [JsonProperty("percent")]
        public object Percent { get; set; }

        [JsonProperty("upper")]
        public int Upper { get; set; }
    }
}
