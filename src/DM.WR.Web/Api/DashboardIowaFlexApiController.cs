using DM.WR.BL.Providers;
using DM.WR.Models.IowaFlex;
using DM.WR.Web.ActionFilters;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DM.WR.Web.Api
{
    [WebApiExceptionFilter]
    public class DashboardIowaFlexApiController : ApiController
    {
        private readonly IIowaFlexProvider _provider;
        private readonly string _appPath;

        public DashboardIowaFlexApiController(IIowaFlexProvider provider)
        {
            _provider = provider;
            _appPath = HttpContext.Current.Request.ApplicationPath == "/" ? "" : HttpContext.Current.Request.ApplicationPath;
        }

        [HttpGet]
        [Route("api/Dashboard/ResetFilters")]
        public object ResetFilters()
        {
            _provider.ResetFilters();
            return Json("success");
        }

        [HttpGet]
        [Route("api/Dashboard/GoToRootNode")]
        public async Task<object> GoToRootNode()
        {
            await _provider.GoToRootNodeAsync();
            return Json("success");
        }

        [HttpGet]
        [Route("api/Dashboard/DrillDownLocations")]
        public async Task<object> DrillDownLocations(int id, string name, string type)
        {
            var node = new LocationNode { NodeId = id, NodeName = name, NodeType = type };
            await _provider.DrillDownLocationsPathAsync(node);

            return Json("success");
        }

        [HttpGet]
        [Route("api/Dashboard/DrillUpLocations")]
        public async Task<object> DrillUpLocations(int id, string name, string type)
        {
            var node = new LocationNode { NodeId = id, NodeName = name, NodeType = type };
            await _provider.DrillUpLocationsPathAsync(node);

            return Json("success");
        }

        [HttpGet]
        [Route("api/Dashboard/GetQuartiles")]
        public async Task<object> GetQuartiles(string performanceBand, string domainId, string domainLevel, string cogat)
        {
            var model = await _provider.GetTestScoresAsync(_appPath, performanceBand, domainId, domainLevel, cogat);
            return model;
        }

        [HttpGet]
        [Route("api/Dashboard/GetCards")]
        public async Task<object> GetCards(string cogat)
        {
            var model = await _provider.GetDomainsAsync(_appPath, null, cogat);
            return model;
        }

        [HttpGet]
        [Route("api/Dashboard/GetCards")]
        public async Task<object> GetCards(string range, string cogat)
        {
            var model = await _provider.GetDomainsAsync(_appPath, range, cogat);
            return model;
        }

        [HttpGet]
        [Route("api/Dashboard/GetRoster")]
        public async Task<object> GetRoster()
        {
            var model = await _provider.GetRosterAsync(_appPath);
            return model;
        }

        [HttpGet]
        [Route("api/Dashboard/GetStudentRoster")]
        public async Task<object> GetStudentRoster(string performanceBand, string domainId, string domainLevel)
        {
            var model = await _provider.GetStudentRosterAsync(_appPath, performanceBand, domainId, domainLevel);
            return model;
        }
    }
}