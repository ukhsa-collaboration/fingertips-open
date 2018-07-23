using System.Text.RegularExpressions;

namespace PholioVisualisation.ServicesWeb.Helpers
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