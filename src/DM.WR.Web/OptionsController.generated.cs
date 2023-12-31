// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments and CLS compliance
// 0108: suppress "Foo hides inherited member Foo. Use the new keyword if hiding was intended." when a controller and its abstract parent are both processed
// 0114: suppress "Foo.BarController.Baz()' hides inherited member 'Qux.BarController.Baz()'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword." when an action (with an argument) overrides an action in a parent controller
#pragma warning disable 1591, 3008, 3009, 0108, 0114
#region T4MVC

using System;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using T4MVC;
namespace DM.WR.Web.Reskin.Controllers
{
    public partial class OptionsController
    {
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected OptionsController(Dummy d) { }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoute(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToAction(Task<ActionResult> taskResult)
        {
            return RedirectToAction(taskResult.Result);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(ActionResult result)
        {
            var callInfo = result.GetT4MVCResult();
            return RedirectToRoutePermanent(callInfo.RouteValueDictionary);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        protected RedirectToRouteResult RedirectToActionPermanent(Task<ActionResult> taskResult)
        {
            return RedirectToActionPermanent(taskResult.Result);
        }

        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.PartialViewResult SwitchLocation()
        {
            return new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.SwitchLocation);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.PartialViewResult GetGroup()
        {
            return new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.GetGroup);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.JsonResult UpdateOptions()
        {
            return new T4MVC_System_Web_Mvc_JsonResult(Area, Name, ActionNames.UpdateOptions);
        }
        [NonAction]
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public virtual System.Web.Mvc.PartialViewResult GoToMultimeasureColumn()
        {
            return new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.GoToMultimeasureColumn);
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public OptionsController Actions { get { return MVC.Options; } }
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Area = "";
        [GeneratedCode("T4MVC", "2.0")]
        public readonly string Name = "Options";
        [GeneratedCode("T4MVC", "2.0")]
        public const string NameConst = "Options";
        [GeneratedCode("T4MVC", "2.0")]
        static readonly ActionNamesClass s_actions = new ActionNamesClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionNamesClass ActionNames { get { return s_actions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNamesClass
        {
            public readonly string Index = "Index";
            public readonly string DisplayLocationChangeModal = "DisplayLocationChangeModal";
            public readonly string SwitchLocation = "SwitchLocation";
            public readonly string GetOptions = "GetOptions";
            public readonly string GetGroup = "GetGroup";
            public readonly string UpdateOptions = "UpdateOptions";
            public readonly string ResetOptions = "ResetOptions";
            public readonly string AddMultimeasureColumn = "AddMultimeasureColumn";
            public readonly string DeleteMultimeasureColumn = "DeleteMultimeasureColumn";
            public readonly string GoToMultimeasureColumn = "GoToMultimeasureColumn";
            public readonly string ClearEditMode = "ClearEditMode";
        }

        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionNameConstants
        {
            public const string Index = "Index";
            public const string DisplayLocationChangeModal = "DisplayLocationChangeModal";
            public const string SwitchLocation = "SwitchLocation";
            public const string GetOptions = "GetOptions";
            public const string GetGroup = "GetGroup";
            public const string UpdateOptions = "UpdateOptions";
            public const string ResetOptions = "ResetOptions";
            public const string AddMultimeasureColumn = "AddMultimeasureColumn";
            public const string DeleteMultimeasureColumn = "DeleteMultimeasureColumn";
            public const string GoToMultimeasureColumn = "GoToMultimeasureColumn";
            public const string ClearEditMode = "ClearEditMode";
        }


        static readonly ActionParamsClass_SwitchLocation s_params_SwitchLocation = new ActionParamsClass_SwitchLocation();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_SwitchLocation SwitchLocationParams { get { return s_params_SwitchLocation; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_SwitchLocation
        {
            public readonly string id = "id";
        }
        static readonly ActionParamsClass_GetGroup s_params_GetGroup = new ActionParamsClass_GetGroup();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GetGroup GetGroupParams { get { return s_params_GetGroup; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GetGroup
        {
            public readonly string groupType = "groupType";
        }
        static readonly ActionParamsClass_UpdateOptions s_params_UpdateOptions = new ActionParamsClass_UpdateOptions();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_UpdateOptions UpdateOptionsParams { get { return s_params_UpdateOptions; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_UpdateOptions
        {
            public readonly string groupType = "groupType";
            public readonly string values = "values";
        }
        static readonly ActionParamsClass_GoToMultimeasureColumn s_params_GoToMultimeasureColumn = new ActionParamsClass_GoToMultimeasureColumn();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ActionParamsClass_GoToMultimeasureColumn GoToMultimeasureColumnParams { get { return s_params_GoToMultimeasureColumn; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ActionParamsClass_GoToMultimeasureColumn
        {
            public readonly string columnNumber = "columnNumber";
        }
        static readonly ViewsClass s_views = new ViewsClass();
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public ViewsClass Views { get { return s_views; } }
        [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
        public class ViewsClass
        {
            static readonly _ViewNamesClass s_ViewNames = new _ViewNamesClass();
            public _ViewNamesClass ViewNames { get { return s_ViewNames; } }
            public class _ViewNamesClass
            {
                public readonly string _Group_Checkbox = "_Group_Checkbox";
                public readonly string _Group_CustomDataFields = "_Group_CustomDataFields";
                public readonly string _Group_LongitudinalTestAdministrations = "_Group_LongitudinalTestAdministrations";
                public readonly string _Group_PerformanceBands = "_Group_PerformanceBands";
                public readonly string _Group_PiledOptions = "_Group_PiledOptions";
                public readonly string _Group_ScoreFilters = "_Group_ScoreFilters";
                public readonly string _Group_ScoreWarnings = "_Group_ScoreWarnings";
                public readonly string _LocationChangeModal = "_LocationChangeModal";
                public readonly string _Options = "_Options";
                public readonly string _Options_TopToBottom = "_Options_TopToBottom";
                public readonly string _OptionsGroupSwitch = "_OptionsGroupSwitch";
                public readonly string OptionsPage = "OptionsPage";
            }
            public readonly string _Group_Checkbox = "~/Reskin/Views/Options/_Group_Checkbox.cshtml";
            public readonly string _Group_CustomDataFields = "~/Reskin/Views/Options/_Group_CustomDataFields.cshtml";
            public readonly string _Group_LongitudinalTestAdministrations = "~/Reskin/Views/Options/_Group_LongitudinalTestAdministrations.cshtml";
            public readonly string _Group_PerformanceBands = "~/Reskin/Views/Options/_Group_PerformanceBands.cshtml";
            public readonly string _Group_PiledOptions = "~/Reskin/Views/Options/_Group_PiledOptions.cshtml";
            public readonly string _Group_ScoreFilters = "~/Reskin/Views/Options/_Group_ScoreFilters.cshtml";
            public readonly string _Group_ScoreWarnings = "~/Reskin/Views/Options/_Group_ScoreWarnings.cshtml";
            public readonly string _LocationChangeModal = "~/Reskin/Views/Options/_LocationChangeModal.cshtml";
            public readonly string _Options = "~/Reskin/Views/Options/_Options.cshtml";
            public readonly string _Options_TopToBottom = "~/Reskin/Views/Options/_Options_TopToBottom.cshtml";
            public readonly string _OptionsGroupSwitch = "~/Reskin/Views/Options/_OptionsGroupSwitch.cshtml";
            public readonly string OptionsPage = "~/Reskin/Views/Options/OptionsPage.cshtml";
        }
    }

    [GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
    public partial class T4MVC_OptionsController : DM.WR.Web.Reskin.Controllers.OptionsController
    {
        public T4MVC_OptionsController() : base(Dummy.Instance) { }

        [NonAction]
        partial void IndexOverride(T4MVC_System_Web_Mvc_ActionResult callInfo);

        [NonAction]
        public override System.Web.Mvc.ActionResult Index()
        {
            var callInfo = new T4MVC_System_Web_Mvc_ActionResult(Area, Name, ActionNames.Index);
            IndexOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void DisplayLocationChangeModalOverride(T4MVC_System_Web_Mvc_PartialViewResult callInfo);

        [NonAction]
        public override System.Web.Mvc.PartialViewResult DisplayLocationChangeModal()
        {
            var callInfo = new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.DisplayLocationChangeModal);
            DisplayLocationChangeModalOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void SwitchLocationOverride(T4MVC_System_Web_Mvc_PartialViewResult callInfo, int id);

        [NonAction]
        public override System.Web.Mvc.PartialViewResult SwitchLocation(int id)
        {
            var callInfo = new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.SwitchLocation);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "id", id);
            SwitchLocationOverride(callInfo, id);
            return callInfo;
        }

        [NonAction]
        partial void GetOptionsOverride(T4MVC_System_Web_Mvc_PartialViewResult callInfo);

        [NonAction]
        public override System.Web.Mvc.PartialViewResult GetOptions()
        {
            var callInfo = new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.GetOptions);
            GetOptionsOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void GetGroupOverride(T4MVC_System_Web_Mvc_PartialViewResult callInfo, string groupType);

        [NonAction]
        public override System.Web.Mvc.PartialViewResult GetGroup(string groupType)
        {
            var callInfo = new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.GetGroup);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "groupType", groupType);
            GetGroupOverride(callInfo, groupType);
            return callInfo;
        }

        [NonAction]
        partial void UpdateOptionsOverride(T4MVC_System_Web_Mvc_JsonResult callInfo, string groupType, System.Collections.Generic.List<string> values);

        [NonAction]
        public override System.Web.Mvc.JsonResult UpdateOptions(string groupType, System.Collections.Generic.List<string> values)
        {
            var callInfo = new T4MVC_System_Web_Mvc_JsonResult(Area, Name, ActionNames.UpdateOptions);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "groupType", groupType);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "values", values);
            UpdateOptionsOverride(callInfo, groupType, values);
            return callInfo;
        }

        [NonAction]
        partial void ResetOptionsOverride(T4MVC_System_Web_Mvc_PartialViewResult callInfo);

        [NonAction]
        public override System.Web.Mvc.PartialViewResult ResetOptions()
        {
            var callInfo = new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.ResetOptions);
            ResetOptionsOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void AddMultimeasureColumnOverride(T4MVC_System_Web_Mvc_PartialViewResult callInfo);

        [NonAction]
        public override System.Web.Mvc.PartialViewResult AddMultimeasureColumn()
        {
            var callInfo = new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.AddMultimeasureColumn);
            AddMultimeasureColumnOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void DeleteMultimeasureColumnOverride(T4MVC_System_Web_Mvc_PartialViewResult callInfo);

        [NonAction]
        public override System.Web.Mvc.PartialViewResult DeleteMultimeasureColumn()
        {
            var callInfo = new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.DeleteMultimeasureColumn);
            DeleteMultimeasureColumnOverride(callInfo);
            return callInfo;
        }

        [NonAction]
        partial void GoToMultimeasureColumnOverride(T4MVC_System_Web_Mvc_PartialViewResult callInfo, int columnNumber);

        [NonAction]
        public override System.Web.Mvc.PartialViewResult GoToMultimeasureColumn(int columnNumber)
        {
            var callInfo = new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.GoToMultimeasureColumn);
            ModelUnbinderHelpers.AddRouteValues(callInfo.RouteValueDictionary, "columnNumber", columnNumber);
            GoToMultimeasureColumnOverride(callInfo, columnNumber);
            return callInfo;
        }

        [NonAction]
        partial void ClearEditModeOverride(T4MVC_System_Web_Mvc_PartialViewResult callInfo);

        [NonAction]
        public override System.Web.Mvc.PartialViewResult ClearEditMode()
        {
            var callInfo = new T4MVC_System_Web_Mvc_PartialViewResult(Area, Name, ActionNames.ClearEditMode);
            ClearEditModeOverride(callInfo);
            return callInfo;
        }

    }
}

#endregion T4MVC
#pragma warning restore 1591, 3008, 3009, 0108, 0114
