using DM.WR.BL.Builders;
using DM.WR.Models.GraphqlClient.StudentEndPoint;
using DM.WR.Models.IowaFlex;
using DM.WR.Models.IowaFlex.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DM.WR.BL.Tests.Builders
{
    public class DashboardIowaFlexProviderBuilderTests
    {
        private readonly DashboardIowaFlexProviderBuilder _sut;

        public DashboardIowaFlexProviderBuilderTests()
        {
            _sut = new DashboardIowaFlexProviderBuilder();
        }

        [Fact]
        public void ToAchievementLevel_Tests()
        {
            var testCases = new[]
            {
                new { PerformanceLevel = 1, Expected = "Low"},
                new { PerformanceLevel = 2, Expected = "Mid"},
                new { PerformanceLevel = 3, Expected = "High"},
                new { PerformanceLevel = 25, Expected = "25 did not match."},
            };

            foreach (var testCase in testCases)
            {
                var actual = _sut.ToAchievementLevel(testCase.PerformanceLevel);
                Assert.Equal(testCase.Expected, actual);
            }
        }

        [Fact]
        public void ToAdaptiveProfileNarrativeDomainModel_ShouldMapAndReturn()
        {
            var domainModel = new DomainModel
            {
                Name = "AAAA",
                Text = "BBBB",
                PerformanceText = "1234 <Student First Name> 5678"
            };
            var actual = _sut.ToAdaptiveProfileNarrativeDomainModel(domainModel, -25, "John");

            Assert.NotNull(actual.AchievementLevel);
            Assert.Equal(domainModel.Name, actual.DomainName);
            Assert.Equal(domainModel.Text, actual.DomainNarrativeText);
            Assert.Equal("1234 John 5678", actual.DomainPerformanceLevelText);
        }

        [Fact]
        public void ToAdaptiveProfileNarrativeViewModel_ShouldMapAndReturn()
        {
            var now = DateTime.Now;
            var student = new Student
            {
                CurrentTestEvent = new CurrentTestEvent
                {
                    District = new District
                    {
                        ChildLocations = new List<ChildLocation>
                        {
                            new ChildLocation
                            {
                                ChildLocations = new List<ChildLocation2>()
                                {
                                  new ChildLocation2 { Name = "childlocation2 name"}
                                },
                                Name = "childlocation name"
                            }
                        },
                        Name = "district 95"
                    },
                    Grade = new Grade { Name = "some grade number" },
                    TestDate = now,
                    TestEventName = "AAAAAA",
                    TestScore = new TestScore
                    {
                        Scores = new List<Score>
                        {
                            new Score
                            {
                                PerformanceBands = new List<PerformanceBand>
                                {
                                    new PerformanceBand
                                    {
                                        Id = 8765309
                                    }
                                },
                                Value = 123,
                            }
                        }
                    }
                },
                UserId = 12345,
                Name = new Name { FirstName = "John", LastName = "Galt" },
            };

            const string subjectName = "asdfasfd";
            var domainNarratives = new List<IowaFlexProfileNarrativeDomainModel>();
            var actual = _sut.ToAdaptiveProfileNarrativeViewModel(student, subjectName, domainNarratives);

            Assert.Equal(student.CurrentTestEvent.TestEventName, actual.AssessmentName);
            Assert.Equal(student.CurrentTestEvent.District.ChildLocations.First().ChildLocations.First().Name, actual.Class);
            Assert.Equal(student.CurrentTestEvent.District.Name, actual.District);
            Assert.Equal(domainNarratives, actual.DomainNarratives);
            Assert.Equal(student.CurrentTestEvent.Grade.Name, actual.Grade);

            Assert.Equal(student.CurrentTestEvent.TestScore.Scores.First().Value.ToString(), actual.NprScore);
            Assert.Equal(student.CurrentTestEvent.TestScore.Scores.First().PerformanceBands.First().Id.ToString(), actual.PerformanceLevel);
            Assert.Equal(student.CurrentTestEvent.District.ChildLocations.First().Name, actual.School);
            Assert.Equal(student.CurrentTestEvent.TestScore.StandardScore.ToString(), actual.StandardScore);

            Assert.Equal(student.Name.FirstName, actual.StudentFirstName);
            Assert.Equal(student.UserId.ToString(), actual.StudentId);
            Assert.Equal(student.Name.LastName, actual.StudentLastName);
            Assert.Equal(subjectName, actual.SubjectName);
            Assert.Equal(now.ToShortDateString(), actual.TestDate);
        }
    }
}
