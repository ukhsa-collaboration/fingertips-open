using System.Web.Mvc;

namespace IndicatorsUI.MainUI.Caching
{
    public class FingertipsOutputCache : OutputCacheAttribute
    {
        public FingertipsOutputCache()
        {
            Duration = CacheHelper.StandardCacheDuration;
        }
    }
}
