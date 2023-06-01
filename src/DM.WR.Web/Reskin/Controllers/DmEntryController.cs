using DM.WR.BL.Providers;
using DM.WR.Models.Config;
using DM.WR.Web.Infrastructure;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DM.WR.Web.Reskin.Controllers
{
    public partial class DmEntryController : BaseController
    {
        private readonly IGlobalProvider _provider;

        public DmEntryController(IGlobalProvider provider, IWebsiteHelper websiteHelper) : base(websiteHelper, false)
        {
            _provider = provider;
        }

        //NOTE:  Param contractInstance has to be encrypted before the call to this function.
        ////[RequestFilter]
        // ReSharper disable once InconsistentNaming
        public virtual async Task<ActionResult> Index(string token, string LoginSource, string view, string contractInstance)
        {
            Logger.Info($"Navigation from DM :: /DmEntry?token={token}&LoginSource={LoginSource}&view={view}&contractInstance={contractInstance}");

            if (view == null)
                return RedirectToAction(MVC.Error.ActionNames.NotFound, MVC.Error.Name);

            var model = await _provider.ProcessDmSiteEntry(token, LoginSource, contractInstance, view == WebReportingView.IowaFlex);

            if (!string.IsNullOrEmpty(model.RedirectToUrl))
                return Redirect(model.RedirectToUrl);

            if (!string.IsNullOrEmpty(model.ErrorMessage))
                return RedirectToAction(MVC.Error.ActionNames.Index, MVC.Error.Name);
            if (view == WebReportingView.IowaFlex)
                return RedirectToAction(MVC.DashboardIowaFlex.ActionNames.Index, MVC.DashboardIowaFlex.Name);
            if (view == WebReportingView.Cogat)
                return RedirectToAction(MVC.DashboardCogat.ActionNames.Index, MVC.DashboardCogat.Name);

            return RedirectToAction(MVC.Error.ActionNames.NotFound, MVC.Error.Name);
        }
    }
}