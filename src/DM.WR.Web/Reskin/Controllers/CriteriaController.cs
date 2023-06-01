using DM.WR.BL.Providers;
using DM.WR.Web.ActionFilters;
using DM.WR.Web.Infrastructure;
using System.Web.Mvc;

namespace DM.WR.Web.Reskin.Controllers
{
    [RestrictAdaptiveAccess]
    public partial class CriteriaController : BaseController
    {
        private readonly ICriteriaProvider _provider;

        public CriteriaController(ICriteriaProvider provider, IWebsiteHelper websiteHelper) : base(websiteHelper)
        {
            _provider = provider;

            ViewBag.Tabs = NavigationHelper.Tabs(Models.TabsNavigationEnum.Criteria);
        }

        [HttpGet]
        public virtual ActionResult Index()
        {
            var model = _provider.BuildViewModel();
            return View(MVC.Criteria.Views.ViewNames.CriteriaPage, model);
        }

        [HttpPost]
        public virtual PartialViewResult GetCriteria(string assessmentCode, string displayType)
        {
            var model = _provider.BuildTableViewModel(assessmentCode, displayType);
            return PartialView(MVC.Criteria.Views.ViewNames._CriteriaTable, model);
        }

        [HttpPost]
        public virtual PartialViewResult LoadCriteria(int criteriaId, bool enableEditMode, string criteriaName, string criteriaDescription, string criteriaDate)
        {
            var invalidGroup = _provider.LoadCriteria(criteriaId, enableEditMode, criteriaName, criteriaDescription, criteriaDate);
            return PartialView(MVC.Shared.Views.dm_ui._Empty);
        }

        [HttpPost]
        public virtual PartialViewResult SaveCriteria(string name, string summary)
        {
            var message = _provider.SaveNewCriteria(name, summary);

            return message == "" ?
                AjaxHtmlAlertSuccess($"Your report criteria '{name}' has been successfully saved.") :
                AjaxHtmlAlertError(message);
        }

        [HttpPost]
        public virtual PartialViewResult UpdateCriteria(int criteriaId, string name, string summary)
        {
            var message = _provider.UpdateExistingCriteria(criteriaId, name, summary);

            return message == "" ?
                AjaxHtmlAlertSuccess("Your report criteria have been successfully updated.") :
                AjaxHtmlAlertError(message);
        }

        [HttpPost]
        public virtual PartialViewResult DeleteCriteria(int criteriaId)
        {
            _provider.DeleteCriteria(criteriaId);
            return AjaxHtmlAlertSuccess("Criteria has been successfully deleted.");
        }

        [HttpGet]
        public virtual PartialViewResult DisplayDeleteCriteriaModal()
        {
            var model = _provider.BuildDeleteCriteriaModal();
            return PartialView(MVC.Criteria.Views.ViewNames._DeleteCriteriaModal, model);
        }

        [HttpGet]
        public virtual PartialViewResult DisplayUnsavedChangesModal()
        {
            var model = _provider.BuildUnsavedChangesModal();
            return PartialView(MVC.Criteria.Views.ViewNames._UnsavedChangesModal, model);
        }

        [HttpGet]
        public virtual PartialViewResult DisplaySaveCriteriaModal()
        {
            var model = _provider.BuildSaveCriteriaModal();
            return PartialView(MVC.Criteria.Views.ViewNames._SaveCriteriaModal, model);
        }

        [HttpGet]
        public virtual PartialViewResult DisplayRunInBackgroundModal()
        {
            var model = _provider.BuildRunInBackgroundModal();
            return PartialView(MVC.Criteria.Views.ViewNames._RunInBackgroundModal, model);
        }
    }
}