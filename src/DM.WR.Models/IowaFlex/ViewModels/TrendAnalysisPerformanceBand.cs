using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class TrendAnalysisPerformanceBand
    {
        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("percent")]
        public int Percent { get; set; }

        [JsonProperty("range_band")]
        public string RangeBand { get; set; }

        [JsonProperty("range")]
        public int Range { get; set; }
    }
}