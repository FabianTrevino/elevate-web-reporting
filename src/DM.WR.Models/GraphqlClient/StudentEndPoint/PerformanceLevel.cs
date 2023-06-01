using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.StudentEndPoint
{
    public class PerformanceLevel
    {
        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nOfStudents")]
        public object NumberOfStudents { get; set; }

        [JsonProperty("percent")]
        public object Percent { get; set; }
    }
}
