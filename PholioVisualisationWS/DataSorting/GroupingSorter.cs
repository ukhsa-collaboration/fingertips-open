using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSorting
{
    public class GroupingSorter
    {
        private readonly IList<Grouping> _groupings;

        public GroupingSorter(IList<Grouping> groupings)
        {
            _groupings = groupings;
        }

        public IList<Grouping> SortByDataPointTimePeriodMostRecentFirst()
        {
            return _groupings
                .OrderByDescending(x => x.DataPointYear)
                .ThenByDescending(x => x.DataPointQuarter)
                .ThenByDescending(x => x.DataPointMonth)
                .ToList();
        }

        public IList<Grouping> SortByBaseLineTimePeriodEarliestFirst()
        {
            return _groupings
                .OrderBy(x => x.BaselineYear)
                .ThenBy(x => x.BaselineQuarter)
                .ThenBy(x => x.BaselineMonth)
                .ToList();
        }

        public int GetMostCommonPolarityId()
        {
            var polarityIds = _groupings.Select(x => x.PolarityId);

            if (polarityIds.Any(x => x == PolarityIds.RagHighIsGood))
            {
                return PolarityIds.RagHighIsGood;
            }

            if (polarityIds.Any(x => x == PolarityIds.RagLowIsGood))
            {
                return PolarityIds.RagLowIsGood;
            }

            return IntListHelper.FindMostFrequentValue(polarityIds);
        }


    }
}