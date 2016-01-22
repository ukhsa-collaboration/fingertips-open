
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class LimitsBuilder
    {
        public bool ExcludeComparators { get; set; }

        private List<string> areaCodes;

        public Limits GetLimits(IList<string> childAreaCodes, Grouping grouping, ComparatorMap comparatorMap)
        {
            areaCodes = new List<string>();

            AddRegionAndChildAreas(childAreaCodes, comparatorMap);
            AddNationalArea(comparatorMap);

            return GetLimitsOfSpecifiedAreas(grouping);
        }

        private void AddRegionAndChildAreas(IList<string> childAreaCodes, ComparatorMap comparatorMap)
        {
            Comparator regionalComparator = comparatorMap.GetSubnationalComparator();
            if (regionalComparator != null)
            {
                areaCodes.AddRange(childAreaCodes);

                if (ExcludeComparators == false)
                {
                    areaCodes.Add(regionalComparator.Area.Code);
                }
            }
        }

        private Limits GetLimitsOfSpecifiedAreas(Grouping grouping)
        {
            TrendDataReader trendReader = ReaderFactory.GetTrendDataReader();

            double? min = trendReader.GetMin(grouping, areaCodes);
            double? max = trendReader.GetMax(grouping, areaCodes);
            return (min.HasValue && max.HasValue)
                ? new MinMaxRounder(min.Value, max.Value).Limits
                : null;
        }

        private void AddNationalArea(ComparatorMap comparatorMap)
        {
            if (ExcludeComparators == false)
            {
                Comparator nationalComparator = comparatorMap.GetNationalComparator();
                if (nationalComparator != null)
                {
                    areaCodes.Add(nationalComparator.Area.Code);
                }
            }
        }
    }
}
