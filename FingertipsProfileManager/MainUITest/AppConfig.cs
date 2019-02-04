using System;
using System.Configuration;
using System.IO;

namespace Fpm.MainUI
{
    public class AppConfig
    {
        private static string jsPath;
        private static string cssPath;
        private static string angularAppPath;
        private static string angularAppDistPath;



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

        public static string UploadFolder
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GetAppSetting("UploadFolder")); }
        }

        public static string JsPath
        {
            get { return jsPath ?? (jsPath = GetPath("js")); }
        }

        public static string CssPath
        {
            get { return cssPath ?? (cssPath = GetPath("css")); }
        }

        public static string AngularAppPath
        {
            get { return angularAppPath ?? (angularAppPath = GetPath("AngularApps")); }
        }

        public static string AngularAppDistPath
        {
            get { return angularAppDistPath ?? (angularAppDistPath = GetPath("angular-app-dist")); }
        }

        public static string GetPholioWs()
        {
            return GetAppSetting("CoreWsUrl");
        }

        public static string GetLiveSiteWsUrl()
        {
            return GetAppSetting("LiveSiteWsUrl");
        }

        public static string GetLiveUpdateKey()
        {
            return GetAppSetting("LiveUpdateKey");
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