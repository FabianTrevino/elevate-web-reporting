using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class GainsAnalysisValue
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("da")]
        public int? DistrictAverage { get; set; }

        [JsonProperty("sa")]
        public int? BuildingAverage { get; set; }

        [JsonProperty("ca")]
        public int? ClassAverage { get; set; }
    }
}