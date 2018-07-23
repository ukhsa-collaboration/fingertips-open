using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.Parsers
{
    /// <summary>
    /// Parses a list of group ID strings, e.g. {"100001", "2000001,2030000"}
    /// </summary>
    public class GroupIdStringListParser
    {
        public IList<int> IntList { get; set; }

        public GroupIdStringListParser(IEnumerable<string> listOfIntListStrings)
        {
            var groupIdLists = ParseGroupIdLists(listOfIntListStrings);
            MergeGroupIdLists(groupIdLists);
            IntList = IntList.Distinct().ToList();
        }

        private static IEnumerable<List<int>> ParseGroupIdLists(IEnumerable<string> listOfIntListStrings)
        {
            IList<List<int>> groupIdLists = listOfIntListStrings
                .Select(idStrings => new IntListStringParser(idStrings).IntList)
                .ToList();
            return groupIdLists;
        }

        private void MergeGroupIdLists(IEnumerable<List<int>> groupIdLists)
        {
            var merged = new List<int>();
            foreach (var ids in groupIdLists)
            {
                merged.AddRange(ids);
            }

            IntList = merged;
        }
    }
}
