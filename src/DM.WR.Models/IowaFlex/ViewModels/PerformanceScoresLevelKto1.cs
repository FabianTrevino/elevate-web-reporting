using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class PerformanceScoresLevelKto1
    {
        [JsonProperty("PLDStage")]
        public string PldStage { get; set; }

        [JsonProperty("PLDStageNum")]
        public int PldStageNum { get; set; }

        [JsonProperty("studentCount")]
        public int StudentCount { get; set; }

        [JsonProperty("percent")]
        public double Percent { get; set; }
    }
}
