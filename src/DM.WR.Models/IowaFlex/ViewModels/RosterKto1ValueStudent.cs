using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class RosterKto1ValueStudent
    {
        [JsonProperty("node_id")]
        public int Id { get; set; }

        [JsonProperty("node_name")]
        public string Name { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("node_type")]
        public string Level => "student";

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("PLDS0")]
        public string PldStage { get; set; }

        [JsonProperty("PLDL0")]
        public int? PldLevel { get; set; }

        [JsonProperty("pld_stage_num")]
        public int? PldStageNum { get; set; }
    }
}