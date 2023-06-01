using DM.WR.Models.GraphqlClient.StudentEndPoint;
using DM.WR.Models.IowaFlex;
using DM.WR.Models.IowaFlex.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace DM.WR.BL.Builders
{
    public interface IDashboardIowaFlexProviderBuilder
    {
        IowaFlexProfileNarrativeReport ToAdaptiveProfileNarrativeViewModel(Student student, string subjectName, List<IowaFlexProfileNarrativeDomainModel> domainNarratives);

        IowaFlexProfileNarrativeDomainModel ToAdaptiveProfileNarrativeDomainModel(DomainModel domainModel, int performanceLevel, string studentFirstName);
    }

    public class DashboardIowaFlexProviderBuilder : IDashboardIowaFlexProviderBuilder
    {
        // public for testing purposes only.  Not exposed on interface
        public string ToAchievementLevel(int performanceLevel)
        {
            switch (performanceLevel)
            {
                case 1:
                    return "Low";
                case 2:
                    return "Mid";
                case 3:
                    return "High";
                default:
                    return $"{performanceLevel} did not match.";
            }
        }

        public IowaFlexProfileNarrativeDomainModel ToAdaptiveProfileNarrativeDomainModel(DomainModel domainModel, int performanceLevel, string studentFirstName)
        {
            return new IowaFlexProfileNarrativeDomainModel
            {
                AchievementLevel = ToAchievementLevel(performanceLevel),
                DomainName = domainModel.Name,
                DomainNarrativeText = domainModel.Text,
                DomainPerformanceLevelText = domainModel.PerformanceText.Replace("<Student First Name>", studentFirstName)
            };
        }

        public IowaFlexProfileNarrativeReport ToAdaptiveProfileNarrativeViewModel(Student student, string subjectName, List<IowaFlexProfileNarrativeDomainModel> domainNarratives)
        {
            return new IowaFlexProfileNarrativeReport
            {
                AssessmentName = student.CurrentTestEvent.TestEventName,
                Class = student.CurrentTestEvent.District.ChildLocations.First().ChildLocations.First().Name,
                District = student.CurrentTestEvent.District.Name,
                DomainNarratives = domainNarratives,
                Grade = student.CurrentTestEvent.Grade.Name,

                NprScore = student.CurrentTestEvent.TestScore.Scores.First().Value.ToString(),
                PerformanceLevel = student.CurrentTestEvent.TestScore.Scores.First().PerformanceBands.First().Id.ToString(),
                School = student.CurrentTestEvent.District.ChildLocations.First().Name,
                StandardScore = student.CurrentTestEvent.TestScore.StandardScore.ToString(),

                StudentFirstName = student.Name.FirstName,
                StudentId = student.UserId.ToString(),
                StudentExternalId = student.ExternalId,
                StudentLastName = student.Name.LastName,
                SubjectName = subjectName,
                TestDate = student.CurrentTestEvent.TestDate.ToShortDateString()
            };
        }
    }
}