using DM.WR.BL.Providers;
using DM.WR.Web.ActionFilters;
using DM.WR.Web.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using DM.WR.Models.ViewModels;
using Rotativa;
using System.Configuration;
using DM.WR.Web.Models;
using System.Net;
using DM.WR.Models.BackgroundReport.ElevateReports;

namespace DM.WR.Web.Reskin.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public partial class DashboardCogatController : BaseController
    {
        private readonly IOptionsProvider _BackGroundprovider;
        private readonly ICogatProvider _provider;
        private readonly string _appPath;

        public DashboardCogatController(ICogatProvider provider, IOptionsProvider BackGroundprovider, IWebsiteHelper websiteHelper) : base(websiteHelper)
        {

            _provider = provider;
            _BackGroundprovider = BackGroundprovider;
            _appPath = websiteHelper.GetAppPath();
        }

        [HttpGet]
        public virtual ActionResult Index()
        {
            var model = _provider.BuildPageViewModel();
            model.AppPath = _appPath;

            return View(MVC.DashboardCogat.Views.DashboardPage, model);
        }

        [HttpGet]
        public virtual PartialViewResult GetFilters()
        {
            var model = _provider.GetFilters(_appPath);
            if (string.Equals(model.ErrorMessage,"Unauthorized",StringComparison.OrdinalIgnoreCase))
            {
                
               return PartialView(MVC.Partials.Views._RedirectOut,model);
            }
            if (!string.IsNullOrEmpty(model.ErrorMessage))
                return AjaxHtmlAlertError(model.ErrorMessage);

            return PartialView(MVC.DashboardCogat.Views.ViewNames._FiltersPanel, model);
        }
        [HttpGet]
        public virtual PartialViewResult GetOptions()
        {
            var model = _BackGroundprovider.GetOptions();
            return PartialView(MVC.Options.Views.ViewNames._Options_TopToBottom, model);
        }
        [HttpPost]
        public virtual JsonResult UpdateFilters(string filterType, List<string> values)
        {
            CogatFiltersViewModel cogatFiltersViewModel = new CogatFiltersViewModel();
            _provider.UpdateFilters(filterType, values, out cogatFiltersViewModel);

            if (cogatFiltersViewModel.ErrorMessage == "Unauthorized")
            {
               return Json("Unauthorized");
            }
            return Json("Success");
        }

        private ActionResult GetRedirection()
        {
            return View(MVC.Partials.Views._RedirectOut);
        }
        [HttpPost]
        public virtual JsonResult ResetPage()
        {
            _provider.ResetPage();
            return Json("Success");
        }

        [HttpGet]
        public async Task<string> GetRosterRowsCount(int testFetchSize, int smFetchSize)
        {
            var model = await _provider.GetRecordsCountAsync(testFetchSize, smFetchSize);

            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public async Task<string> GetRosterRowsCountNew()
        {
           
            var model = await _provider.GetCutScoreAsync();
          
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public async Task<string> GetCutScore(string groupingType, string filter)
        {
           
            var model = await _provider.GetCutScoreAsync(groupingType, filter);
            
            return JsonConvert.SerializeObject(model);
        }


        [HttpGet]
        public async Task<string> GetStudentRoster(int take, int skip, string filter, string score, string orderType)
        {
            
            var model = await _provider.GetStudentRosterAsync(take, skip, filter, score, orderType);
            
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public async Task<string> GetAllData()
        {
           
            var model = await _provider.GetAllDataAsync();
          
            return JsonConvert.SerializeObject(model);
        }
        [HttpGet]
        public async Task<string> GetAgeStanines()
        {
            
            var model = await _provider.GetAgeStaninesAsync();
          
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public async Task<string> GetAbilityProfiles()
        {
           
            var model = await _provider.GetAbilityProfilesAsync();
         
            return JsonConvert.SerializeObject(model);
        }

        [HttpGet]
        public async Task<string> GetGroupTotals()
        {
            try
            {
                var model = await _provider.GetGroupTotalsAsync();

                return JsonConvert.SerializeObject(model);
            }
            catch (Exception ex)
            {
                if (string.Equals(ex.Message, "Unauthorized", StringComparison.OrdinalIgnoreCase))
                {
                    return ex.Message;
                }
                throw ex;
            }
        }

        [HttpPost, ValidateInput(false)]
        public virtual ActionResult Print(string reportType, string input)
        {
            switch (reportType)
            {
                case "differentiated_instruction":
                    input = HttpUtility.UrlDecode(input, Encoding.GetEncoding("windows-1251"));
                    var stronglyTypedInput = JsonConvert.DeserializeObject<DashboardDifferentiatedInstructionReportViewModel>(input);
                    Response.AppendHeader("Content-Disposition", new System.Net.Mime.ContentDisposition { Inline = true, FileName = "Differentiated Instruction Report.pdf" }.ToString());
                    return new ViewAsPdf(MVC.DashboardCogat.Views.DifferentiatedInstructionReportPage, stronglyTypedInput)
                    {
                        PageSize = Rotativa.Options.Size.A4,
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        PageMargins = new Rotativa.Options.Margins { Top = 8, Bottom = 8, Left = 8, Right = 8 },
                        //CustomSwitches = "--no-outline"
                        CustomSwitches = "--no-outline --disable-smart-shrinking --dpi 192 --page-width 12.64in --page-height 17.2in"
                    };
                default:

                    Response.AppendHeader("Content-Disposition", new System.Net.Mime.ContentDisposition { Inline = true, FileName = "Dashboard.pdf" }.ToString());
                    return new ViewAsPdf(MVC.DashboardCogat.Views.DashboardPdfPage, model: input)
                    {
                        //FileName = "Dashboard.pdf",
                        PageSize = Rotativa.Options.Size.A4,
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        PageMargins = new Rotativa.Options.Margins { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                        //CustomSwitches = "--no-outline --zoom 0.65 --disable-smart-shrinking"
                        CustomSwitches = "--no-outline --disable-smart-shrinking --dpi 192 --page-width 12.64in --page-height 17.2in"
                    };

            }
        }

        #region BackGround Report Request
        [HttpPost]
        public virtual async Task<PartialViewResult> SendToBackground(ElevateUiBackgroundRequest ECogatRequest)
        {

            var message = await _provider.SendReportToBackgroundAsync(ECogatRequest);

            return message == "" ?
               AjaxHtmlAlertSuccess($"Your report, {ECogatRequest.FileName}, has been sent to the Report Library.") :
               AjaxHtmlAlertError(message);
        }


        #endregion
        #region Background Report Center

        [HttpGet]
        public virtual ActionResult GetfileforDownload(string Filename, string FilePath, string UserID, int FileID, string reportType)
        {
            var reportExt = "";
            var UserName = ConfigurationManager.AppSettings["folderUserName"];
            var Password = ConfigurationManager.AppSettings["folderPassword"];
            var domain = ConfigurationManager.AppSettings["folderDomain"];
            var folderPath = ConfigurationManager.AppSettings["folderPathDownload"];
            NetworkCredential credentials = new NetworkCredential(UserName, Password, domain);
            if (reportType == "CatalogExporter")
            {
                reportExt = ".txt";
                var root = folderPath + @"\" + UserID + @"\" + Filename + FileID + reportExt;
                var downloadFilename = Filename + reportExt;
                var downloadFileCheck = Filename + FileID;
                using (new ConnectToSharedFolder(root, credentials))
                {
                    System.IO.FileStream fs = System.IO.File.OpenRead(root);
                    byte[] data = new byte[fs.Length];
                    int br = fs.Read(data, 0, data.Length);
                    if (br != fs.Length)
                        throw new System.IO.IOException(root);
                    //byte[] filebytes = System.IO.File.ReadAllBytes(root);
                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = Filename + ".txt",
                        Inline = false
                    };
                    Response.AppendHeader("content-disposition", cd.ToString());
                    return File(data, cd.ToString(), downloadFilename);
                }
            }
            else
            {
                reportExt = ".pdf";
                var root = folderPath + @"\" + UserID + @"\" + Filename + FileID + reportExt;
                var downloadFilename = Filename + reportExt;
                var downloadFileCheck = Filename + FileID;
                //var ff = "";
                //if (downloadFilename.Contains(","))
                //{
                //    ff= downloadFilename.Replace(",", " ");
                //}
                using (new ConnectToSharedFolder(root, credentials))
                {
                    byte[] filebytes = System.IO.File.ReadAllBytes(root);
                    Response.AppendHeader("content-disposition", $"inline;filename={downloadFilename}");
                    return File(filebytes, downloadFilename);
                }
            }
        }

        [HttpGet]
        public virtual Task<string> GetFileByID()
        {
            var retItem = _provider.GetFileByUserID();
            return retItem;
        }
        [HttpGet]
        public virtual PartialViewResult DeletePDF(int FileID)
        {
            var message = "";
            _provider.DeletePDF(FileID);
            return message == "" ?
               AjaxHtmlAlertSuccess($"Report Deleted Successfully") :
               AjaxHtmlAlertError(message);
        }
        [HttpGet]
        public virtual Task<string> GetDataExportData(int fileID)
        {
            var retItem = _provider.GetDataExportData(fileID);
            return retItem;
        }

        #endregion
        //[HttpGet]
        //public string GetScoreWarnings()
        //{
        //    var model = _provider.GetScoreWarnings();

        //    return JsonConvert.SerializeObject(model);
        //}
    }
}