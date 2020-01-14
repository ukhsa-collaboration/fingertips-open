using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataSorting
{
    public class QuinaryPopulationSorter
    {
        /// <summary>
        /// Population percentages sorted by age ID: youngest first, oldest last
        /// </summary>
        public IList<double> SortedValues { get; private set; }

        public IList<CoreDataSet> SortedData { get; private set; }

        public IList<int> SortedAgeIds { get; private set; }

        public QuinaryPopulationSorter(IList<CoreDataSet> data)
        {
            SortedAgeIds = data.Any(x => x.AgeId == AgeIds.Over95)
                ? GetAgeIdsToOver95()
                : GetAgeIdsToOver90();

            SortedData = SortedAgeIds
                .Select(ageId => data.FirstOrDefault(x => x.AgeId == ageId))
                .Where(x => x != null)
                .ToList();

            SortedValues = SortedData
                .Where(x => x != null && x.Value != ValueData.NullValue)
                .Select(x => x.Value)
                .ToList();
        }

        public QuinaryPopulationSorter(IList<QuinaryPopulationValue> data)
        {
            SortedAgeIds = data.Any(x => x.AgeId == AgeIds.Over95)
                ? GetAgeIdsToOver95()
                : GetAgeIdsToOver90();

            IEnumerable<QuinaryPopulationValue> sortedData = SortedAgeIds
                .Select(ageId => data.FirstOrDefault(x => x.AgeId == ageId));

            SortedValues = sortedData
                .Where(x => x != null && x.Value != ValueData.NullValue)
                .Select(x => x.Value)
                .ToList();
        }

        public static IList<int> GetAgeIdsToOver95()
        {
            var ageIds = GetAgeIdsUpTo89();
            ageIds.Add(AgeIds.From90To94);
            ageIds.Add(AgeIds.Over95);
            return ageIds;
        }

        public static IList<int> GetAgeIdsToOver90()
        {
            var ageIds = GetAgeIdsUpTo89();
            ageIds.Add(AgeIds.Over90);
            return ageIds;
        }

        private static List<int> GetAgeIdsUpTo89()
        {
            List<int> ageIds = new List<int> {AgeIds.From0To4};
            for (int ageId = AgeIds.From5To9; ageId <= AgeIds.From80To84; ageId++)
            {
                ageIds.Add(ageId);
            }
            ageIds.Add(AgeIds.From85To89);
            return ageIds;
        }
    }
}
