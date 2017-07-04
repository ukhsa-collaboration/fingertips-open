using System.Text.RegularExpressions;

namespace Profiles.MainUI.Helpers
{
    public class SsrsHelper
    {
        private string ssrsBaseUrl = "sqlpor01";
        private string currentBaseUrl = "localhost:5996/reports/ssrs/images";
        public string ChangeUrl(string report)
        {
            return Regex.Replace(report, ssrsBaseUrl, currentBaseUrl);
        }
    }
}
