using System.Configuration;

namespace Fpm.MainUI
{
    public class AppConfig
    {
        private static string jsPath;
        private static string cssPath;

        public static string LastUpdatedDateBatchTemplate
        {
            get { return GetAppSetting("LastUpdatedDateBatchTemplate"); }
        }

        public static string LastUpdatedDateSimpleTemplate
        {
            get { return GetAppSetting("LastUpdatedDateSimpleTemplate"); }
        }

        public static string ErrorFile
        {
            get { return GetAppSetting("ErrorFile"); }
        }

        public static string CurrentUserName
        {
            get { return GetAppSetting("CurrentUserName"); }
        }

        public static string DefaultTestUrl
        {
            get { return GetAppSetting("DefaultTestUrl"); }
        }

        public static string JsPath
        {
            get { return jsPath ?? (jsPath = GetPath("js")); }
        }

        public static string CssPath
        {
            get { return cssPath ?? (cssPath = GetPath("css")); }
        }

        private static string GetPath(string folderName)
        {
            string version = GetAppSetting("JavaScriptVersionFolder");

            return string.IsNullOrWhiteSpace(version)
                ? "/" + folderName + "/" // Running on dev machine
                : "/" + version.Trim() + "/" + folderName + "/";
        }

        private static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}