using DM.WR.BL.Providers;
using DM.WR.Models.Email;
using DM.WR.Web.ActionFilters;
using DM.WR.Web.Infrastructure;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DM.WR.Web.Reskin.Controllers
{
    public partial class UtilityController : BaseController
    {
        private readonly IGlobalProvider _provider;
        private readonly IWebsiteHelper _websiteHelper;

        public UtilityController(IGlobalProvider provider, IWebsiteHelper websiteHelper) : base(websiteHelper)
        {
            _provider = provider;
            _websiteHelper = websiteHelper;
        }

        [HttpPost]
        public virtual ActionResult KeepAlive()
        {
            _provider.CheckOptionsExistence();
            //TODO:  Maybe some sort of Session existence check here?  Like, are the Options still there??
            return PartialView(MVC.Shared.Views.dm_ui._Empty);
        }

        [HttpGet]
        public virtual PartialViewResult DisplayReportingKeyModal()
        {
            var model = _provider.BuildReportingKeyModal();
            return PartialView(MVC.Utility.Views.ViewNames._ReportingKeyModal, model);
        }

        [HttpPost]
        public virtual async Task<PartialViewResult> AddReportingKey(string reportingKey)
        {
            var model = await _provider.AddReportingKey(reportingKey);

            if (model.Success)  //Message Text - ST-6595
                return AjaxHtmlAlertSuccess("Reporting Key successfully added. You can now run reports for the new location.");

            return PartialView(MVC.Utility.Views.ViewNames._ReportingKeyModal, model);
        }

        [HttpGet]
        [UnderMaintenance(Disable = true)]
        public virtual PartialViewResult UnderMaintenance()
        {
            ViewBag.Title = "Under Maintenance";
            return PartialView(MVC.Utility.Views.ViewNames.UnderMaintenancePage);
        }

        [HttpGet]
        public virtual ActionResult Navigate(string to)
        {
            var url = _provider.GetRedirectUrl(to, _websiteHelper.GetAppPath());
            return Redirect(url);
        }

        [HttpGet]
        public virtual async Task<ActionResult> WebReports(string reprocess)
        {
            var model = await _provider.ReprocessSiteEntryForAllTestEvents(!string.IsNullOrEmpty(reprocess));

            if (!string.IsNullOrEmpty(model.RedirectToUrl))
                return Redirect(model.RedirectToUrl);

            if (!string.IsNullOrEmpty(model.ErrorMessage))
                return RedirectToAction(MVC.Error.ActionNames.Index, MVC.Error.Name);

            return RedirectToAction(MVC.Options.ActionNames.Index, MVC.Options.Name);
        }

        [HttpPost]
        public virtual async Task<string> SendFeedback(FeedbackModel model)
        {
            await _provider.SendFeedback(model);

            return "success";
        }
    }
}