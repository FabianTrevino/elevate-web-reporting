using System.Collections;
using System.Collections.Generic;
using DM.WR.Models.GraphqlClient.PerformanceLevelDescriptorsEndPoint;

namespace DM.WR.Models.IowaFlex.ViewModels
{
    public class ProfileNarrativeKto1ViewModel : IEnumerable
    {
        public string GraphqlQuery { get; set; }

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
        public string SubjectName { get; set; }
        public string PldName { get; set; }
        public int? PldLevel { get; set; }

        public PerformanceLevelDescriptor PerformanceLevelDescriptor { get; set; }
        public PerformanceLevelStatement PerformanceLevelStatement { get; set; }
        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}