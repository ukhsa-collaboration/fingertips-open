using System;
using System.Collections.Generic;
using System.Configuration;

namespace FingertipsBridgeWS.Cache
{
    public static class AppConfiguration
    {
        public static string BridgeCacheConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["BridgeCacheConnectionString"];

                return connectionString != null ?
                    connectionString.ConnectionString :
                    null;
            }
        }

        public static string CoreWsUrlForLogging
        {
            get
            {
                // May need to call a different URL on server than from the client
                var url = ConfigurationManager.AppSettings["CoreWsUrlForLogging"];

                if (string.IsNullOrEmpty(url))
                {
                    url = ConfigurationManager.AppSettings["CoreWsUrl"];
                }

                return url.TrimEnd('/');
            }
        }

        public static string CoreWsUrlForAjaxBridge
        {
            get
            {
                // May need to call a different URL on server than from the client
                var url = ConfigurationManager.AppSettings["CoreWsUrlForAjaxBridge"];

                if (string.IsNullOrEmpty(url))
                {
                    url = ConfigurationManager.AppSettings["CoreWsUrl"];
                }

                return url.TrimEnd('/');
            }
        }

        public static string UserAgent
        {
            get { return ConfigurationManager.AppSettings["UserAgent"]; }
        }

        public static bool UseClientCaching
        {
            get { return bool.Parse(ConfigurationManager.AppSettings["UseClientCaching"]); }
        }

        public static bool UseDatabaseCaching
        {
            get { return bool.Parse(ConfigurationManager.AppSettings["UseDatabaseCaching"]); }
        }

        public static bool UseInMemoryCaching
        {
            get { return bool.Parse(ConfigurationManager.AppSettings["UseInMemoryCaching"]); }
        }

    }
}