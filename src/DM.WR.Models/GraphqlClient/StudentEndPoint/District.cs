using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.StudentEndPoint
{
    public class District
    {
        [JsonProperty("childLocations")]
        public List<ChildLocation> ChildLocations { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
