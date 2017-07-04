using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace PholioVisualisation.Formatting
{
    public class HtmlCleaner
    {
        private Regex htmlRemover = new Regex(@"<[A-z /]+>");
        private Regex linkFinder = new Regex(@"<a[^>]+>[^<]+</a>", RegexOptions.IgnoreCase);
        private Regex hrefFinder = new Regex(".*href=[\"']([^\"']+)");

        public virtual string RemoveHtml(string text)
        {
            text = TransformLinks(text);

            // Remove non-breaking spaces
            text = text.Replace("&nbsp;", "");

            text = HttpUtility.HtmlDecode(text);
            return htmlRemover.Replace(text, string.Empty).Trim();
        }

        public virtual string TransformLinks(string text)
        {
            MatchCollection matches = linkFinder.Matches(text);

            if (matches.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                int firstIndex = 0;

                for (int i = 0; i < matches.Count; i++)
                {
                    Match match = matches[i];
                    string matchText = match.Value;
                    Match matchHref = hrefFinder.Match(matchText);
                    string url = matchHref.Groups[1].Value;

                    int length = match.Index - firstIndex;
                    sb.Append(text.Substring(firstIndex, length));
                    sb.Append(" "); /*to separate from adjacent links and text*/
                    sb.Append(url);
                    sb.Append(" "); /*to separate from adjacent links and text*/

                    firstIndex = match.Index + match.Length;
                }

                sb.Append(text.Substring(firstIndex));

                text = sb.ToString().Trim();
            }

            return text;
        }

    }
}
