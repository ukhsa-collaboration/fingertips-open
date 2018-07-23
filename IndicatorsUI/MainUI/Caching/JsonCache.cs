using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using IndicatorsUI.DataAccess;

namespace IndicatorsUI.MainUI.Caching
{
    public class JsonCache
    {
        private JsonCacheManager cacheManager;

        private static byte[] GetFromWebCache(string key, Dictionary<string, byte[]> dictionary)
        {
            byte[] json = null;
            if (UseInMemoryCache)
            {
                dictionary.TryGetValue(key, out json);
            }
            return json;
        }

        private JsonCacheManager DatabaseCacheManager
        {
            get { return cacheManager ?? (cacheManager = new JsonCacheManager()); }
        }

        public void AddJson(JsonUnit jsonUnit)
        {
            if (jsonUnit.IsJsonOk())
            {
                if (UseDatabaseCache)
                {
                    DatabaseCacheManager.SaveJson(jsonUnit);
                }
                AddToWebCache(jsonUnit);
            }
        }

        private static void AddToWebCache(JsonUnit jsonUnit)
        {
            if (UseInMemoryCache)
            {
                Dictionary<string, byte[]> d = GetDictionary(jsonUnit.ServiceId);

                // Lock to prevent thread collisions, in .Net4 use ConcurrentDictionary
                lock (d)
                {
                    if (d.ContainsKey(jsonUnit.CacheKey) == false)
                    {
                        d.Add(jsonUnit.CacheKey, jsonUnit.Json);
                    }
                }
            }
        }

        private byte[] GetFromDatabaseCache(string serviceId, string cacheKey)
        {
            if (UseDatabaseCache)
            {
                JsonUnit jsonUnit = DatabaseCacheManager.ReadJson(serviceId, cacheKey);
                if (jsonUnit != null && jsonUnit.IsJsonOk())
                {
                    AddToWebCache(jsonUnit);
                    return jsonUnit.Json;
                }
            }
            return null;
        }

        public byte[] GetJson(string serviceId, string cacheKey)
        {
            return GetFromWebCache(cacheKey, GetDictionary(serviceId)) ??
                GetFromDatabaseCache(serviceId, cacheKey);
        }

        private static bool UseDatabaseCache
        {
            get { return AppConfig.Instance.UseDatabaseCaching; }
        }

        private static bool UseInMemoryCache
        {
            get { return AppConfig.Instance.UseInMemoryCaching; }
        }

        private static Dictionary<string, byte[]> GetDictionary(string serviceKey)
        {
            Dictionary<string, byte[]> o = HttpContext.Current.Cache[serviceKey] as Dictionary<string, byte[]>;
            if (o == null)
            {
                o = new Dictionary<string, byte[]>();
                HttpContext.Current.Cache.Add(serviceKey, o, null, CachePolicyHelper.Midnight/*expiry time*/, new TimeSpan(),
                                          CacheItemPriority.Normal, null);
            }
            return o;
        }
    }
}
