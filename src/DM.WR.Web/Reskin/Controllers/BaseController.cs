using DM.UI.Library;
using DM.UI.Library.Models;
using DM.WR.Web.Infrastructure;
using Newtonsoft.Json;
using NLog;
using System.Linq;
using System.Web.Mvc;

namespace DM.WR.Web.Reskin.Controllers
{
    public abstract partial class BaseController : DmUiBaseController
    {
        private readonly IWebsiteHelper _websiteHelper;
        internal static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected BaseController(IWebsiteHelper websiteHelper, bool initMenu = true)
        {
            //logger.Properties["loggerId"] = Guid.NewGuid();

            _websiteHelper = websiteHelper;

            ViewBag.Title = "Reports";
            ViewBag.MainTitle = "REPORTS CENTER";
            ViewBag.SectionColor = SectionColor.Green;

            if (initMenu && !_websiteHelper.IsDemo())
            {
                ViewBag.MagicMenuModel = _websiteHelper.GetMainMenu();

                ViewBag.HeaderViewModel = _websiteHelper.GetHeaderModel();
            }

            ViewBag.MagicFooterViewModel = _websiteHelper.GetFooterModel();

            ViewBag.UiSettings = JsonConvert.SerializeObject(_websiteHelper.GetUiSettings(), Formatting.None);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                return;

            //NLog
            Logger.Error($"Exception caught :: {filterContext.Exception}");

            //Log to SDR Database
            _websiteHelper.LogToDatabase(filterContext.Exception);

            //Redirect to appropriate error page or AJAX alert
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                var acceptTypes = filterContext.RequestContext.HttpContext.Request.AcceptTypes;

                if (acceptTypes != null && acceptTypes.Any(t => t.Contains("application/json")))
                    filterContext.Result = filterContext.Exception.Message.Contains("Adaptive Reporting API") ?
                        AjaxJsonAlertError(filterContext.Exception.Message) :
                        AjaxJsonAlertError(AjaxErrorText);
                else
                {
                    if (filterContext.Exception.Message.Contains("Adaptive: No data"))
                        filterContext.Result = AjaxHtmlAlertError(AjaxNoDataErrorText);
                    else if (filterContext.Exception.Message.Contains("GraphQL API"))
                        filterContext.Result = AjaxHtmlAlertError(filterContext.Exception.Message);
                    else
                        filterContext.Result = AjaxHtmlAlertError(filterContext.Exception.Message);
                }
            }
            else if (filterContext.Exception.Message.Contains("GetAllAssessments"))
                filterContext.Result = RedirectToAction(MVC.Error.ActionNames.ReportsNotAvailable, MVC.Error.Name);
            else
                filterContext.Result = RedirectToAction(MVC.Error.ActionNames.Index, MVC.Error.Name);

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
        }

        private string AjaxErrorText =>
            "An error occurred while processing your request. Please try again later or contact the <em>Elevate</em> Support Center.<br/>" +
            "1-877-246-8337 | <a href=\"mailto:help@riversidedatamanager.com\">help@riversidedatamanager.com</a><br/>" +
            "<em>Elevate Support Center hours are Monday - Friday from 7:00 AM - 6:00 PM (CST).</em>";

        private string AjaxNoDataErrorText =>
            "<em>Elevate</em> currently could not find any data. This could be because tests have not been completed as of yet. Please try again later or contact the <em>Elevate</em> Support Center.<br/>" +
            "1-877-246-8337 | <a href=\"mailto:help@riversidedatamanager.com\">help@riversidedatamanager.com</a><br/>" +
            "<em>Elevate Support Center hours are Monday - Friday from 7:00 AM - 6:00 PM (CST).</em>";
    }
}