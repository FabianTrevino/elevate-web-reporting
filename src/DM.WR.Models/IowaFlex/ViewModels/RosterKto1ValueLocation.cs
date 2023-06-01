using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class RosterKto1ValueLocation
    {
        [JsonProperty("node_id")]
        public int Id { get; set; }

        [JsonProperty("node_name")]
        public string Name { get; set; }

        [JsonProperty("node_type")]
        public string Level { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("PE0")]
        public int? PreEmerging { get; set; }

        [JsonProperty("E0")]
        public int? Emerging { get; set; }

        [JsonProperty("B0")]
        public int? Beginning { get; set; }

        [JsonProperty("T0")]
        public int? Transitioning { get; set; }

        [JsonProperty("I0")]
        public int? Independent { get; set; }
    }
}