
using System;
using System.Configuration;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public static class ApplicationConfiguration
    {
        public static bool UseServerInMemoryCache
        {
            get { return bool.Parse(ConfigurationManager.AppSettings["UseServerInMemoryCache"]); }
        }

        public static bool UseResponseCache
        {
            get { return bool.Parse(ConfigurationManager.AppSettings["UseResponseCache"]); }
        }

        public static bool UseFileCache
        {
            get { return bool.Parse(ConfigurationManager.AppSettings["UseFileCache"]); }
        }

        public static string ImagesDirectory
        {
            get { return ConfigurationManager.AppSettings["ImagesDirectory"]; }
        }

        public static string ExportFileDirectory
        {
            get { return ConfigurationManager.AppSettings["ExportFileDirectory"]; }
        }

        public static string SearchIndexDirectory
        {
            get { return ConfigurationManager.AppSettings["SearchIndexDirectory"]; }
        }

        public static string UrlUI
        {
            get { return ConfigurationManager.AppSettings["UrlUI"]; }
        }

        public static string CoreWsUrlForLogging
        {
            get { return ConfigurationManager.AppSettings["CoreWsUrlForLogging"]; }
        }
      
        public static string ApplicationName
        {
            get { return ConfigurationManager.AppSettings["ApplicationName"]; }
        }

        public static string ExceptionLogFilePath
        {
            get { return ConfigurationManager.AppSettings["ExceptionLogFilePath"]; }
        }

        public static bool IsEnvironmentLive
        {
            get
            {
                var env = ConfigurationManager.AppSettings["Environment"];
                return string.IsNullOrEmpty(env) == false &&
                    env.Equals(ApplicationEnvironments.Live);
            }
        }
    }
}