using DM.WR.BL.Builders;
using DM.WR.Models.Options;
using DM.WR.Models.Types;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DM.WR.BL.Tests.Builders
{
    public class GenerateReportRequestBuilderTests
    {
        private readonly Mock<IOptionPageParser> _mockOptionPageParser;
        private readonly Mock<ISMIModelBuilder> _mockSMIModelBuilder;

        private readonly GenerateReportRequestBuilder _sut;

        public GenerateReportRequestBuilderTests()
        {
            _mockOptionPageParser = new Mock<IOptionPageParser>();
            _mockSMIModelBuilder = new Mock<ISMIModelBuilder>();

            _sut = new GenerateReportRequestBuilder(_mockOptionPageParser.Object, _mockSMIModelBuilder.Object);
        }

        //[Fact]
        //public void GetProfileNarrativeGenerateReportRequest_ShouldHandleDefaults()
        //{
        //    const string reportFormat = "reportFormat";
        //    const string reportName = "reportName-asdflkj;lkj";

        //    var userData = new UserData
        //    {
        //        CustomerInfoList = new List<CustomerInfo>(),
        //        CurrentGuid = "aaaaa"
        //    };
        //    var optionPage = new OptionPage("xmlPath");
        //    _mockOptionPageParser.Setup(x => x.GetReportFormat(optionPage)).Returns(reportFormat);

        //   // var actual = _sut.GetReport(userData, optionPage, reportName);

        //    Assert.NotNull(actual.JsonNeedToGenerateReport);
        //    Assert.Contains(reportFormat, actual.JsonNeedToGenerateReport);
        //    Assert.Equal(reportName, actual.Name);
        //    Assert.Equal("ProfileNarrative", actual.ReportType);
        //    Assert.Equal(userData.CurrentGuid, actual.UserId);
    }
}

