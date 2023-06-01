using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class DonutCardLevelKto1
    {
        [JsonProperty("studentCount")]
        public int StudentCount { get; set; }

        [JsonProperty("percent")]
        public double Percent { get; set; }

        [JsonProperty("PLDLevel")]
        public int PldLevel { get; set; }
    }
}