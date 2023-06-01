using DM.WR.BL.Providers;
using DM.WR.Models.IowaFlex;
using DM.WR.Web.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;

namespace DM.WR.Web.Reskin.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public partial class IowaFlexLongitudinalController : BaseController
    {
        private readonly IIowaFlexLongitudinalProvider _provider;
        private readonly string _appPath;

        public IowaFlexLongitudinalController(IIowaFlexLongitudinalProvider provider, IWebsiteHelper websiteHelper) : base(websiteHelper)
        {
            _provider = provider;
            _appPath = websiteHelper.GetAppPath();
        }

        [HttpGet]
        public virtual ActionResult Index(string enableQueryLogging)
        {
            var model = _provider.BuildPageViewModel(enableQueryLogging);
            model.ContentIndicator = "web";
            model.AppPath = _appPath;

            return View(MVC.IowaFlexLongitudinal.Views.IowaFlexLongitudinalPage, model);
        }

        [HttpGet]
        public virtual PartialViewResult GetFilters()
        {
            var model = _provider.GetFilters(_appPath);
            return PartialView(MVC.IowaFlexLongitudinal.Views.ViewNames._FiltersPanel, model);
        }

        [HttpPost]
        public virtual async Task<JsonResult> UpdateFilters(string filterType, List<string> values)
        {
            await _provider.UpdateFiltersAsync(filterType, values);
            return Json("Success");
        }

        [HttpGet]
        public virtual JsonResult ResetFilters()
        {
            _provider.ResetFilters();
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual JsonResult PersistLongitudinalFilters(bool persist)
        {
            _provider.PersistLongitudinalFilters(persist);
            return Json("Success");
        }

        [HttpGet]
        public virtual async Task<object> GoToRootNode()
        {
            await _provider.GoToRootNodeAsync();
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual async Task<object> DrillDownLocations(int id, string name, string type)
        {
            var node = new LocationNode { NodeId = id, NodeName = name, NodeType = type };
            await _provider.DrillDownLocationsPathAsync(node);

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual async Task<object> DrillUpLocations(int id, string name, string type)
        {
            var node = new LocationNode { NodeId = id, NodeName = name, NodeType = type };
            await _provider.DrillUpLocationsPathAsync(node);

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual async Task<string> GetTrendAnalysis()
        {
            var model = await _provider.GetTrendAnalysisAsync();
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public virtual async Task<string> GetGainsAnalysis(string testEventIds)
        {
            var model = await _provider.GetGainsAnalysisAsync(testEventIds);
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public virtual async Task<string> GetRoster(string testEventIds)
        {
            var model = await _provider.GetRosterAsync(testEventIds, _appPath);
            return JsonConvert.SerializeObject(model);
        }
    }
}