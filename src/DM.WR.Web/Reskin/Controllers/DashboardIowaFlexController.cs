using DM.WR.BL.Providers;
using DM.WR.Web.Infrastructure;
using Rotativa;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DM.WR.Web.Reskin.Controllers
{
    public partial class DashboardIowaFlexController : BaseController
    {
        private readonly IIowaFlexProvider _provider;
        private readonly string _appPath;

        public DashboardIowaFlexController(IIowaFlexProvider provider, IWebsiteHelper websiteHelper) : base(websiteHelper)
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

            return View(MVC.DashboardIowaFlex.Views.DashboardPage, model);
        }

        [HttpGet]
        public virtual async Task<PartialViewResult> GetFilters(string cogat)
        {
            var model = await _provider.GetFiltersAsync(_appPath, cogat);
            return PartialView(MVC.DashboardIowaFlex.Views.ViewNames._FiltersPanel, model);
        }

        [HttpPost]
        public virtual async Task<JsonResult> UpdateFilters(string filterType, List<string> values)
        {
            await _provider.UpdateFiltersAsync(filterType, values, _appPath);
            return Json("Success");
        }

        [HttpGet]
        public async Task<string> GetPerformanceScoresKto1()
        {
            var model = await _provider.GetPerformanceScoresKto1Async();
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public async Task<string> GetPerformanceDonutsKto1()
        {
            var model = await _provider.GetPerformanceDonutsKto1Async(null, null);
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public async Task<string> GetRosterKto1(string pldStage, int? pldLevel)
        {
            var model = await _provider.GetRosterKto1Async(_appPath, pldStage, pldLevel);
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public async Task<string> GetDifferentiatedReportHierarchy()
        {
            var model = await _provider.GetDifferentiatedReportHierarchyKto1Async();
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public async Task<string> GetCogatLocationRoster(int? performanceBand, int? domainId, int? domainLevel, int? cogatAbility, string cogatScore)
        {
            var model = await _provider.GetCogatLocationRosterAsync(_appPath, performanceBand, domainId, domainLevel, cogatAbility, cogatScore);
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public async Task<string> GetCogatStudentRoster(int? performanceBand, int? domainId, int? domainLevel, int? cogatAbility, string cogatScore, string contentName)
        {
            var model = await _provider.GetCogatStudentRosterAsync(_appPath, performanceBand, domainId, domainLevel, cogatAbility, cogatScore, contentName);
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public virtual ActionResult Print()
        {
            return View(MVC.Shared.Views.ViewNames.EmptyPage);
        }

        [HttpGet]
        public async Task<string> GetPerformanceLevelMatrix(string contentType, string contentName, string performanceBand, string domainId, string domainLevel)
        {
            var model = await _provider.GetPerformanceLevelMatrix(contentType, contentName, performanceBand, domainId, domainLevel);

            return JsonConvert.SerializeObject(model);
        }

        [HttpPost, ValidateInput(false)]
        public virtual async Task<ActionResult> Print(string reportType, string input)
        {
            switch (reportType)
            {
                case "profile_narrative":
                    var viewModel = await _provider.GetProfileNarrativeAsync(input);

                    Response.AppendHeader("Content-Disposition", new System.Net.Mime.ContentDisposition { Inline = true, FileName = "Student Profile Narrative.pdf" }.ToString());
                    return new ViewAsPdf(MVC.DashboardIowaFlex.Views.ProfileNarrativePage, viewModel)
                    {
                        PageSize = Rotativa.Options.Size.A4,
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        PageMargins = new Rotativa.Options.Margins { Top = 8, Bottom = 8, Left = 8, Right = 8 },
                        //CustomSwitches = "--no-outline --zoom 0.75 --disable-smart-shrinking --dpi 192"
                        //CustomSwitches = "--no-outline --disable-smart-shrinking --dpi 192 --page-width 13.3in --page-height 21in"
                        CustomSwitches = "--no-outline --disable-smart-shrinking --dpi 192 --page-width 12.64in --page-height 17.2in"
                    };
                case "profile_narrative_k1":
                    var viewModelKto1Narrative = await _provider.GetProfileNarrativeKto1Async(input);

                    Response.AppendHeader("Content-Disposition", new System.Net.Mime.ContentDisposition { Inline = true, FileName = "Student Profile Narrative.pdf" }.ToString());
                    //return View(MVC.DashboardIowaFlex.Views.ProfileNarrativeKto1Page, viewModelKto1Narrative);
                    return new ViewAsPdf(MVC.DashboardIowaFlex.Views.ProfileNarrativeKto1Page, viewModelKto1Narrative)
                    {
                        PageSize = Rotativa.Options.Size.A4,
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        PageMargins = new Rotativa.Options.Margins { Top = 8, Bottom = 8, Left = 8, Right = 8 },
                        //CustomSwitches = "--no-outline --zoom 0.75 --disable-smart-shrinking --dpi 192"
                        //CustomSwitches = "--no-outline --disable-smart-shrinking --dpi 192 --page-width 13.3in --page-height 21in"
                        CustomSwitches = "--no-outline --disable-smart-shrinking --dpi 192 --page-width 12.64in --page-height 17.2in"
                    };
                case "differentiated_instruction":
                    var viewModelKto1Differentiated = await _provider.GetDifferentiatedReportKto1Async(input);

                    Response.AppendHeader("Content-Disposition", new System.Net.Mime.ContentDisposition { Inline = true, FileName = "Differentiated Instruction Report.pdf" }.ToString());
                    //return View(MVC.DashboardIowaFlex.Views.ProfileDifferentiatedKto1Page, viewModelKto1Differentiated);
                    return new ViewAsPdf(MVC.DashboardIowaFlex.Views.ProfileDifferentiatedKto1Page, viewModelKto1Differentiated)
                    {
                        PageSize = Rotativa.Options.Size.A4,
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        PageMargins = new Rotativa.Options.Margins { Top = 8, Bottom = 8, Left = 8, Right = 8 },
                        //CustomSwitches = "--no-outline --zoom 0.75 --disable-smart-shrinking --dpi 192"
                        //CustomSwitches = "--no-outline --disable-smart-shrinking --dpi 192 --page-width 13.3in --page-height 21in"
                        CustomSwitches = "--no-outline --disable-smart-shrinking --dpi 192 --page-width 12.64in --page-height 17.2in"
                    };
                default:
                    Response.AppendHeader("Content-Disposition", new System.Net.Mime.ContentDisposition { Inline = true, FileName = "Dashboard.pdf" }.ToString());
                    //return View(MVC.DashboardIowaFlex.Views.DashboardPdfPage, model: input);
                    return new ViewAsPdf(MVC.DashboardIowaFlex.Views.DashboardPdfPage, model: input)
                    {
                        PageSize = Rotativa.Options.Size.A4,
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        PageMargins = new Rotativa.Options.Margins { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                        //CustomSwitches = "--no-outline --zoom 0.62 --disable-smart-shrinking --dpi 192"
                        //CustomSwitches = "--no-outline --disable-smart-shrinking --dpi 192 --page-width 12.64in --page-height 17.2in"
                        CustomSwitches = "--no-outline --disable-smart-shrinking --dpi 192 --page-width 13.3in --page-height 17.21in"
                    };
            }
        }
    }
}