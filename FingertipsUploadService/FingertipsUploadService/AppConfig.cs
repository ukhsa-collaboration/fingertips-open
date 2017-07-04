using System;
using System.Configuration;

namespace FingertipsUploadService
{
    internal class AppConfig
    {
        public static string GetUploadFolder()
        {
            return GetAppSetting("UploadFolder");
        }

        public static string GetAutoUploadFolder()
        {
            return GetAppSetting("AutoUploadPath");
        }

        public static string GetAutoUploadArchiveFolder()
        {
            return GetAppSetting("AutoUploadArchivePath");
        }

        public static int GetAutoUploadUserId()
        {
            return Convert.ToInt32(GetAppSetting("AutoUploadUserId"));
        }

        public static int GetAutoUploadPoolRate()
        {
            return Convert.ToInt32(GetAppSetting("AutoUploadPoolRate"));
        }

        private static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
