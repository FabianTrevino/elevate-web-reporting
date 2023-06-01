using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class DonutCardKto1
    {
        [JsonProperty("PLDStage")]
        public string PldStage { get; set; }

        [JsonProperty("PLDStageNum")]
        public string PldStageNum { get; set; }

        [JsonProperty("values")]
        public List<DonutCardLevelKto1> CardLevels { get; set; }
    }
}