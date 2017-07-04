﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace Profiles.DataConstruction
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

        public FingertipsUrl(AppConfig appConfig, Uri uri)
        {
            if (appConfig.IsEnvironmentLive)
            {
                _environment = FingertipsEnvironment.Live;
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
                        /* DF - live used to be https but users preferentially directed to http
                         * to mitigate PHE network caching, especially of error pages  */
                        return "http://";
                    default:
                        return "http://";
                }
            }
        }

        private string GetHost(Skin coreSkin)
        {
            var host = _environment == FingertipsEnvironment.Live
                ? coreSkin.LiveHost
                : coreSkin.TestHost;
            return host;
        }
    }
}