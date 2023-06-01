using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.PerformanceLevelDescriptorsEndPoint
{
    public class PerformanceLevelDescriptor
    {
        [JsonProperty("pldName")]
        public string PldName { get; set; }

        [JsonProperty("pldAltName")]
        public string PldAltName { get; set; }

        [JsonProperty("pldDesc")]
        public string PldDesc { get; set; }
    }
}
