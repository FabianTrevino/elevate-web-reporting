using DM.WR.BL.Builders;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using DM.WR.Models.Xml;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DM.WR.BL.Tests.Builders
{
    public class OptionPageParserTests
    {
        private OptionPageParser _sut;

        public OptionPageParserTests()
        {
           // _sut = new OptionPageParser();
        }

        [Fact]
        public void GetACTScoreGrade_ShouldReturnDefaultOfZero()
        {
            var optionPage = new OptionPage("xmlPath")
            {
                Assessment = new Assessment { TestFamilyGroupCode = XMLProductCodeEnum.CCAT.ToString() }
            };

            var actual = _sut.GetACTScoreGrade(optionPage);

            Assert.Equal(0, actual);
        }

        [Fact]
        public void GetACTScoreGrade_ShouldReturnValueOfCollegeReadiness()
        {
            var expected = 5333;
            var mockIOptions = new Mock<IOptionPage>();
            mockIOptions.Setup(x => x.GroupExists(XMLGroupType.CollegeReadiness)).Returns(true);
            mockIOptions.Setup(x => x.GetSelectedValuesOf(XMLGroupType.CollegeReadiness)).Returns(new List<string> { expected.ToString() });

            var actual = _sut.GetACTScoreGrade(mockIOptions.Object);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetACTScoreGrade_ShouldReturnSuppressedValueOfZero_WhenAssessmentCodeIsCTBSAndReportTypeIsSPP()
        {
            var mockIOptions = new Mock<IOptionPage>();
            mockIOptions.SetupGet(x => x.AssessmentCode).Returns(XMLProductCodeEnum.CTBS);
            mockIOptions.SetupGet(x => x.ReportXml).Returns(new XMLReport { reportType = XMLReportType.SPP });
            mockIOptions.Setup(x => x.GroupExists(XMLGroupType.CollegeReadiness)).Returns(true);

            var actual = _sut.GetACTScoreGrade(mockIOptions.Object);

            Assert.Equal(0, actual);
        }

        [Fact]
        public void GetACTScoreGrade_ShouldReturnSuppressedValueOfZero_WhenAssessmentCodeIsCTBSAndReportTypeIsGPP()
        {
            var mockIOptions = new Mock<IOptionPage>();
            mockIOptions.SetupGet(x => x.AssessmentCode).Returns(XMLProductCodeEnum.CTBS);
            mockIOptions.SetupGet(x => x.ReportXml).Returns(new XMLReport { reportType = XMLReportType.GPP });
            mockIOptions.Setup(x => x.GroupExists(XMLGroupType.CollegeReadiness)).Returns(true);

            var actual = _sut.GetACTScoreGrade(mockIOptions.Object);

            Assert.Equal(0, actual);
        }

        [Fact]
        public void GetCollegeReadyScoreGrade_ShouldReturnDefaultOf6()
        {
            var optionPage = new OptionPage("xmlPath")
            {
                Assessment = new Assessment { TestFamilyGroupCode = XMLProductCodeEnum.CCAT.ToString() }
            };

            var actual = _sut.GetCollegeReadyScoreGrade(optionPage);

            Assert.Equal(6, actual);
        }

        [Fact]
        public void GetCollegeReadyScoreGrade_ShouldReturnValueOfCollegeReadiness()
        {
            var expected = 5333;
            var mockIOptions = new Mock<IOptionPage>();
            mockIOptions.Setup(x => x.GroupExists(XMLGroupType.CollegeReadiness)).Returns(true);
            mockIOptions.Setup(x => x.GetSelectedValuesOf(XMLGroupType.CollegeReadiness)).Returns(new List<string> { expected.ToString() });

            var actual = _sut.GetCollegeReadyScoreGrade(mockIOptions.Object);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetReportFormat_ShouldSetReportFormatToCOGATForCCAT()
        {
            var optionPage = new OptionPage("xmlPath")
            {
                Assessment = new Assessment { TestFamilyGroupCode = XMLProductCodeEnum.CCAT.ToString() }
            };

            var actual = _sut.GetReportFormat(optionPage);

            Assert.Contains("COGAT", actual);
        }

        [Fact]
        public void GetProfileNarrativeGenerateReportRequest_ShouldSetReportFormatToIOWAForCTBS()
        {
            var optionPage = new OptionPage("xmlPath")
            {
                Assessment = new Assessment { TestFamilyGroupCode = XMLProductCodeEnum.CTBS.ToString() }
            };

            var actual = _sut.GetReportFormat(optionPage);

            Assert.Contains("IOWA", actual);
        }

        [Fact]
        public void GetProfileNarrativeGenerateReportRequest_ShouldSetReportFormatToIOWAForIsIss()
        {
            var issMathOptionPage = new OptionPage("xmlPath")
            {
                Assessment = new Assessment { TestFamilyGroupCode = XMLProductCodeEnum.ISSMATH.ToString() }
            };

            var actual = _sut.GetReportFormat(issMathOptionPage);
            Assert.Contains("IOWA", actual);

            var issReadOptionPage = new OptionPage("xmlPath")
            {
                Assessment = new Assessment { TestFamilyGroupCode = XMLProductCodeEnum.ISSREAD.ToString() }
            };
            actual = _sut.GetReportFormat(issReadOptionPage);
            Assert.Contains("IOWA", actual);

            var isssciOptionPage = new OptionPage("xmlPath")
            {
                Assessment = new Assessment { TestFamilyGroupCode = XMLProductCodeEnum.ISSSCI.ToString() }
            };
            actual = _sut.GetReportFormat(isssciOptionPage);
            Assert.Contains("IOWA", actual);
        }
    }
}
