using DM.WR.BL.Builders;
using DM.WR.BL.Managers;
using DM.WR.BL.Providers;
using DM.WR.GraphQlClient;
using DM.WR.Models.Types;
using Moq;
using Xunit;

namespace DM.WR.BL.Tests.Providers
{
    public class IowaFlexLongitudinalProviderTests
    {
        private readonly Mock<IApiClient> _mockAdaptiveApiClient;
        private readonly Mock<IIowaFlexFiltersBuilder> _mockAdaptiveFiltersBuilder;
        private readonly Mock<IDashboardIowaFlexProviderBuilder> _mockDashboardIowaFlexProviderBuilder;
        private readonly Mock<IGraphQlQueryStringBuilder> _mockGraphQlQueryStringBuilder;
        private readonly Mock<ISessionManager> _mockSessionManager;
        private readonly Mock<IIowaFlexCommonProviderFunctions> _commonFlexFunctions;

        private readonly UserData _userData;
        private readonly IowaFlexLongitudinalProvider _sut;

        public IowaFlexLongitudinalProviderTests()
        {
            _mockAdaptiveApiClient = new Mock<IApiClient>();
            _mockAdaptiveFiltersBuilder = new Mock<IIowaFlexFiltersBuilder>();
            _mockDashboardIowaFlexProviderBuilder = new Mock<IDashboardIowaFlexProviderBuilder>();
            _mockGraphQlQueryStringBuilder = new Mock<IGraphQlQueryStringBuilder>();
            _mockSessionManager = new Mock<ISessionManager>();
            var mockUserDataManager = new Mock<IUserDataManager>();

            _userData = new UserData
            {
                IsAdaptive = true,
                IsDemo = true
            };
            mockUserDataManager.Setup(x => x.GetUserData()).Returns(_userData);

            _sut = new IowaFlexLongitudinalProvider(_mockAdaptiveApiClient.Object, /* _mockAdaptiveFiltersBuilder.Object, _mockDashboardIowaFlexProviderBuilder.Object, */_mockGraphQlQueryStringBuilder.Object, _mockSessionManager.Object, mockUserDataManager.Object, _commonFlexFunctions.Object);
        }

        [Fact]
        public void BuildPageViewModel_ShouldGetUserAndReturnMappedResults()
        {
            var actual = _sut.BuildPageViewModel("false");
            Assert.Equal(_userData.IsAdaptive, actual.IsAdaptive);
            Assert.Equal(_userData.IsDemo, actual.IsDemo);
        }
    }
}