using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class CogatRosterValue
    {
        [JsonProperty("node_id")]
        public int NodeId { get; set; }

        [JsonProperty("node_name")]
        public string NodeName { get; set; }

        [JsonProperty("SS")]
        public int? Ss { get; set; }

        [JsonProperty("NPR")]
        public int? Npr { get; set; }

        [JsonProperty("v")]
        public int? Verbal { get; set; }

        [JsonProperty("q")]
        public int? Quantitative { get; set; }

        [JsonProperty("n")]
        public int? NonVerbal { get; set; }

        [JsonProperty("vq")]
        // ReSharper disable once InconsistentNaming
        public int? CompVQ { get; set; }

        [JsonProperty("vn")]
        // ReSharper disable once InconsistentNaming
        public int? CompVN { get; set; }

        [JsonProperty("qn")]
        // ReSharper disable once InconsistentNaming
        public int? CompQN { get; set; }

        [JsonProperty("vqn")]
        // ReSharper disable once InconsistentNaming
        public int? CompVQN { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }
    }
}