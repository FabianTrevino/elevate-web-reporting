using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.PerformanceLevelDescriptorsEndPoint
{
    public class PerformanceLevelStatement
    {
        [JsonProperty("pldLvlName")]
        public string PldLevelName { get; set; }

        [JsonProperty("canStmt")]
        public string CanStatement { get; set; }

        [JsonProperty("readyStmt")]
        public string ReadyStatement { get; set; }

        [JsonProperty("practiceStmt")]
        public string PracticeStatement { get; set; }

        [JsonProperty("iCanDesc")]
        public string CanDescription { get; set; }

        [JsonProperty("needDesc")]
        public string NeedDescription { get; set; }

        [JsonProperty("readyDesc")]
        public string ReadyDescription { get; set; }
    }
}