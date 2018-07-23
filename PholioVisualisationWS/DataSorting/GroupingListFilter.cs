using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSorting
{
    public class GroupingListFilter
    {
        private IEnumerable<Grouping> groupings;

        public GroupingListFilter(IEnumerable<Grouping> groupings)
        {
            this.groupings = groupings.ToList();
        }

        /// <summary>
        /// Selects groupings that matches any of the specified area type IDs.
        /// </summary>
        public IEnumerable<Grouping> SelectForAreaTypeIds(IList<int> areaTypeIds)
        {
            if (areaTypeIds != null)
            {
                return groupings.Where(x => areaTypeIds.Contains(x.AreaTypeId));
            }

            return groupings;
        }

        public IEnumerable<Grouping> SelectForIndicatorIds(IList<int> indicatorIds)
        {
            if (indicatorIds != null)
            {
                return groupings.Where(x => indicatorIds.Contains(x.IndicatorId));
            }

            return groupings;
        }

        public IEnumerable<int> SelectDistinctComparatorIds()
        {
                return groupings.Select(x => x.ComparatorId).Distinct();
        }
    }
}
