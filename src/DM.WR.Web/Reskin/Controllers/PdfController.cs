using DM.WR.Models.ViewModels;
using DM.WR.Web.Infrastructure;
using Rotativa;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DM.WR.Web.Reskin.Controllers
{
    public partial class PdfController : BaseController
    {
        public PdfController(IWebsiteHelper websiteHelper) : base(websiteHelper)
        {
        }

        [ValidateInput(false)]
        public virtual async Task<ActionResult> IndexCogat(DashboardDifferentiatedInstructionReportViewModel model)
        {
            Response.AppendHeader("Content-Disposition", new System.Net.Mime.ContentDisposition { Inline = true, FileName = "Differentiated Instruction Report.pdf" }.ToString());
            return new ViewAsPdf(MVC.DashboardCogat.Views.DifferentiatedInstructionReportPage, model)
            {
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageMargins = new Rotativa.Options.Margins { Top = 8, Bottom = 8, Left = 8, Right = 8 },
                //CustomSwitches = "--no-outline"
                CustomSwitches = "--no-outline --disable-smart-shrinking --dpi 192 --page-width 12.64in --page-height 17.2in"
            };
        }
    }
}