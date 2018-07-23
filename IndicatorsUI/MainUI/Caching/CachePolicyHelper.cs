using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndicatorsUI.MainUI.Caching
{
    public class CachePolicyHelper
    {
        public static DateTime Midnight
        {
            get { return DateTime.UtcNow.AddDays(1).Date; }
        }

        public static void CacheForOneMonth(HttpCachePolicyBase cache)
        {
            cache.SetExpires(DateTime.Now.AddMonths(1));
            cache.SetCacheability(HttpCacheability.Public);
            cache.SetValidUntilExpires(true);
        }

        public static void SetNoCaching(HttpCachePolicyBase cache)
        {
            // See https://stackoverflow.com/questions/1160105/disable-browser-cache-for-entire-asp-net-website
            cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            cache.SetValidUntilExpires(false);
            cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            cache.SetCacheability(HttpCacheability.NoCache);
            cache.SetNoStore();
        }
    }
}