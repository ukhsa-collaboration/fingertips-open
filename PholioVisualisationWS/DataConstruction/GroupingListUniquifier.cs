using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupingListUniquifier
    {
        private readonly IEnumerable<Grouping> _groupings;
        private List<Grouping> _uniqueGroupings;

        public GroupingListUniquifier(IEnumerable<Grouping> groupings)
        {
            _groupings = groupings;
        }

        public IList<Grouping> GetUniqueGroupings()
        {
            _uniqueGroupings = new List<Grouping>();

            var idToGroupings = GetMapOfUniqueIdToGroupings();

            // Create unique groupings
            foreach (var idToGrouping in idToGroupings)
            {
                var groupings = idToGrouping.Value;
                var grouping = groupings.First();
                _uniqueGroupings.Add(grouping);

                if (groupings.Count > 1)
                {
                    var sortedGroupings = new GroupingSorter(groupings)
                        .SortByDataPointTimePeriodMostRecentFirst().ToList();

                    // Use overall earliest timepoint
                    var earliestGrouping = sortedGroupings.Last();
                    grouping.BaselineYear = earliestGrouping.BaselineYear;
                    grouping.BaselineQuarter = earliestGrouping.BaselineQuarter;
                    grouping.BaselineMonth = earliestGrouping.BaselineMonth;

                    // Use overall latest timepoint
                    var latestGrouping = sortedGroupings.First();
                    grouping.DataPointYear = latestGrouping.DataPointYear;
                    grouping.DataPointQuarter = latestGrouping.DataPointQuarter;
                    grouping.DataPointMonth = latestGrouping.DataPointMonth;

                    // Use most popular polarity
                    grouping.PolarityId = new GroupingSorter(groupings).GetMostCommonPolarityId();
                }
            }

            return _uniqueGroupings;
        }

        private Dictionary<string, List<Grouping>> GetMapOfUniqueIdToGroupings()
        {
            var idToGroupings = new Dictionary<string, List<Grouping>>();
            foreach (Grouping grouping in _groupings)
            {
                string idString = GetGroupingIdString(grouping);
                if (idToGroupings.ContainsKey(idString) == false)
                {
                    idToGroupings[idString] = new List<Grouping>();
                }

                idToGroupings[idString].Add(grouping);
            }
            return idToGroupings;
        }

        private static string GetGroupingIdString(Grouping grouping)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(grouping.IndicatorId);
            sb.Append("-");
            sb.Append(grouping.SexId);
            sb.Append("-");
            sb.Append(grouping.AgeId);
            sb.Append("-");
            sb.Append(grouping.ComparatorId);
            sb.Append("-");
            sb.Append(grouping.AreaTypeId);
            sb.Append("-");
            sb.Append(grouping.YearRange);
            return sb.ToString();
        }

    }
}
