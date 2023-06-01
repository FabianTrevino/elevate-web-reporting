using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.StudentEndPoint
{
    public class Grade
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
