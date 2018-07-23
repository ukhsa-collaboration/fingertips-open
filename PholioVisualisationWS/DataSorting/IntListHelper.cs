using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataSorting
{
    public class IntListHelper
    {
        public static int FindMostFrequentValue(IEnumerable<int> values)
        {
            return values.GroupBy(x => x)
                .OrderByDescending(g => g.Count())
                .Take(1)
                .Select(g => g.Key).First();
        }
    }
}