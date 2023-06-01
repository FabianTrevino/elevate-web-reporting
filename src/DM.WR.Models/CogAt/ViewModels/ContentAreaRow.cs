using Newtonsoft.Json;

namespace DM.WR.Models.CogAt.ViewModels
{
    public class ContentAreaRow
    {
        [JsonProperty("content_area")]
        public string ContentArea { get; set; }
        [JsonProperty("stanine1_per")]
        public int Stanine1Per { get; set; }
        [JsonProperty("stanine1_num")]
        public int Stanine1Num { get; set; }
        [JsonProperty("stanine2_per")]
        public int Stanine2Per { get; set; }
        [JsonProperty("stanine2_num")]
        public int Stanine2Num { get; set; }
        [JsonProperty("stanine3_per")]
        public int Stanine3Per { get; set; }
        [JsonProperty("stanine3_num")]
        public int Stanine3Num { get; set; }
        [JsonProperty("stanine4_per")]
        public int Stanine4Per { get; set; }
        [JsonProperty("stanine4_num")]
        public int Stanine4Num { get; set; }
        [JsonProperty("stanine5_per")]
        public int Stanine5Per { get; set; }
        [JsonProperty("stanine5_num")]
        public int Stanine5Num { get; set; }
        [JsonProperty("stanine6_per")]
        public int Stanine6Per { get; set; }
        [JsonProperty("stanine6_num")]
        public int Stanine6Num { get; set; }
        [JsonProperty("stanine7_per")]
        public int Stanine7Per { get; set; }
        [JsonProperty("stanine7_num")]
        public int Stanine7Num { get; set; }
        [JsonProperty("stanine8_per")]
        public int Stanine8Per { get; set; }
        [JsonProperty("stanine8_num")]
        public int Stanine8Num { get; set; }
        [JsonProperty("stanine9_per")]
        public int Stanine9Per { get; set; }
        [JsonProperty("stanine9_num")]
        public int Stanine9Num { get; set; }
    }
}