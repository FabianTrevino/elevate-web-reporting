using Newtonsoft.Json;

namespace DM.WR.Models.CogAt.ViewModels
{
    public class ContentAreaNationalPercent
    {
        [JsonProperty("stanine1")]
        public int Stanine1 { get; set; }
        [JsonProperty("stanine2")]
        public int Stanine2 { get; set; }
        [JsonProperty("stanine3")]
        public int Stanine3 { get; set; }
        [JsonProperty("stanine4")]
        public int Stanine4 { get; set; }
        [JsonProperty("stanine5")]
        public int Stanine5 { get; set; }
        [JsonProperty("stanine6")]
        public int Stanine6 { get; set; }
        [JsonProperty("stanine7")]
        public int Stanine7 { get; set; }
        [JsonProperty("stanine8")]
        public int Stanine8 { get; set; }
        [JsonProperty("stanine9")]
        public int Stanine9 { get; set; }
    }
}