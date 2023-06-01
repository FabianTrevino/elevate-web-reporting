using DM.WR.BL.Providers;
using DM.WR.Web.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


namespace DM.WR.Web.Api
{
    public class CogatDataToJsonController : ApiController
    {

        private readonly IOptionsProvider _BackGroundprovider;
        private readonly ICogatProvider _provider;
        private readonly string _appPath;
        public CogatDataToJsonController(ICogatProvider provider, IOptionsProvider BackGroundprovider, IWebsiteHelper websiteHelper)
        {
            _provider = provider;
            _BackGroundprovider = BackGroundprovider;
            _appPath = websiteHelper.GetAppPath();
        }

        [HttpGet]
        [Route("api/CogatDataToJson/GetBackGroundGradeLocations")]
        public object GetFilters()
        {
            var model = _provider.GetBackGroundGradeLocations();
            return Json(model);
        }

        //GetStudentsDataDebug
        [HttpGet]
        [Route("api/CogatDataToJson/GetStudentsDataDebug")]
        public object GetStudentsDataDebug()
        {
            var model = _provider.GetStudentsDataDebug();
            return Json(model);
        }

        [HttpGet]
        [Route("api/CogatDataToJson/GetGroupTotalsPayloadDebug")]
        public object GetGroupTotalsPayloadDebug()
        {
            var model = _provider.GetGroupTotalsPayloadForDebug();
            return Json(model);
        }
    }
}
