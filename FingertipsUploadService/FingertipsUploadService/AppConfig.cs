using System.Configuration;

namespace FingertipsUploadService
{
    internal class AppConfig
    {
        public static string GetUploadFolder()
        {
            return GetAppSetting("UploadFolder");
        }

        private static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
