
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using Profiles.DomainObjects;

namespace Profiles.DataAccess
{
    public class AppConfig
    {
        public static NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }

        private NameValueCollection appSettings;

        public static AppConfig Instance = new AppConfig(AppSettings);

        public AppConfig(NameValueCollection appSettings)
        {
            this.appSettings = appSettings;
        }

        public string BridgeCacheConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["BridgeCacheConnectionString"];

                return connectionString != null ?
                    connectionString.ConnectionString :
                    null;
            }
        }

        public bool IsContentCachedInMemory
        {
            get { return ParseBool("IsContentCachedInMemory"); }
        }

        public bool IsAccessControlToProfiles
        {
            get { return ParseBool("IsAccessControlToProfiles"); }
        }

        public bool IsIndicatorSearchAvailable
        {
            get
            {
                // Search should be available by default
                return ParseBool("IsIndicatorSearchAvailable", true);
            }
        }
        
        public bool IsEnvironmentLive
        {
            get
            {
                return string.IsNullOrEmpty(Environment) == false &&
                       Environment.Equals("Live");
            }
        }

        public bool IsEnvironmentTest
        {
            get
            {
                return IsEnvironmentLive == false;
            }
        }

        public string Environment
        {
            get
            {
                TextInfo ti = new CultureInfo("en-GB", false).TextInfo;
                return ti.ToTitleCase(GetAppSetting("Environment"));
            }
        }

        public bool UseMinifiedJavaScript
        {
            get { return ParseBool("UseMinifiedJavaScript"); }
        }

        public bool UseGoogleAnalytics
        {
            get { return ParseBool("UseGoogleAnalytics"); }
        }

        public bool ShowCancer
        {
            get { return ParseBool("ShowCancer"); }
        }

        public bool UseDatabaseCaching
        {
            get { return ParseBool("UseDatabaseCaching"); }
        }

        public bool UseInMemoryCaching
        {
            get { return ParseBool("UseInMemoryCaching"); }
        }

        public string ExceptionLogFilePath
        {
            get { return GetAppSetting("ExceptionLogFilePath"); }
        }

        public string SkinOverride
        {
            get { return GetAppSetting("SkinOverride"); }
        }

        public string ApplicationName
        {
            get { return GetAppSetting("ApplicationName"); }
        }

        public bool IsSkinOverride
        {
            get
            {
                return string.IsNullOrEmpty(SkinOverride) == false;
            }
        }

        public string BridgeWsUrl
        {
            get
            {
                return GetAppSetting("BridgeWsUrl");
            }
        }

        public string StaticContentUrl
        {
            get
            {
                var s = GetAppSetting("StaticContentUrl");

                return string.IsNullOrEmpty(s) ?
                    "/" :
                    EnsureUrlEndsWithForwardSlash(s);
            }
        }

        public string JavaScriptVersionFolder
        {
            get
            {
                const string parameterName = "JavaScriptVersionFolder";
                var s = GetAppSetting(parameterName);
                CheckNotNull(s, parameterName);
                return s;
            }
        }

        private static void CheckNotNullOrWhiteSpace(string s, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new FingertipsException(parameterName + " is not defined");
            }
        }

        private static void CheckNotNull(string s, string parameterName)
        {
            if (s == null)
            {
                throw new FingertipsException(parameterName + " is not defined");
            }
        }

        public string CoreWsUrlForAjaxBridge
        {
            get
            {
                return GetAlternativeCoreWsUrl("CoreWsUrlForAjaxBridge");
            }
        }

        public string CoreWsUrlForLogging
        {
            get
            {
                return GetAlternativeCoreWsUrl("CoreWsUrlForLogging");
            }
        }

        /// <summary>
        /// May need to call a different URL on server than from the client for
        /// specific purposes.
        /// </summary>
        private string GetAlternativeCoreWsUrl(string parameterName)
        {
            var s = GetAppSetting(parameterName);
            return string.IsNullOrWhiteSpace(s) ?
                CoreWsUrl :
                EnsureUrlEndsWithForwardSlash(s);
        }

        public string CoreWsUrl
        {
            get
            {
                const string parameterName = "CoreWsUrl";
                var s = GetAppSetting(parameterName);
                CheckNotNullOrWhiteSpace(s, parameterName);
                return EnsureUrlEndsWithForwardSlash(s);
            }
        }

        public string PdfUrl
        {
            get
            {
                return EnsureUrlEndsWithForwardSlash(GetAppSetting("PdfUrl"));
            }
        }

        public string FeatureSwitch
        {
            get { return GetAppSetting("FeatureSwitch"); }
        }

        public bool ShowUpdateDelayedMessage
        {
            get { return ParseBool("ShowUpdateDelayedMessage"); }
        }

        public bool ShowLongerLivesSuicidePrevention
        {
            get { return ParseBool("ShowLongerLivesSuicidePrevention"); }
        }

        public string EnsureUrlEndsWithForwardSlash(string url)
        {
             return url.TrimEnd('/') + "/";
        }

        
        /// <summary>
        /// Parses "true" or "false". If the setting is not defined then it will default to false.
        /// </summary>
        private bool ParseBool(string keyString, bool? valueIfParameterNotDefined = null)
        {
            var key = GetAppSetting(keyString);

            if (string.IsNullOrWhiteSpace(key))
            {
                return valueIfParameterNotDefined ?? false;
            }

            return bool.Parse(key);
        }

        private string GetAppSetting(string key)
        {
            return appSettings[key];
        }
    }
}
