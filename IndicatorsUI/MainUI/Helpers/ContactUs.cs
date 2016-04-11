using System.Web;

namespace Profiles.MainUI.Helpers
{
    public class ContactUs
    {
        public ContactUs(string emailAddress, string urlKey)
        {
            var link =
                string.Format(
                    @"<a href=""mailto:{0}&subject=Profile%20Feedback%20[{1}]"" title=""If you would like to ask a question or submit feedback then please send us an email"">Contact Us</a>",
                    emailAddress, urlKey);

            Link = new HtmlString(link);
        }

        public HtmlString Link { get; private set; }
    }
}