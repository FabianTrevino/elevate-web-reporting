using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class ChildLocation
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public string ParentId { get; set; }

        public string ParentName { get; set; }
    }
}