using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    /// <summary>
    ///     Build a list of CoreDataSet objects processed to include the following:
    ///     - ValueFormatted defined
    ///     - significances calculated
    ///     - numbers truncated
    /// </summary>
    public class ChildAreaValuesBuilder : AreaValuesBuilder
    {
        private IAreasReader areasReader;
        private IndicatorComparerFactory indicatorComparerFactory;
        private IArea parentArea;
        private IProfileReader profileReader;

        public ChildAreaValuesBuilder(IndicatorComparerFactory indicatorComparerFactory,
            IGroupDataReader groupDataReader, IAreasReader areasReader, IProfileReader profileReader)
            : base(groupDataReader)
        {
            this.indicatorComparerFactory = indicatorComparerFactory;
            this.areasReader = areasReader;
            this.profileReader = profileReader;
        }

        public int ComparatorId { get; set; }
        public int RestrictToProfileId { get; set; }

        /// <summary>
        ///     Supported combinations:
        ///     Parent area England & comparator (i) deprivation decile (ii) national
        ///     Parent area is a region & comparator (i) region
        /// </summary>
        public IList<CoreDataSet> Build(Grouping grouping)
        {
            InitBuild(grouping);

            if (Grouping != null)
            {
                parentArea = AreaFactory.NewArea(areasReader, ParentAreaCode);
                return GetDataList();
            }

            return null;
        }

        private IList<CoreDataSet> GetDataList()
        {
            IList<CoreDataSet> dataList = FilterData(GetAllChildData());
            PerformComparisons(dataList);
            new CoreDataProcessor(Formatter).FormatAndTruncateList(dataList);
            return dataList;
        }

        private IList<CoreDataSet> GetAllChildData()
        {
            return new CoreDataSetListProvider(groupDataReader)
                .GetChildAreaData(Grouping, parentArea, Period);
        }

        private IList<CoreDataSet> FilterData(IList<CoreDataSet> dataList)
        {
            IList<string> areaCodesToIgnoreForParent = GetAreaCodesToIgnoreForParent();
            return new CoreDataSetFilter(dataList).RemoveWithAreaCode(areaCodesToIgnoreForParent).ToList();
        }

        private IList<string> GetAreaCodesToIgnoreForParent()
        {
            return RestrictToProfileId != ProfileIds.Undefined
                ? profileReader.GetAreaCodesToIgnore(RestrictToProfileId).AreaCodesIgnoredEverywhere
                : null;
        }

        private void PerformComparisons(IList<CoreDataSet> dataList)
        {
            IndicatorComparer comparer = indicatorComparerFactory.New(Grouping);
            TargetComparer targetComparer = TargetComparerFactory.New(IndicatorMetadata.TargetConfig);
            new TargetComparerHelper(groupDataReader, parentArea)
                    .AssignExtraDataIfRequired(parentArea, targetComparer, Grouping, IndicatorMetadata);

            CoreDataSet benchmarkData = null;
            ICategoryComparer categoryComparer = null;
            var averageCalculator = AverageCalculatorFactory.New(dataList, IndicatorMetadata);
            if (comparer is ICategoryComparer)
            {
                categoryComparer = comparer as ICategoryComparer;
                var values = new CoreDataSetFilter(dataList).SelectValidValues().ToList();
                categoryComparer.SetDataForCategories(values);
            }
            else
            {
                benchmarkData = new BenchmarkDataProvider(groupDataReader)
                    .GetBenchmarkData(Grouping, Period, averageCalculator, parentArea);
            }

            foreach (CoreDataSet coreData in dataList)
            {
                // NHibernate may have provided a cached data object for which significance
                // has already been added
                if (coreData.Significance.ContainsKey(ComparatorId) == false)
                {
                    int significance = categoryComparer == null
                        ? (int)comparer.Compare(coreData, benchmarkData, IndicatorMetadata)
                        : categoryComparer.GetCategory(coreData);

                    coreData.Significance.Add(ComparatorId, significance);
                }

                // NHibernate may have provided a cached data object for which significance
                // has already been added
                if (coreData.Significance.ContainsKey(ComparatorIds.Target) == false)
                {
                    TargetComparer.AddTargetSignificance(coreData, targetComparer);
                }
            }
        }
    }
}