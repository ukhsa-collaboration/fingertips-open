using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace Profiles.DataConstruction
{
    public class FingertipsUrl
    {
        private AppConfig appConfig;

        public FingertipsUrl(AppConfig appConfig)
        {
            this.appConfig = appConfig;
        }

        public string Host
        {
            get
            {
                var isEnvironmentLive = appConfig.IsEnvironmentLive;

                var isEnvironmentDevelopment = appConfig.BridgeWsUrl.ToLower().Contains("localhost") &&
                    isEnvironmentLive == false;

                if (isEnvironmentDevelopment)
                {
                    return string.Empty;
                }

                var coreSkin = ReaderFactory.GetProfileReader().GetSkinFromId(SkinIds.Core);

                var host = isEnvironmentLive
                    ? coreSkin.LiveHost
                    : coreSkin.TestHost;

                return Protocol + host;
            }
        }

        public string Protocol
        {
            get
            {
                var protocol = appConfig.IsSecureConnection ? "https" : "http";
                protocol += "://";
                return protocol;
            }
        }
    }
}
