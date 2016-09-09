using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessages
{
    public enum DeprivationLevel
    {
        Low,
        Mid,
        High
    }

    public static class HealthProfilesKeyMessage1DeprivationLevel
    {
        public static DeprivationLevel GetDeprivationLevel(double deprivationValue, int areaTypeId)
        {
            if (areaTypeId == AreaTypeIds.County)
            {
                // Counties
                if (deprivationValue <= 15.014)
                {
                    return DeprivationLevel.Low;
                }

                if (deprivationValue >= 29.725)
                {
                    return DeprivationLevel.High;
                }
            }
            else
            {
                // Districts and UAs
                if (deprivationValue <= 12.108)
                {
                    return DeprivationLevel.Low;
                }

                if (deprivationValue >= 26.892)
                {
                    return DeprivationLevel.High;
                }
            }

            return DeprivationLevel.Mid;
        }
    }
}