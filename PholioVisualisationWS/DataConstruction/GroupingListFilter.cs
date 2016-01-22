using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
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
                return groupings.Where(g => areaTypeIds.Contains(g.AreaTypeId));
            }

            return groupings;
        }

        public IEnumerable<int> SelectDistinctComparatorIds()
        {
                return groupings.Select(x => x.ComparatorId).Distinct();
        }
    }
}
