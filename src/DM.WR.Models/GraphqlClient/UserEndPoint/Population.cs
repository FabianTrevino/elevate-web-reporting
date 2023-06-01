using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class Population
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("values")]
        public List<string> Values { get; set; }
    }
}