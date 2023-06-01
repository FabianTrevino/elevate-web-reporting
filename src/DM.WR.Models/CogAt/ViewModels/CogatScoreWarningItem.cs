using Newtonsoft.Json;

namespace DM.WR.Models.CogAt.ViewModels
{
    public class CogatScoreWarningItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}