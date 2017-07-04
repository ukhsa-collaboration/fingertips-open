namespace PholioVisualisation.Export.File
{
    public class CacheFileNamer
    {
        public static string GetIndicatorFileName(int indicatorId, string parentAreaCode, int parentAreaTypeId,
            int childAreaTypeId, int profileId)
        {
            return string.Format("{0}-{1}-{2}-{3}-{4}.data.csv",
                profileId, parentAreaCode, parentAreaTypeId, childAreaTypeId, indicatorId);
        }

        public static string GetAddressFileName(string parentAreaCode, int areaTypeId)
        {
            return string.Format("{0}-{1}.addresses.csv",
                parentAreaCode, areaTypeId);
        }
    }
}