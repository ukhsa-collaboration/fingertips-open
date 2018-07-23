using IndicatorsUI.DataAccess;

namespace IndicatorsUI.MainUI.Caching
{
    public static class CacheHelper
    {
        public static int StandardCacheDuration
        {
            get
            {
                // Only cache the output in the live environment
                if (AppConfig.Instance.IsEnvironmentLive)
                {
                    // 36000 = 10 hours, 60s x 60m x 10h
                    return 36000;
                }
                return 0;
            }
        }
    }
}