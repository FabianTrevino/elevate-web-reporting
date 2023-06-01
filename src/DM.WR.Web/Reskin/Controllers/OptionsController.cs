using DM.WR.BL.Providers;
using DM.WR.Models.Options;
using DM.WR.Web.ActionFilters;
using DM.WR.Web.Infrastructure;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DM.WR.Web.Reskin.Controllers
{

    [RestrictAdaptiveAccess]//, BuildMainMenu]
    public partial class OptionsController : BaseController
    {
        private readonly IOptionsProvider _provider;

        public OptionsController(IOptionsProvider provider, IWebsiteHelper websiteHelper) : base(websiteHelper)
        {
            _provider = provider;

            ViewBag.Tabs = NavigationHelper.Tabs(Models.TabsNavigationEnum.Options);
        }

        [HttpGet]
        public virtual ActionResult Index()
        {
            var model = _provider.BuildDefaultModel();
            return View(MVC.Options.Views.ViewNames.OptionsPage, model);
        }

        [HttpGet]
        public virtual PartialViewResult DisplayLocationChangeModal()
        {
            var model = _provider.BuildLocationChangeModalModel();
            return PartialView(MVC.Options.Views.ViewNames._LocationChangeModal, model);
        }

        [HttpPost]
        public virtual PartialViewResult SwitchLocation(int id)
        {
            _provider.SwitchLocation(id);
            return PartialView(MVC.Shared.Views.dm_ui._Empty);
        }

        [HttpGet]
        public virtual PartialViewResult GetOptions()
        {
            var model = _provider.GetOptions();
            return PartialView(MVC.Options.Views.ViewNames._Options_TopToBottom, model);
        }

        [HttpPost]
        public virtual PartialViewResult GetGroup(string groupType)
        {
            var group = _provider.GetGroup(groupType);
            return PartialView(MVC.Options.Views.ViewNames._OptionsGroupSwitch, new List<OptionGroup> { group });
        }

        [HttpPost]
        public virtual JsonResult UpdateOptions(string groupType, List<string> values)
        {
            _provider.UpdateOptions(groupType, values);
            return Json("Success");
        }

        [HttpPost]
        public virtual PartialViewResult ResetOptions()
        {
            _provider.ResetOptions();
            return PartialView(MVC.Shared.Views.dm_ui._Empty);
        }

        [HttpPost]
        public virtual PartialViewResult AddMultimeasureColumn()
        {
            _provider.AddMultimeasureColumn();
            return PartialView(MVC.Shared.Views.dm_ui._Empty);
        }

        [HttpPost]
        public virtual PartialViewResult DeleteMultimeasureColumn()
        {
            _provider.RemoveCurrentMultimeasureColumn();
            return PartialView(MVC.Shared.Views.dm_ui._Empty);
        }

        [HttpPost]
        public virtual PartialViewResult GoToMultimeasureColumn(int columnNumber)
        {
            _provider.GoToMultimeasureColumn(columnNumber);
            return PartialView(MVC.Shared.Views.dm_ui._Empty);
        }

        [HttpPost]
        public virtual PartialViewResult ClearEditMode()
        {
            _provider.ClearCriteriaEditMode();
            return PartialView(MVC.Shared.Views.dm_ui._Empty);
        }
    }
}