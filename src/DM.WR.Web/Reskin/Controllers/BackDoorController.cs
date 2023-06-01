using DM.WR.BL.Providers;
using DM.WR.Models.Config;
using DM.WR.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DM.WR.Web.Reskin.Controllers
{
    public partial class BackDoorController : Controller
    {
        private readonly IGlobalProvider _provider;

        public BackDoorController(IGlobalProvider provider)
        {
            _provider = provider;
        }

        [HttpGet]
        public virtual ActionResult Index()
        {
            if (Request.Url != null)
            {
                var host = Request.Url.Host;

                if (host.StartsWith("iowaflexdemo.") && host.Count(h => h == '.') > 1)
                    return RedirectToAction(MVC.Demo.ActionNames.IowaFlex, MVC.Demo.Name);
            }

            var model = _provider.BuildBackDoorModel();
            return View(MVC.BackDoor.Views.ViewNames.BackDoor, model);
        }

        [HttpPost]
        public virtual async Task<ActionResult> Index(EntranceViewModel model)
        {
            model = await _provider.ProcessBackdoorSiteEntry(model);

            if (!string.IsNullOrEmpty(model.ErrorMessage))
                return View(MVC.BackDoor.Views.ViewNames.BackDoor, model);

            if (model.View == WebReportingView.Classic)
                return RedirectToAction(MVC.Options.ActionNames.Index, MVC.Options.Name);
            if (model.View == WebReportingView.IowaFlex)
                return RedirectToAction(MVC.DashboardIowaFlex.ActionNames.Index, MVC.DashboardIowaFlex.Name);
            if (model.View == WebReportingView.Cogat)
                return RedirectToAction(MVC.DashboardCogat.ActionNames.Index, MVC.DashboardCogat.Name);

            return RedirectToAction(MVC.Error.ActionNames.NotFound, MVC.Error.Name);
        }

        public virtual async Task<ActionResult> GetElevate(string token)
        {
            
            var view = WebReportingView.Cogat;

            if (view == null)
                return RedirectToAction(MVC.Error.ActionNames.NotFound, MVC.Error.Name);

            var model = await _provider.ProcessDmSiteEntryElevates(token);

            if (!string.IsNullOrEmpty(model.RedirectToUrl))
                return Redirect(model.RedirectToUrl);
            if (view == WebReportingView.Cogat)
                return RedirectToAction(MVC.DashboardCogat.ActionNames.Index, MVC.DashboardCogat.Name);

            return RedirectToAction(MVC.Error.ActionNames.NotFound, MVC.Error.Name);
        }

        [HttpGet]
        public virtual async Task<ActionResult> GetElevateWithCookie()
        {
            //this endpoint requires the Elevate auth cookies

            var view = WebReportingView.Cogat;
            var cookies = Request.Cookies;
            var idTokenCookie = cookies.Get("idToken");

            if (view == null)
                return RedirectToAction(MVC.Error.ActionNames.NotFound, MVC.Error.Name);

            var model = await _provider.ProcessDmSiteEntryElevate(Request.Cookies);

            if (string.Equals(model.ErrorMessage,"Unauthorized",System.StringComparison.OrdinalIgnoreCase))
            {
                return Redirect(ConfigSettings.ReturnUrl);
            }

            if (!string.IsNullOrEmpty(model.RedirectToUrl))
                return Redirect(model.RedirectToUrl);

            if (!string.IsNullOrEmpty(model.ErrorMessage))
                return RedirectToAction(MVC.Error.ActionNames.NotFound, MVC.Error.Name);

            if (view == WebReportingView.Cogat)
                return RedirectToAction(MVC.DashboardCogat.ActionNames.Index, MVC.DashboardCogat.Name);

            return RedirectToAction(MVC.Error.ActionNames.NotFound, MVC.Error.Name);
        }

        [HttpGet]
        public virtual async Task<ActionResult> AfterRedirection()
        {
            var models = await _provider.ProcessReEntry(Request.Cookies);

            //var model = await _provider.ProcessDmSiteEntryElevate(Request.Cookies);
            return RedirectToAction(MVC.DashboardCogat.ActionNames.Index, MVC.DashboardCogat.Name);
        }
    }
}