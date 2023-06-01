using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.DomainEndPoint
{
    public class PerformanceLevel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}