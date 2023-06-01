using System.Web;
using DM.WR.BL.Builders;
using DM.WR.BL.Email;
using DM.WR.BL.Managers;
using DM.WR.BL.Providers;
using DM.WR.Data.Logging;
using DM.WR.GraphQlClient;
using DM.WR.Models.Types;
using DM.WR.ServiceClient.DmServices;
using Moq;
using Xunit;

namespace DM.WR.BL.Tests.Providers
{
    public class GlobalProviderTests
    {
        private Mock<ILoginManager> _mockLoginManager;
        private Mock<ISessionManager> _mockSessionManager;

        private Mock<IWebReportingClient> _mockWebReportingClient;
        private Mock<IUserApiClient> _mockUserApiClient;
        private Mock<ICogatFeedbackSender> _mockCogatFeedbackSender;

        private Mock<IDbLogger> _mockDbLogger;

        private UserData _userData;

        private Mock<CommonProviderFunctions> _mockCommonProviderFunctions;
        private Mock<IEncryptionManagerElevate> _mockEncryptionManagerElevate;
        private GlobalProvider _sut;

        public GlobalProviderTests()
        {
            _mockLoginManager = new Mock<ILoginManager>();
            _mockSessionManager = new Mock<ISessionManager>();
            _mockWebReportingClient = new Mock<IWebReportingClient>();
            _mockUserApiClient = new Mock<IUserApiClient>();
            _mockDbLogger = new Mock<IDbLogger>();
            _mockCommonProviderFunctions = new Mock<CommonProviderFunctions>();
            _mockCogatFeedbackSender = new Mock<ICogatFeedbackSender>();
            
            var mockUserDataManager = new Mock<IUserDataManager>();

            _userData = new UserData();
            mockUserDataManager.Setup(x => x.GetUserData()).Returns(_userData);

            _sut = new GlobalProvider(_mockLoginManager.Object, mockUserDataManager.Object, _mockDbLogger.Object, _mockWebReportingClient.Object, _mockUserApiClient.Object, _mockSessionManager.Object, _mockCogatFeedbackSender.Object, _mockEncryptionManagerElevate.Object);
        }

        [Fact]
        public void IsDemo_ShouldBeFalseIfUserDataIsNull()
        {
            _userData = null;
            var mockUserDataManager = new Mock<IUserDataManager>();
            mockUserDataManager.Setup(x => x.GetUserData()).Returns(_userData);

            var actual = _sut.IsDemo();

            Assert.False(actual);
        }

        [Fact]
        public void BuildFooterModel_ShouldSetTermsOfUseLinksIfUserDataIsNull()
        {
            var expectedTermsOfUse = "https://www.riversidedatamanager.com/BalancedManagement/termsOfuse";
            var expectedPrivacyPolicy = "https://www.riversidedatamanager.com/BalancedManagement/termsOfUse#PrivacyPolicy";

            _userData = null;
            var mockUserDataManager = new Mock<IUserDataManager>();
            mockUserDataManager.Setup(x => x.GetUserData()).Returns(_userData);

            var actual = _sut.BuildFooterModel();

            Assert.Equal(expectedPrivacyPolicy, actual.PrivacyPolicyLink.Link);
            Assert.Equal(expectedTermsOfUse, actual.TermsOfUseLink.Link);
        }
    }
}