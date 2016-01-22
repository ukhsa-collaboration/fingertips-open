using System.Web.Mvc;
using Imap.ProfileData;

namespace Imap.MainUI.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString ActionImage(this HtmlHelper html, string action, string imagePath, string alt, int groupId, int selectedDomain)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", url.Content(imagePath));
            imgBuilder.MergeAttribute("alt", alt);
            imgBuilder.AddCssClass("reorderIndicator");
            imgBuilder.Attributes.Add("selectedDomain", selectedDomain.ToString());
            string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);
            return MvcHtmlString.Create(imgHtml);
        }
    }
}