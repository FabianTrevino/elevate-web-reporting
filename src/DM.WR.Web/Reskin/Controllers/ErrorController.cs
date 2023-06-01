using System.Web.Mvc;
using DM.WR.Web.Infrastructure;

namespace DM.WR.Web.Reskin.Controllers
{
    public partial class ErrorController : BaseController
    {
        private readonly IWebsiteHelper _websiteHelper;

        public ErrorController(IWebsiteHelper websiteHelper) : base(websiteHelper, false)
        {
            _websiteHelper = websiteHelper;
        }

        public virtual ActionResult Index()
        {
            ViewBag.Title = "Error";
            ViewBag.DataManagerUrl = _websiteHelper.GetDataManagerLoginUrl();
            var model = _websiteHelper.GetDataManagerAddReportingKeyUrl();
            return View(MVC.Error.Views.ViewNames.ErrorPage, model: model);
        }

        public virtual ActionResult ReportsNotAvailable()
        {
            ViewBag.HeaderViewModel = _websiteHelper.GetHeaderModel();
            ViewBag.MagicMenuModel = _websiteHelper.GetMainMenu();

            ViewBag.DataManagerUrl = _websiteHelper.GetDataManagerLoginUrl();
            return View(MVC.Error.Views.ViewNames.ReportsNotAvailablePage);
        }

        public virtual ActionResult NotFound()
        {
            var viewModel = _websiteHelper.BuildNotFoundPageModel();
            return View(MVC.Shared.Views.dm_ui.ViewNames._404, viewModel);
        }
    }
}