using System.Collections.Generic;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public static class CategoryTypeIdsExcludedForExport
    {
        public static List<int> Ids = new List<int>
            {
                CategoryTypeIds.LsoaDeprivationDecilesWithinArea2010,
                CategoryTypeIds.LimitsForHealthProfilesLifeExpectancyChart
            };
    }
}