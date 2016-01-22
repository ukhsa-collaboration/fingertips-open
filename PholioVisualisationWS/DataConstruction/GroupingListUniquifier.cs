using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupingListUniquifier
    {
        private IEnumerable<Grouping> groupings;
        private List<string> ids;
        private List<Grouping> uniqueGroupings;

        public GroupingListUniquifier(IEnumerable<Grouping> groupings)
        {
            this.groupings = groupings;
        }

        public IList<Grouping> GetUniqueGroupings()
        {
            Reset();

            foreach (Grouping grouping in groupings)
            {
                string idString = GetGroupingIdString(grouping);
                if (GroupingIsUniqueSoFar(idString))
                {
                    uniqueGroupings.Add(grouping);
                    ids.Add(idString);
                }
            }

            return uniqueGroupings;
        }

        private void Reset()
        {
            ids = new List<string>();
            uniqueGroupings = new List<Grouping>();
        }

        private bool GroupingIsUniqueSoFar(string idString)
        {
            return ids.Contains(idString) == false;
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
            return sb.ToString();
        }

    }
}
