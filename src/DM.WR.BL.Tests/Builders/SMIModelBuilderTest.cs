using DM.WR.BL.Builders;
using DM.WR.Data.Repository;
using DM.WR.Data.Repository.Types;
using DM.WR.Models.Options;
using DM.WR.Models.SMI;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DM.WR.BL.Tests.Builders
{
    public class SMIModelBuilderTest
    {
        private readonly Mock<IDbClient> _mockDbClient;

        private readonly SMIModelBuilder _sut;

        public SMIModelBuilderTest()
        {
            _mockDbClient = new Mock<IDbClient>();

            _sut = new SMIModelBuilder(_mockDbClient.Object);
        }

        [Fact]
        public void BuildSMIBaseParameters_ShouldBuildTrueConditionPaths()
        {
            var customerInfo = new CustomerInfo();
            var scoringOptions = new ScoringOptions
            {
                AccountabilityFlag = 1,
                LprNodeList = "LprNodeList",
                LprNodeLevel = "LprNodeLevel"
            };

            var mockOptionsPage = new Mock<IOptionPage>();

            mockOptionsPage.SetupGet(x => x.ScoringOptions).Returns(scoringOptions);
            mockOptionsPage.SetupGet(x => x.ScoreSetId).Returns(9);
            var grades = Models.Xml.XMLGroupType.GradePaper.ToString();

            mockOptionsPage.Setup(x => x.GetSelectedValuesStringOf(Models.Xml.XMLGroupType.GradePaper)).Returns(grades);
            mockOptionsPage.SetupGet(x => x.ExcludeEla).Returns(0);
            mockOptionsPage.SetupGet(x => x.ExcludeMathComputation).Returns(0);
            mockOptionsPage.Setup(x => x.GetSelectedValuesStringOf(Models.Xml.XMLGroupType.HomeReporting)).Returns("SPANISH");

            mockOptionsPage.Setup(x => x.GetLastSelectedNodeIds(customerInfo)).Returns(new List<int> { 1, 2, 3 });

            const string lastLselectedNodeType = "lastLselectedNodeType";
            mockOptionsPage.Setup(x => x.GetLastSelectedNodeType(customerInfo)).Returns(lastLselectedNodeType);

            var actual = _sut.BuildSMIBaseParameters(customerInfo, mockOptionsPage.Object);

            Assert.True(actual.Accountability);
            Assert.Equal("9", actual.CustomerScoresetIDs);
            Assert.Equal(grades, actual.Grades);
            Assert.Equal(ExcludeType.Include, actual.ExcludeMathComputation);
            Assert.Equal(ExcludeType.Include, actual.ExcludeWordAnalysisListening);
            Assert.Equal(3, actual.Language);

            Assert.Equal("1,2,3", actual.ReportPopulationNodeIDs);
            Assert.Equal(lastLselectedNodeType, actual.ReportPopulationNodeType);

            Assert.Equal(scoringOptions.LprNodeList, actual.TestPopulationNodeIDs);
            Assert.Equal(scoringOptions.LprNodeLevel, actual.TestPopulationNodeType);
        }

        [Fact]
        public void BuildSMIBaseParameters_ShouldBuildFalseConditionPaths()
        {
            var customerInfo = new CustomerInfo();
            var mockOptionsPage = new Mock<IOptionPage>();

            mockOptionsPage.SetupGet(x => x.ScoringOptions).Returns(new ScoringOptions { AccountabilityFlag = 1 });
            mockOptionsPage.SetupGet(x => x.ScoreSetId).Returns(7);
            mockOptionsPage.SetupGet(x => x.ExcludeEla).Returns(1);
            mockOptionsPage.SetupGet(x => x.ExcludeMathComputation).Returns(1);
            mockOptionsPage.Setup(x => x.GetSelectedValuesStringOf(Models.Xml.XMLGroupType.HomeReporting)).Returns("A value not matching 'SPANISH'");
            mockOptionsPage.Setup(x => x.GetLastSelectedNodeIds(customerInfo)).Returns(new List<int>());

            var actual = _sut.BuildSMIBaseParameters(customerInfo, mockOptionsPage.Object);

            Assert.Equal(ExcludeType.Exclude, actual.ExcludeMathComputation);
            Assert.Equal(ExcludeType.Exclude, actual.ExcludeWordAnalysisListening);
            Assert.Equal(3, actual.Language);
        }

        [Fact]
        public void BuildSMIFilteringParameters_ShouldMapAndReturn()
        {
            var mockOptionsPage = new Mock<IOptionPage>();
            var scoringOptions = new ScoringOptions { GroupsetCode = "aaa" };
            mockOptionsPage.SetupGet(x => x.ScoringOptions).Returns(scoringOptions);

            var actual = _sut.BuildSMIFilteringParameters(mockOptionsPage.Object);

            Assert.Equal("AAA", actual.GroupSetCode);
        }

        [Fact]
        public void BuildSMISkillParameters_ShouldNotSetSkillSetIdForNonSkillDomainClassification()
        {
            var mockOptionsPage = new Mock<IOptionPage>();
            mockOptionsPage.Setup(x => x.GroupExists(It.IsAny<XMLGroupType>())).Returns(false);

            var actual = _sut.BuildSMISkillParameters(null, mockOptionsPage.Object);

            Assert.Equal(string.Empty, actual.SkillsetIDs);
        }

        [Fact]
        public void BuildSMISkillParameters_ShouldSetSkillIdFromDBCall()
        {
            const int adminValue = 99;
            const string customerId = "1111";
            const string reportCode = "aaaaa";
            var customerInfo = new CustomerInfo { CustomerId = customerId };

            var mockOptionsPage = new Mock<IOptionPage>();
            mockOptionsPage.Setup(x => x.GroupExists(It.IsAny<XMLGroupType>())).Returns(true);
            mockOptionsPage.SetupGet(x => x.XmlDataFilteringOptions).Returns(new XMLDataFilteringOptions { Skillset = new XMLReportOptionGroup_Skillset { getValuesOnSubmit = true } });
            mockOptionsPage.SetupGet(x => x.TestAdminValue).Returns(adminValue);
            mockOptionsPage.SetupGet(x => x.XmlDisplayOption).Returns(new XMLDisplayOption { reportCode = reportCode });

            var skillSetIds = new List<SkillSet>
            {
                new SkillSet { Id = "xxxx" },
                new SkillSet { Id = "yyyy" }
            };

            _mockDbClient.Setup(x => x.GetSkillSets(customerId.ToString(), adminValue, null, reportCode)).Returns(skillSetIds);

            mockOptionsPage.SetupGet(x => x.ScoringOptions).Returns(new ScoringOptions { SkillSetId = 1 });

            var actual = _sut.BuildSMISkillParameters(customerInfo, mockOptionsPage.Object);

            Assert.Equal("xxxx,yyyy", actual.SkillsetIDs);
        }

        [Fact]
        public void BuildSMISkillParameters_ShouldSetSkillIdForHiddenGroup()
        {
            var mockOptionsPage = new Mock<IOptionPage>();
            mockOptionsPage.Setup(x => x.GroupExists(It.IsAny<XMLGroupType>())).Returns(true);
            mockOptionsPage.SetupGet(x => x.XmlDataFilteringOptions).Returns(new XMLDataFilteringOptions { Skillset = new XMLReportOptionGroup_Skillset { getValuesOnSubmit = false } });
            mockOptionsPage.Setup(x => x.IsGroupHidden(XMLGroupType.SkillDomainClassification)).Returns(true);

            var expected = "1";
            mockOptionsPage.SetupGet(x => x.ScoringOptions).Returns(new ScoringOptions { SkillSetId = 1 });

            var actual = _sut.BuildSMISkillParameters(null, mockOptionsPage.Object);

            Assert.Equal(expected, actual.SkillsetIDs);
        }

        [Fact]
        public void BuildSMISkillParameters_ShouldGetSelectedValue()
        {
            var mockOptionsPage = new Mock<IOptionPage>();
            mockOptionsPage.Setup(x => x.GroupExists(It.IsAny<XMLGroupType>())).Returns(true);
            mockOptionsPage.SetupGet(x => x.XmlDataFilteringOptions).Returns(new XMLDataFilteringOptions { Skillset = new XMLReportOptionGroup_Skillset { getValuesOnSubmit = false } });
            mockOptionsPage.Setup(x => x.IsGroupHidden(XMLGroupType.SkillDomainClassification)).Returns(false);

            var expected = "theCodeThisTestsIsABBOM";
            mockOptionsPage.Setup(x => x.GetSelectedValuesStringOf(XMLGroupType.SkillDomainClassification)).Returns(expected);

            var actual = _sut.BuildSMISkillParameters(null, mockOptionsPage.Object);

            Assert.Equal(expected, actual.SkillsetIDs);
        }

        [Fact]
        public void BuildSMISubtestParameters_ShouldHandleNoScoresSelectedSubtestScoresList()
        {
            var mockOptionsPage = new Mock<IOptionPage>();
            mockOptionsPage.Setup(x => x.GetSelectedValuesOf(Models.Xml.XMLGroupType.Scores)).Returns((List<string>)null);

            var actual = _sut.BuildSMISubtestParameters(mockOptionsPage.Object);

            Assert.True(string.IsNullOrEmpty(actual.SubtestScoresList));
        }

        [Fact]
        public void BuildSMISubtestParameters_ShouldHandleSPandGPScoresSelectedSubtestScoresList()
        {
            var mockOptionsPage = new Mock<IOptionPage>();
            var scoreIds = new List<string> { "1", "2" };
            mockOptionsPage.Setup(x => x.GetSelectedValuesOf(Models.Xml.XMLGroupType.Scores)).Returns(scoreIds);

            var hiddenScores = new List<string> { "3h", "4h" };
            mockOptionsPage.Setup(x => x.GetHiddenValuesOf(Models.Xml.XMLGroupType.Scores)).Returns(hiddenScores);

            mockOptionsPage.SetupGet(x => x.XmlDisplayType).Returns(Models.Xml.XMLReportType.SP);
            var actual = _sut.BuildSMISubtestParameters(mockOptionsPage.Object);

            Assert.Equal("1/2/3h/4h", actual.SubtestScoresList);
        }

        [Fact]
        public void BuildSMISubtestParameters_ShouldHandleNonSPandGPScoresSelectedSubtestScoresList()
        {
            var mockOptionsPage = new Mock<IOptionPage>();
            var scoreIds = new List<string> { "1", "2" };
            mockOptionsPage.Setup(x => x.GetSelectedValuesOf(Models.Xml.XMLGroupType.Scores)).Returns(scoreIds);

            var hiddenScores = new List<string> { "3h", "4h" };
            mockOptionsPage.Setup(x => x.GetHiddenValuesOf(Models.Xml.XMLGroupType.Scores)).Returns(hiddenScores);

            mockOptionsPage.SetupGet(x => x.XmlDisplayType).Returns(Models.Xml.XMLReportType.CIRR);
            var actual = _sut.BuildSMISubtestParameters(mockOptionsPage.Object);

            Assert.Equal("1,2,3h,4h", actual.SubtestScoresList);
        }

        [Fact]
        public void BuildSMISubtestParameters_ShouldHandleSubtestFamiliesToSuppress()
        {
            var mockOptionsPage = new Mock<IOptionPage>();
            mockOptionsPage.Setup(x => x.GetSelectedValuesOf(Models.Xml.XMLGroupType.Scores)).Returns((List<string>)null);

            mockOptionsPage.Setup(x => x.GroupExists(Models.Xml.XMLGroupType.ShowReadingTotal)).Returns(true);
            mockOptionsPage.Setup(x => x.GetSelectedValuesStringOf(Models.Xml.XMLGroupType.ShowReadingTotal)).Returns("Some string with RDGTOTL in it"); ;

            var actual = _sut.BuildSMISubtestParameters(mockOptionsPage.Object);

            Assert.Equal("'RDGTOTL'", actual.SubtestFamiliesToSuppress);
        }
    }
}
