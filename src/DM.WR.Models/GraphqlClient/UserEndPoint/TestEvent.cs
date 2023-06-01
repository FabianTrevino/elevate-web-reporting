using System.Collections.Generic;
using Newtonsoft.Json;

namespace DM.WR.Models.GraphqlClient.UserEndPoint
{
    public class TestEvent
    {
        [JsonProperty("testEventId")]
        public int Id { get; set; }

        [JsonProperty("testEventName")]
        public string Name { get; set; }

        [JsonProperty("testEventDate")]
        public string Date { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("isDefault")]
        public object IsDefault { get; set; }

        [JsonProperty("isLongitudinal")]
        public bool IsLongitudinal { get; set; }

        [JsonProperty("isCogat")]
        public bool IsCogat { get; set; }

        [JsonProperty("grades")]
        public List<Grade> Grades { get; set; }

        [JsonProperty("parentLocations")]
        public List<ParentLocation> ParentLocations { get; set; }

        [JsonProperty("populations")]
        public List<Population> Populations { get; set; }

        [JsonProperty("studentRoster")]
        public StudentRoster StudentRoster { get; set; }

        [JsonProperty("domainScores")]
        public List<DomainScore> DomainScores { get; set; }

        [JsonProperty("locationRoster")]
        public LocationRoster LocationRoster { get; set; }

        [JsonProperty("testScore")]
        public TestScore TestScore { get; set; }

        [JsonProperty("longitudinalTestEvents")]
        public List<LongitudinalTestEvent> LongitudinalTestEvents { get; set; }

        [JsonProperty("gainAnalysis")]
        public GainAnalysis GainAnalysis { get; set; }

        [JsonProperty("students")]
        public List<LongitudinalNode> Students { get; set; }

        [JsonProperty("locations")]
        public List<LongitudinalNode> Locations { get; set; }

        [JsonProperty("rosterCard")]
        public RosterCard RosterCard { get; set; }

        [JsonProperty("k1DifferentiatedReport")]
        public List<DifferentiatedReportKto1> DifferentiatedReportKto1 { get; set; }

        [JsonProperty("PerformanceLevelMatrix")]
        public PerformanceLevelMatrix PerformanceLevelMatrix { get; set; }

        [JsonProperty("cogatRoster")]
        public CogatRoster CogatRoster { get; set; }
    }
}