using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class PerformanceLevelMatrixDataPoint
    {
        [JsonProperty("abilityAchievement")]
        public string AbilityAchievement { get; set; }

        [JsonProperty("studCount")]
        public string StudCount { get; set; }
    }
}