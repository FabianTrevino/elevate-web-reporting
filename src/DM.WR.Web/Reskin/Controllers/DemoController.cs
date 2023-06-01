using DM.WR.BL.Providers;
using System.Web.Mvc;

namespace DM.WR.Web.Reskin.Controllers
{
    public partial class DemoController : Controller
    {
        private readonly IGlobalProvider _provider;

        public DemoController(IGlobalProvider provider)
        {
            _provider = provider;
        }

        [HttpGet]
        public virtual ActionResult IowaFlex()
        {
            var model = _provider.ProcessDemoSiteEntry(isAdaptive: true);

            if (!string.IsNullOrEmpty(model.ErrorMessage))
                return RedirectToAction(MVC.Error.ActionNames.NotFound, MVC.Error.Name);

            return RedirectToAction(MVC.DashboardIowaFlex.ActionNames.Index, MVC.DashboardIowaFlex.Name);
        }
    }
}