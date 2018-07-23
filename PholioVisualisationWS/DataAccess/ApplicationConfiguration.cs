
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public class ApplicationConfiguration
    {
        public static NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }

        private NameValueCollection appSettings;

        public static ApplicationConfiguration Instance = new ApplicationConfiguration(AppSettings);

        public ApplicationConfiguration(NameValueCollection appSettings)
        {
            this.appSettings = appSettings;
        }

        /// <summary>
        /// Check whether a particular feature is active.
        /// </summary>
        public bool IsFeatureActive(string featureSwitch)
        {
            return ParseCommaSeparatedStringArray("ActiveFeatures").Contains(featureSwitch);
        }

        public bool UseServerInMemoryCache
        {
            get { return bool.Parse(GetAppSetting("UseServerInMemoryCache")); }
        }

        public bool UseResponseCache
        {
            get { return bool.Parse(GetAppSetting("UseResponseCache")); }
        }

        public bool UseFileCache
        {
            get { return bool.Parse(GetAppSetting("UseFileCache")); }
        }

        public string ImagesDirectory
        {
            get { return GetAppSetting("ImagesDirectory"); }
        }

        public string ExportFileDirectory
        {
            get { return GetAppSetting("ExportFileDirectory"); }
        }

        public string SearchIndexDirectory
        {
            get { return GetAppSetting("SearchIndexDirectory"); }
        }

        public string StaticReportsDirectory
        {
            get { return GetAppSetting("StaticReportsDirectory"); }
        }

        public string UrlUI
        {
            get { return GetAppSetting("UrlUI"); }
        }

        public string CoreWsUrlForLogging
        {
            get { return GetAppSetting("CoreWsUrlForLogging"); }
        }

        /// <summary>
        /// Only set on live
        /// </summary>
        public string BuildVersion
        {
            get { return GetAppSetting("BuildVersion"); }
        }

        public string ApplicationName
        {
            get { return GetAppSetting("ApplicationName"); }
        }

        public string ExceptionLogFilePath
        {
            get { return GetAppSetting("ExceptionLogFilePath"); }
        }

        public bool IsEnvironmentLive
        {
            get
            {
                var env = GetAppSetting("Environment");
                return string.IsNullOrEmpty(env) == false &&
                    env.Equals(ApplicationEnvironments.Live);
            }
        }

        public string GetLiveUpdateKey()
        {
            return GetAppSetting("LiveUpdateKey");
        }

        private IList<string> ParseCommaSeparatedStringArray(string key)
        {
            var configValue = GetAppSetting(key);

            if (string.IsNullOrWhiteSpace(configValue))
            {
                return new List<string>();
            }

            return configValue.Split(',');
        }

        private string GetAppSetting(string key)
        {
            return appSettings[key];
        }
    }
}