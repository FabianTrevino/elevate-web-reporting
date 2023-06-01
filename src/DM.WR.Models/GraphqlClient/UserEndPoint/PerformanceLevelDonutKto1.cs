using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class PerformanceLevelDonutKto1
    {
        [JsonProperty("PLDStage")]
        public string PldStage { get; set; }

        [JsonProperty("PLDStageNum")]
        public int PldStageNum { get; set; }

        [JsonProperty("PLDLevel")]
        public int PldLevel { get; set; }

        [JsonProperty("percent")]
        public double Percent { get; set; }

        [JsonProperty("studentCount")]
        public int StudentCount { get; set; }
    }
}