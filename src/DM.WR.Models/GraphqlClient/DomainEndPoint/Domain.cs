using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.DomainEndPoint
{
    public class Domain
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("shortName")]
        public string ShortName { get; set; }
    }
}