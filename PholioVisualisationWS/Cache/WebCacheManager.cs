using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace PholioVisualisation.Cache
{
    public class WebCacheManager
    {
        private object lockObject = new object();

        public void ClearCache()
        {
            lock (lockObject)
            {
                ClearHttpContextCache();
            }
        }

        /// <summary>
        /// Clears the current HttpContext cache.
        /// </summary>
        private static void ClearHttpContextCache()
        {
            System.Web.Caching.Cache cache = HttpContext.Current.Cache;

            List<string> keys = new List<string>();
            IDictionaryEnumerator enumerator = cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }

            foreach (string key in keys)
            {
                cache.Remove(key);
            }
        }
    }
}
