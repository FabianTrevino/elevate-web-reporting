using DM.WR.BL.Providers;
using DM.WR.Web.ActionFilters;
using DM.WR.Web.Infrastructure;
using Rotativa;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DM.WR.Web.Reskin.Controllers
{
    [RestrictAdaptiveAccess]
    public partial class LibraryController : BaseController
    {
        private readonly ILibraryProvider _provider;
        private readonly string websiteAppPath;
        public LibraryController(ILibraryProvider provider, IWebsiteHelper websiteHelper) : base(websiteHelper)
        {
            _provider = provider;
            websiteAppPath = websiteHelper.GetAppPath();
            ViewBag.Tabs = NavigationHelper.Tabs(Models.TabsNavigationEnum.Library);
        }

        [HttpGet]
        public virtual async Task<ActionResult> Index()
        {
            var model = await _provider.BuildModelAsync();
            model.appPath = websiteAppPath;
            return View(MVC.Library.Views.ViewNames.LibraryPage, model);
        }

        //[HttpGet]
        //public virtual async Task<ActionResult> GetReport(string reportId)
        //{
        //    var bytes = await _provider.GetReportAsync(reportId);
        //    return new FileContentResult(bytes, "application/pdf");
        //}
        [HttpPost, ValidateInput(false)]
        public virtual ActionResult PrintDataExporter(string input)
        {
            Response.AppendHeader("Content-Disposition", new System.Net.Mime.ContentDisposition { Inline = true, FileName = "FieldNameAndLengths.pdf" }.ToString());
            return new ViewAsPdf(MVC.DashboardCogat.Views.DashboardPdfPage, model: input)
            {
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Landscape
            };
        }
        [HttpPost]
        public virtual ActionResult UpdateOptions(string parameters)
        {
            _provider.UpdateOptions(parameters);
            return PartialView(MVC.Shared.Views.dm_ui.ViewNames._Empty);
        }
        [HttpPost]
        public virtual ActionResult DeleteDataExporterSession()
        {
            _provider.DeletdataExporterSession();
            return PartialView(MVC.Shared.Views.dm_ui.ViewNames._Empty);
        }
    }
}