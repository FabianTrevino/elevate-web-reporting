using System.Web.Optimization;
using DM.WR.Web.Models;

namespace DM.WR.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //JavaScript
            bundles.Add(new ScriptBundle(BundlePath.JsOptionsPage).Include(
                Links.Bundles.Reskin.Scripts.Assets.jquery_ui_js,
                Links.Bundles.Reskin.Scripts.Assets.Common_js,
                Links.Bundles.Reskin.Scripts.Assets.Criteria_js,
                Links.Bundles.Reskin.Scripts.Assets.Options_js
            ));
            bundles.Add(new ScriptBundle(BundlePath.JsCriteriaPage).Include(
                Links.Bundles.Reskin.Scripts.Assets.Common_js,
                Links.Bundles.Reskin.Scripts.Assets.Criteria_js
            ));
            bundles.Add(new ScriptBundle(BundlePath.JsLibraryPage).Include(
                Links.Bundles.Reskin.Scripts.Assets.Common_js,
                Links.Bundles.Reskin.Scripts.Assets.Library_js,
                Links.Bundles.Reskin.Scripts.KendoUiBackground.js.Assets.kendo_custom_min_js
            ));

            bundles.Add(new ScriptBundle(BundlePath.JsDashboardIowaFlexPage).Include(
                Links.Bundles.Reskin.Scripts.Assets.d3_v5_min_js,
                Links.Bundles.Reskin.Scripts.Assets.kendo_custom_min_js,
                Links.Bundles.Reskin.Scripts.Assets.Common_js,
                Links.Bundles.Reskin.Scripts.Assets.Options_js,
                Links.Bundles.Reskin.Scripts.Assets.DashboardD3Charts_js,
                Links.Bundles.Reskin.Scripts.Assets.DashboardRandomJsonFunctions_js,
                Links.Bundles.Reskin.Scripts.Assets.Dashboard_js
            ));

            bundles.Add(new ScriptBundle(BundlePath.JsDashboardLongitudinalIowaFlexPage).Include(
                Links.Bundles.Reskin.Scripts.Assets.DashboardD3ChartsLongitudinal_js,
                Links.Bundles.Reskin.Scripts.Assets.DashboardRandomJsonFunctionsLongitudinal_js,
                Links.Bundles.Reskin.Scripts.Assets.DashboardPrintFunctions_js,
                Links.Bundles.Reskin.Scripts.Assets.DashboardLongitudinalWCAG_js,
                Links.Bundles.Reskin.Scripts.Assets.DashboardLongitudinal_js
            ));

            bundles.Add(new ScriptBundle(BundlePath.JsDashboardCogatPage).Include(
                Links.Bundles.Reskin.Scripts.Assets.d3_v5_min_js,
                Links.Bundles.Reskin.Scripts.Assets.kendo_custom_min_js,
                Links.Bundles.Reskin.Scripts.Assets.Common_js,
                Links.Bundles.Reskin.Scripts.Assets.Options_js,
                Links.Bundles.Reskin.Scripts.Assets.DashboardD3ChartsCogat_js,
                Links.Bundles.Reskin.Scripts.Assets.DashboardRandomJsonFunctionsCogat_js,
                Links.Bundles.Reskin.Scripts.Assets.DashboardCogat_js,
                Links.Bundles.Reskin.Scripts.Assets.DashBoardReportCenter_js,
                Links.Bundles.Reskin.Scripts.KendoUiBackground.js.Assets.kendo_custom_min_js
            ));

            bundles.Add(new ScriptBundle(BundlePath.JsIowaFlexStudentProfilePage).Include(
                Links.Bundles.Reskin.Scripts.Assets.DashboardStudentProfile_js
            ));

            bundles.Add(new ScriptBundle(BundlePath.JsCommonOnly).Include(
                Links.Bundles.Reskin.Scripts.Assets.Common_js
            ));


            //CSS
            bundles.Add(new StyleBundle(BundlePath.CssOptionsPage).Include(
                Links.Bundles.Reskin.Content.Assets.OptionsPage_css,
                Links.Bundles.Reskin.Content.Assets.ReportViewer_css

            ));
            bundles.Add(new StyleBundle(BundlePath.CssCriteriaPage).Include(
                Links.Bundles.Reskin.Content.Assets.CriteriaPage_css,
                Links.Bundles.Reskin.Content.Assets.ReportViewer_css
            ));
            bundles.Add(new StyleBundle(BundlePath.CssLibraryPage).Include(
                Links.Bundles.Reskin.Content.Assets.LibraryPage_css,
                Links.Bundles.Reskin.Content.Assets.ReportViewer_css,
                Links.Bundles.Reskin.Content.web.Assets.kendo_common_css,
                Links.Bundles.Reskin.Content.web.Assets.kendo_office365_css,
                Links.Bundles.Reskin.Content.web.Assets.kendo_office365_mobile_css,
                Links.Bundles.Reskin.Content.web.Assets.kendo_default_mobile_min_css

            ));
            bundles.Add(new StyleBundle(BundlePath.CssBackdoorPage).Include(
                Links.Bundles.Reskin.Content.Assets.BackDoor_css
            ));

            bundles.Add(new StyleBundle(BundlePath.CssDashboardIowaFlexPage).Include(
               Links.Bundles.Reskin.Scripts.KendoUi.styles.web.Assets.kendo_common_css,
               Links.Bundles.Reskin.Scripts.KendoUi.styles.web.Assets.kendo_office365_css,
               Links.Bundles.Reskin.Content.fontawesome_free_5_10_1_web.css.Assets.all_min_css,
               Links.Bundles.Reskin.Content.Assets.Dashboard_css
           ));

            bundles.Add(new StyleBundle(BundlePath.CssResponsiveDashboardIowaFlexPage).Include(
                Links.Bundles.Reskin.Content.Assets.DashboardResponsive_css
            ));

            bundles.Add(new StyleBundle(BundlePath.CssDashboardLongitudinalIowaFlexPage).Include(
                Links.Bundles.Reskin.Content.Assets.DashboardLongitudinal_css
            ));

            bundles.Add(new StyleBundle(BundlePath.CssDashboardCogatPage).Include(
                Links.Bundles.Reskin.Scripts.KendoUi.styles.web.Assets.kendo_common_css,
                Links.Bundles.Reskin.Scripts.KendoUi.styles.web.Assets.kendo_office365_css,
                Links.Bundles.Reskin.Content.fontawesome_free_5_10_1_web.css.Assets.all_min_css,
                Links.Bundles.Reskin.Content.web.Assets.kendo_default_mobile_min_css,
                Links.Bundles.Reskin.Content.Assets.DashboardCogat_css,
                Links.Bundles.Reskin.Content.Assets.Dashboard_CogAT_css
            ));

        }
    }
}