using System.Text.RegularExpressions;

namespace ServicesWeb.Helpers
{
    public class UrlHelper
    {
        private static Regex _regex = new Regex("^https*://");

        public static string TrimProtocol(string url)
        {
            return _regex.Replace(url, "");
        }
    }
}