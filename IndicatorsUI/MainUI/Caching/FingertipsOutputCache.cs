using System.Web.Mvc;

namespace Profiles.MainUI.Caching
{
    public class FingertipsOutputCache : OutputCacheAttribute
    {
        public FingertipsOutputCache()
        {
            Duration = CacheHelper.StandardCacheDuration;
        }
    }
}
