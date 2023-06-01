using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class Band
    {
        [JsonProperty("range")]
        public int Range { get; set; }

        [JsonProperty("range_band")]
        public string RangeBand { get; set; }
    }
}