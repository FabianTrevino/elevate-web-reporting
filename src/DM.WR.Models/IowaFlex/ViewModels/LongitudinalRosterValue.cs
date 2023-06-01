using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class LongitudinalRosterValue
    {
        [JsonProperty("node_id")]
        public int NodeId { get; set; }

        [JsonProperty("node_name")]
        public string NodeName { get; set; }

        [JsonProperty("SS0")]
        public int? Ss0 { get; set; }

        [JsonProperty("SS1")]
        public int? Ss1 { get; set; }

        [JsonProperty("SS2")]
        public int? Ss2 { get; set; }

        [JsonProperty("gain")]
        public int? Gain { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }
    }
}