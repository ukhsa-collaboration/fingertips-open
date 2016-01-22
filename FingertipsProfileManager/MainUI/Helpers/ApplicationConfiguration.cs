using System.Configuration;

namespace Fpm.MainUI.Helpers
{
    public static class ApplicationConfiguration
    {
        public static string SiteUrlForTesting
        {
            get { return ConfigurationManager.AppSettings["SiteUrlForTesting"]; }
        }

        public static string CoreWsUrl
        {
            get { return ConfigurationManager.AppSettings["CoreWsUrl"]; }
        }

        public static string CoreWsUrlForLogging
        {
            get { return ConfigurationManager.AppSettings["CoreWsUrlForLogging"]; }
        }
    }
}