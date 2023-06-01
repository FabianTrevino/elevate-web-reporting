using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.StudentEndPoint
{
    public class CurrentTestEvent
    {
        [JsonProperty("district")]
        public District District { get; set; }

        [JsonProperty("domainScores")]
        public List<DomainScore> DomainScores { get; set; }

        [JsonProperty("grade")]
        public Grade Grade { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("subjectFullName")]
        public string SubjectName { get; set; }

        [JsonProperty("pldName")]
        public string PldName { get; set; }

        [JsonProperty("pldLevel")]
        public int? PldLevel { get; set; }

        [JsonProperty("testDate")]
        public DateTime TestDate { get; set; }

        [JsonProperty("testEventId")]
        public int TestEventId { get; set; }

        [JsonProperty("testEventName")]
        public string TestEventName { get; set; }

        [JsonProperty("testScore")]
        public TestScore TestScore { get; set; }
    }
}