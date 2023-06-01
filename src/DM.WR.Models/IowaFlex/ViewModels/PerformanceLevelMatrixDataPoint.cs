using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class PerformanceLevelMatrixDataPointModel
    {
        [JsonProperty("abilityAchievement")]
        public string AbilityAchievement { get; set; }

        [JsonProperty("studCount")]
        public string StudentCount { get; set; }

    }
}