using Rotativa;
using System.Web.Mvc;

namespace DM.WR.Web.Reskin.Controllers
{
    public partial class PdfDashboardCogatController : BaseController
    {
        [HttpGet]
        public virtual ActionResult Index()
        {
            return View(MVC.Shared.Views.ViewNames.EmptyPage);
        }

        [HttpPost, ValidateInput(false)]
        public virtual ActionResult Index(string html)
        {
            //var html = html.Replace(System.Environment.NewLine, "");
            //var html = Regex.Replace(html, @"\r\n?|\n", "");
            //return View(MVC.DashboardCogat.Views.DashboardPdfPage, model: html);
            Response.AppendHeader("Content-Disposition", new System.Net.Mime.ContentDisposition { Inline = true, FileName = "Dashboard.pdf" }.ToString());
            return new ViewAsPdf(MVC.DashboardCogat.Views.DashboardPdfPage, model: html)
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
}