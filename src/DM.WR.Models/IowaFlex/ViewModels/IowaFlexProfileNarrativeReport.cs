using System.Collections.Generic;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class IowaFlexProfileNarrativeReport
    {
        public string GraphqlQuery { get; set; }
        public string GraphqlLookUpQuery { get; set; }

        public IowaFlexProfileNarrativeReport()
        {
            DomainNarratives = new List<IowaFlexProfileNarrativeDomainModel>();
            TestEvents = new List<IowaFlexProfileNarrativeTestEvent>();
        }

        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string StudentId { get; set; }
        public string StudentExternalId { get; set; }
        public string Grade { get; set; }
        public string TestDate { get; set; }
        public string Class { get; set; }
        public string School { get; set; }
        public string District { get; set; }
        public string AssessmentName { get; set; }
        public string StandardScore { get; set; }
        public string NprScore { get; set; }
        public string SubjectName { get; set; }
        public string PerformanceLevel { get; set; }
        public List<IowaFlexProfileNarrativeDomainModel> DomainNarratives { get; set; }
        public List<IowaFlexProfileNarrativeTestEvent> TestEvents { get; set; }
    }
}