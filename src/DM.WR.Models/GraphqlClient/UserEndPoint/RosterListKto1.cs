using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class RosterListKto1
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("externalStudentId")]
        public string ExternalStudentId { get; set; }

        [JsonProperty("preEmerging")]
        public int? PreEmerging { get; set; }

        [JsonProperty("emerging")]
        public int? Emerging { get; set; }

        [JsonProperty("beginning")]
        public int? Beginning { get; set; }

        [JsonProperty("transitioning")]
        public int? Transitioning { get; set; }

        [JsonProperty("independent")]
        public int? Independent { get; set; }

        [JsonProperty("PLDLevel")]
        public int? PldLevel { get; set; }

        [JsonProperty("PLDStage")]
        public string PldStage { get; set; }

        [JsonProperty("PLDStageNum")]
        public int? PldStageNum { get; set; }
    }
}