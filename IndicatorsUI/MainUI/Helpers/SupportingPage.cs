using System.Web;

namespace Profiles.MainUI.Helpers
{
    public class SupportingPage
    {
        public HtmlString Link { get; private set; }

        public SupportingPage(string profileKey, string contentKey, int profileId)
        {
            var href = string.Format("/profile/{0}/supporting-information/{1}",
                profileKey, contentKey);

            var content = ContentProvider.GetContentItem(contentKey,profileId);

            Link = new HtmlString("<a href=\"" + href + "\" >" + content.Description + "</a>");
        } 
    }
}