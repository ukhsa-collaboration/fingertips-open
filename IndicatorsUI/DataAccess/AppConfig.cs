
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.DataAccess
{
    public interface IAppConfig
    {
        string BridgeCacheConnectionString { get; }
        string WebProxy { get; }
        bool IsContentCachedInMemory { get; }
        bool IsAccessControlToProfiles { get; }
        bool IsIndicatorSearchAvailable { get; }
        bool IsEnvironmentLive { get; }
        bool IsEnvironmentTest { get; }
        string Environment { get; }
        bool UseMinifiedJavaScript { get; }
        bool UseGoogleAnalytics { get; }
        bool UseDatabaseCaching { get; }
        bool UseInMemoryCaching { get; }
        string LongerLivesFrontPageProfileKey { get; }
        string ExceptionLogFilePath { get; }
        string SkinOverride { get; }
        string ApplicationName { get; }
        bool IsSkinOverride { get; }
        string BridgeWsUrl { get; }
        string DomainUrl { get; }
        string NotifyApikey { get; }
        string StaticContentUrl { get; }
        string JavaScriptVersionFolder { get; }
        string CoreWsUrlForAjaxBridge { get; }
        string CoreWsUrlForLogging { get; }
        string CoreWsUrl { get; }
        IList<string> ActiveFeatures { get; }
        bool ShowUpdateDelayedMessage { get; }

        /// <summary>
        /// Check whether a particular feature is active.
        /// </summary>
        bool IsFeatureActive(string featureSwitch);

        string EnsureUrlEndsWithForwardSlash(string url);
    }

    public class AppConfig : IAppConfig
    {
        public static NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }

        private NameValueCollection _appSettings;

        public static AppConfig Instance = new AppConfig(AppSettings);

        /// <summary>
        /// List of active features that can be switched on and off
        /// </summary>
        private IList<string> _featureFlags;

        public AppConfig(NameValueCollection appSettings)
        {
            _appSettings = appSettings;
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

        public string WebProxy
        {
            get { return GetAppSetting("WebProxy"); }
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

        public bool UseDatabaseCaching
        {
            get { return ParseBool("UseDatabaseCaching"); }
        }

        public bool UseInMemoryCaching
        {
            get { return ParseBool("UseInMemoryCaching"); }
        }

        public bool UseFileCaching
        {
            get { return ParseBool("UseFileCaching"); }
        }

        public string LongerLivesFrontPageProfileKey
        {
            get { return GetAppSetting("LongerLivesFrontPageProfileKey"); }
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

        public string DomainUrl
        {
            get
            {
                return GetAppSetting("DomainUrl");
            }
        }

        public string NotifyApikey
        {
            get
            {
                return GetAppSetting("NotifyApikey");
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

        public string StaticReportsDirectory
        {
            get { return GetAppSetting("StaticReportsDirectory"); }
        }

        /// <summary>
        /// Check whether a particular feature is active.
        /// </summary>
        public bool IsFeatureActive(string featureSwitch)
        {
            return ActiveFeatures.Contains(featureSwitch);
        }

        public IList<string> ActiveFeatures
        {
            get
            {
                if (_featureFlags == null)
                {
                    _featureFlags = ParseCommaSeparatedStringArray("ActiveFeatures");
                }
                return _featureFlags;
            }
        }

        public bool ShowUpdateDelayedMessage
        {
            get { return ParseBool("ShowUpdateDelayedMessage"); }
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
            return _appSettings[key];
        }

        public bool isAllowingRedirection
        {
            get
            {
                return !AppConfig.Instance.IsEnvironmentTest || ConfigurationManager.AppSettings["allowRedirectionInTestEnvironment"].ToLower() == "true";
            }
        }
    }
}
