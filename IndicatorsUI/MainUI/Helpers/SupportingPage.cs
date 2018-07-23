using System.Web;

namespace IndicatorsUI.MainUI.Helpers
{
    public class SupportingPage
    {
        public HtmlString Link { get; private set; }

        public SupportingPage(string profileKey, string contentKey, int profileId)
        {
            var href = string.Format("/profile/{0}/supporting-information/{1}",
                profileKey, contentKey);

            var content = ContentProvider.GetContentItem(contentKey,profileId);

            var link = content !=null 
                ? "<a href=\"" + href + "\" >" + content.Description + "</a>"
                : "";

            Link = new HtmlString(link);
        } 
    }
}