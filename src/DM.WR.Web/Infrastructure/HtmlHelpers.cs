using System.Web.Mvc;

namespace DM.WR.Web.Infrastructure
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString WrSpecialGroupLink(this HtmlHelper helper, string modalId, string labelId, string descriptionId)
        {
            var link = new TagBuilder("a");
            link.Attributes.Add("aria-labelledby", labelId);
            link.Attributes.Add("aria-describedby", descriptionId);
            link.AddCssClass("launch-group-modal");
            link.MergeAttribute("data-modal-id", modalId);
            link.MergeAttribute("tabindex", "0");
            link.SetInnerText("Change");

            return MvcHtmlString.Create(link.ToString(TagRenderMode.Normal));
        }
    }
}