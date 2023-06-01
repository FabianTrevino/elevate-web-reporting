using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class Grade
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}