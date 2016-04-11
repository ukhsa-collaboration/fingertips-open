using System.Configuration;

namespace FingertipsUploadService.Upload
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