using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class DifferentiatedReportKto1
    {
        [JsonProperty("districtId")]
        public int? DistrictId { get; set; }

        [JsonProperty("districtName")]
        public string DistrictName { get; set; }

        [JsonProperty("buildingId")]
        public int? BuildingId { get; set; }

        [JsonProperty("buildingName")]
        public string BuildingName { get; set; }

        [JsonProperty("classId")]
        public int? ClassId { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("studentList")]
        public List<DifferentiatedReportKto1Student> StudentList { get; set; }
    }
}