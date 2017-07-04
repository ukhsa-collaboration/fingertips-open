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
    }
}