using System.Configuration;

namespace FingertipsUploadService.Upload
{
    internal class AppConfig
    {
        private static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}