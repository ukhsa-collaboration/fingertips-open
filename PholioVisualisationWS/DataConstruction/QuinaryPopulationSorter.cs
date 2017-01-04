
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class QuinaryPopulationSorter
    {
        /// <summary>
        /// Values sorted by age ID: youngest first, oldest last
        /// </summary>
        public IList<double> SortedValues { get; private set; }

        public QuinaryPopulationSorter(IEnumerable<CoreDataSet> data)
        {
            IEnumerable<int> ageIds = GetAgeIdsToOver95();
            IEnumerable<CoreDataSet> sortedData = ageIds
                .Select(ageId => data.FirstOrDefault(x => x.AgeId == ageId));

            SortedValues = sortedData.Where(x => x != null &&  x.Value != ValueData.NullValue).Select(x => x.Value).ToList();
        }

        public QuinaryPopulationSorter(IEnumerable<QuinaryPopulationValue> data)
        {
            IEnumerable<QuinaryPopulationValue> sortedData = GetAgeIdsToOver95()
                .Select(ageId => data.FirstOrDefault(x => x.AgeId == ageId));

            SortedValues = sortedData.Where(x => x != null && x.Value != ValueData.NullValue).Select(x => x.Value).ToList();
        }

        public static IEnumerable<int> GetAgeIdsToOver95()
        {
            List<int> ageIds = new List<int> { AgeIds.From0To4 };
            for (int ageId = AgeIds.From5To9; ageId <= AgeIds.From80To84; ageId++)
            {
                ageIds.Add(ageId);
            }
            ageIds.Add(AgeIds.From85To89);
            ageIds.Add(AgeIds.From90To95);
            ageIds.Add(AgeIds.Over95);
            return ageIds;
        }
    }
}
