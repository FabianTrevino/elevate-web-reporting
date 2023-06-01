using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.CogAt.ViewModels
{
    public class StudentRosterExtraParams
    {
        [JsonProperty("form")]
        public string Form { get; set; }

        [JsonProperty("norm_year")]
        public string NormYear { get; set; }

        [JsonProperty("has_lpr_score")]
        public bool HasLprScore { get; set; }

        [JsonProperty("has_ls_score")]
        public bool HasLsScore { get; set; }

        //[JsonProperty("sw_lookup")]
        //public List<CogatScoreWarningItem> ScoreWarningsLookup { get; set; }

        [JsonProperty("warnings_list")]
        public List<string> WarningsList { get; set; }
    }
}