using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.DataConstruction
{
    public enum FingertipsEnvironment
    {
        Development,
        Testing,
        Live
    }

    public class FingertipsUrl
    {
        private readonly FingertipsEnvironment _environment;
        private string _host;

        public FingertipsUrl(IAppConfig appConfig, Uri uri)
        {
            if (appConfig.IsEnvironmentLive)
            {
                _environment = FingertipsEnvironment.Live;

                // Use live-a.phe.org.uk or live-b.phe.org.uk when testing live site
                var host = uri.Host;
                if (host.StartsWith("live"))
                {
                    _host = host;
                }
            }
            else if (uri.Host.Contains("localhost"))
            {
                _environment = FingertipsEnvironment.Development;
            }
            else
            {
                _environment = FingertipsEnvironment.Testing;
            }
        }

        /// <summary>
        /// Hostname preceeded by protocol, e.g. "http://fingertips.phe.org.uk"
        /// </summary>
        public string ProtocolAndHost
        {
            get
            {
                if (_environment == FingertipsEnvironment.Development)
                {
                    return string.Empty;
                }

                var coreSkin = ReaderFactory.GetProfileReader().GetSkinFromId(SkinIds.Core);
                var host = GetHost(coreSkin);
                return Protocol + host;
            }
        }

        /// <summary>
        /// HTTP or HTTPS
        /// </summary>
        public string Protocol
        {
            get
            {
                switch (_environment)
                {
                    case FingertipsEnvironment.Testing:
                        return "https://";
                    case FingertipsEnvironment.Live:
                        return "https://";
                    default:
                        return "http://";
                }
            }
        }

        private string GetHost(Skin coreSkin)
        {
            string host;
            if (_environment == FingertipsEnvironment.Live)
            {
                host = _host ?? coreSkin.LiveHost;
            }
            else
            {
                host = coreSkin.TestHost;
            }
            return host;
        }
    }
}
