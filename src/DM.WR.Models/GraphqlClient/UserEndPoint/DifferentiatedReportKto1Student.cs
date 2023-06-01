using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class DifferentiatedReportKto1Student
    {
        [JsonProperty("studentId")]
        public int StudentId { get; set; }

        [JsonProperty("studentName")]
        public string StudentName { get; set; }

        [JsonProperty("studentExternalId")]
        public int StudentExternalId { get; set; }

        [JsonProperty("pldLevel")]
        public int PldLevel { get; set; }

        [JsonProperty("pldStage")]
        public string PldStage { get; set; }

        [JsonProperty("pldStageNum")]
        public int PldStageNum { get; set; }

        [JsonProperty("classId")]
        public int ClassId { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }
    }
}