using System.Configuration;

namespace Fpm.Upload
{
    internal class AppConfig
    {
        public static string GetPholioWs()
        {
            return GetAppSetting("CoreWsUrl");
        }

        private static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}