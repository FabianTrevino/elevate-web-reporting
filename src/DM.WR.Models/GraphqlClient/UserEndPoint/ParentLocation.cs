using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class ParentLocation
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("childLocations")]
        public List<ChildLocation> ChildLocations { get; set; }
    }
}