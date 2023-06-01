using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class PldStageKto1
    {
        [JsonProperty("PLDStage")]
        public string PldStage { get; set; }

        [JsonProperty("PLDStageNum")]
        public int PldStageNum { get; set; }

        [JsonProperty("percent")]
        public int Percent { get; set; }

        [JsonProperty("studentCount")]
        public int StudentCount { get; set; }
    }
}