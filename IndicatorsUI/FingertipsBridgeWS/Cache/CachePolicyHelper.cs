using System;
using System.Collections.Generic;

using System.Text;
using System.Web;

namespace FingertipsBridgeWS.Cache
{
    public class CachePolicyHelper
    {
        public static DateTime Midnight
        {
            get { return DateTime.UtcNow.AddDays(1).Date; }
        }

        public static void SetToBeCached(HttpCachePolicy cache)
        {
            if (AppConfiguration.UseClientCaching)
            {
                cache.SetCacheability(HttpCacheability.Public);
                cache.SetExpires(Midnight);
                cache.SetValidUntilExpires(true);
            }
        }
    }
}
