using DM.WR.BL.Providers;
using DM.WR.Web.ActionFilters;
using DM.WR.Web.Infrastructure;
using DM.WR.Web.Models;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;

namespace DM.WR.Web.Reskin.Controllers
{
    [RestrictAdaptiveAccess]
    public partial class ReportController : BaseController
    {
        private readonly IReportProvider _provider;

        public ReportController(IReportProvider provider, IWebsiteHelper websiteHelper) : base(websiteHelper)
        {
            _provider = provider;
        }

        [HttpPost]
        public virtual JsonResult GetReportViewerData()
        {
            return Json(_provider.BuildReportViewerModel());
        }

        [HttpPost]
        public virtual PartialViewResult UpdateOptions(string parameters)
        {
            _provider.UpdateOptions(parameters);
            return PartialView(MVC.Shared.Views.dm_ui._Empty);
        }

        [HttpPost]
        public virtual async Task<PartialViewResult> SendReportToBackground(string name)
        {
            var message = await _provider.SendReportToBackgroundAsync(name);

            return message == "" ?
                AjaxHtmlAlertSuccess($"Your report, {name}, has been sent to the Report Library.") :
                AjaxHtmlAlertError(message);
        }

        [HttpPost]
        public virtual JsonResult GetExcelReportString(string queryString)
        {
            return Json(_provider.CreateExcelQueryString(queryString));
        }

        [HttpPost]
        public virtual JsonResult GetLastNameSearchString(string lastName)
        {
            return Json(new { QueryString = _provider.ApplyLastNameSearch(lastName) });
        }

        [HttpGet]
        public virtual FileResult DownloadManager(string fileName)
        {
            var dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/Downloads") ?? throw new InvalidOperationException("Could not map '~/Downloads' directory from ReportController DownloadManager Action."));

            var filePath = "";
            foreach (var item in dirInfo.GetFiles(fileName))
            {
                filePath = item.FullName;
            }

            return File(filePath, "text/plain", fileName);
        }
        [HttpGet]
        public virtual ActionResult GetfileforDownload(string Filename,string FilePath ,string UserID, int FileID, string reportType)
        {
            var reportExt = "";
            var UserName = ConfigurationManager.AppSettings["folderUserName"];
            var Password = ConfigurationManager.AppSettings["folderPassword"];
            var domain = ConfigurationManager.AppSettings["folderDomain"];
            NetworkCredential credentials = new NetworkCredential(UserName, Password, domain);
            if (reportType == "CatalogExporter")
            {
                reportExt = ".txt";
                var root = FilePath + @"\" + Filename + FileID + reportExt;
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
                    return File(data, cd.ToString(),downloadFilename);
                }
            }
            else
            {
                reportExt = ".pdf";
                var root = FilePath + @"\" + Filename + FileID + reportExt;
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
    }
}