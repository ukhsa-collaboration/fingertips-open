using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupRootComparisonManager
    {
        private IndicatorComparer comparer;

        public PholioReader PholioReader { get; set; }
        public TargetComparer TargetComparer { get; set; }
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();

        public void CompareToCalculateSignificance(GroupRoot groupRoot, IndicatorMetadata metadata)
        {
            CompareLocalAreaData(groupRoot, metadata);
            CompareAllDataAgainstTarget(groupRoot);
            CompareRegionalAgainstNational(groupRoot, metadata);
        }

        private void CompareLocalAreaData(GroupRoot groupRoot, IndicatorMetadata metadata)
        {
            var groupings = groupRoot.Grouping;
            foreach (var grouping in groupings)
            {
                if (grouping.IsComparatorDefined())
                {
                    comparer = new IndicatorComparerFactory { PholioReader = PholioReader }.New(grouping);
                    ICategoryComparer categoryComparer = comparer as ICategoryComparer;
                    IndicatorComparisonHelper.AssignCategoryDataIfRequired(comparer, grouping, groupDataReader,
                        groupRoot.Data);

                    foreach (CoreDataSet coreData in groupRoot.Data)
                    {
                        int significance = categoryComparer != null
                            ? categoryComparer.GetCategory(coreData)
                            : (int) comparer.Compare(coreData, grouping.ComparatorData, metadata);

                        coreData.Significance.Add(grouping.ComparatorId, significance);
                    }
                }
            }
        }

        private void CompareRegionalAgainstNational(GroupRoot groupRoot, IndicatorMetadata metadata)
        {
            if (comparer != null &&
                groupRoot.Grouping.Count > 1 &&
                CanCompareSubnationalAgainstNational(groupRoot.FirstGrouping.ComparatorMethodId))
            {
                Grouping regionalGrouping = groupRoot.GetSubnationalGrouping();
                Grouping nationalGrouping = groupRoot.GetNationalGrouping();

                if (nationalGrouping != null && regionalGrouping != null && regionalGrouping.ComparatorData != null)
                {
                    // If category comparer expection here then method may need to be in CanCompareSubnationalAgainstNational
                    Significance significance = comparer.Compare(regionalGrouping.ComparatorData, nationalGrouping.ComparatorData, metadata);
                    regionalGrouping.ComparatorData.AddSignificance(nationalGrouping.ComparatorId,
                        significance);
                }
            }
        }

        public bool CanCompareSubnationalAgainstNational(int comparatorMethodId)
        {
            return
                comparatorMethodId != ComparatorMethodIds.Quintiles &&
                comparatorMethodId != ComparatorMethodIds.Quartiles &&
                comparatorMethodId != ComparatorMethodIds.NoComparison;
        }

        private void CompareAllDataAgainstTarget(GroupRoot groupRoot)
        {
            if (TargetComparer != null)
            {
                // Local area data
                groupRoot.Data.ToList().ForEach(x =>
                    TargetComparer.AddTargetSignificance(x, TargetComparer));

                // Benchmark data
                groupRoot.Grouping.ToList().ForEach(x =>
                     TargetComparer.AddTargetSignificance(x.ComparatorData, TargetComparer));
            }
        }
    }
}
