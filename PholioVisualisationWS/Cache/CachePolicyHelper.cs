using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.Cache
{
    public class CachePolicyHelper
    {
        public static DateTime Midnight
        {
            get { return DateTime.UtcNow.AddDays(1).Date; }
        }

        public static DateTime OneWeekFromNow
        {
            get { return DateTime.UtcNow.AddDays(7); }
        }

        public static DateTime OneYearFromNow
        {
            get { return DateTime.UtcNow.AddYears(1); }
        }

        public static void SetMidnightWebCache(HttpResponse response)
        {
            if (ApplicationConfiguration.UseResponseCache)
            {
                response.ExpiresAbsolute = Midnight;
                response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
            }
        }

        public static void SetWeekWebCache(HttpResponse response)
        {
            if (ApplicationConfiguration.UseResponseCache)
            {
                response.ExpiresAbsolute = OneWeekFromNow;
                response.Cache.SetCacheability(HttpCacheability.Public);
            }
        }
    }
}
