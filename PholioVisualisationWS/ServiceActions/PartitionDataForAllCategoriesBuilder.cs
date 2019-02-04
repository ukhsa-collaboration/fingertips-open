using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.ServiceActions
{
    public class PartitionDataForAllCategoriesBuilder : PartitionDataBuilderBase
    {
         private IList<CoreDataSet> _specialCaseBenchmarkDataList = new List<CoreDataSet>();

        public PartitionDataForAllCategories GetPartitionData(int profileId,
            string areaCode, int indicatorId, int sexId, int ageId, int areaTypeId)
                {
            var partitionData = new PartitionDataForAllCategories
            {
                AreaCode = areaCode,
                IndicatorId = indicatorId,
                AgeId = ageId,
                SexId = sexId,
                BenchmarkDataSpecialCases = _specialCaseBenchmarkDataList
            };

            // Check grouping exists
            InitGrouping(profileId, areaTypeId, indicatorId, sexId, ageId);
            if (_grouping == null)
            {
                return partitionData;
            }

            InitMetadata(_grouping);

            var timePeriod = TimePeriod.GetDataPoint(_grouping);

            var maxYear = _groupDataReader.GetCoreDataMaxYear(indicatorId);
            if (timePeriod.Year != maxYear)
            {
                timePeriod.Year = maxYear;
            }

            // Get Data
            IList<CoreDataSet> categoryDataList = _groupDataReader.GetAllCategoryDataWithinParentArea(areaCode,
                indicatorId, sexId, ageId, timePeriod);

            CalculateSignificances(areaCode, timePeriod, categoryDataList);
            FormatData(categoryDataList);
            FormatData(_specialCaseBenchmarkDataList);

            //partitionData.CategoryTypes = GetCategoryTypes(categoryDataList);
            partitionData.CategoryTypes = GetCategoryTypes(areaCode, indicatorId, sexId, ageId);

            partitionData.Data = categoryDataList;
            return partitionData;
        }

        public PartitionTrendData GetPartitionTrendData(int profileId,
            string areaCode, int indicatorId, int ageId, int sexId, int categoryTypeId, int areaTypeId)
        {
            InitGrouping(profileId, areaTypeId, indicatorId, sexId, ageId);
            if (_grouping == null)
            {
                return null;
            }

            InitMetadata(_grouping);
            var categories = _areasReader.GetCategories(categoryTypeId);

            var dictionaryBuilder = new PartitionTrendDataDictionaryBuilder(
                categories.Cast<INamedEntity>().ToList(), PartitionDataType.Category);

            IList<CoreDataSet> areaAverage = new List<CoreDataSet>();

            // Add data for each time period
            var timePeriods = _grouping.GetTimePeriodIterator(_indicatorMetadata.YearType).TimePeriods;
            foreach (var timePeriod in timePeriods)
            {
                IList<CoreDataSet> dataList = _groupDataReader
                    .GetAllCategoryDataWithinParentArea(areaCode, indicatorId, sexId, ageId, timePeriod)
                    .Where(x => x.CategoryTypeId == categoryTypeId).ToList();
                dictionaryBuilder.AddDataForNextTimePeriod(dataList);

                // Get coredata for selected area
                var coreDataSetForSelectedArea = _groupDataReader
                    .GetCoreData(_grouping, timePeriod, areaCode)
                    .Where(x => x.CategoryTypeId == CategoryTypeIds.Undefined)
                    .ToList();
                if (coreDataSetForSelectedArea.Any())
                {
                    areaAverage.Add(coreDataSetForSelectedArea.First());
                }
            }

            // Remove entities without data from dictionary
            var allData = dictionaryBuilder.AllDataAsList;

            timePeriods = RemoveEarlyEmptyTimePeriods(dictionaryBuilder, timePeriods);

            var areaAverageAccordingToTimePeriod = new List<CoreDataSet>();

            // Format category data
            FormatData(allData);
            // Format area average            
            FormatData(areaAverageAccordingToTimePeriod);
            var limits = new LimitsBuilder().GetLimits(allData);

            return new PartitionTrendData
            {
                Limits = limits,
                Labels = categories.Cast<INamedEntity>().ToList(),
                TrendData = dictionaryBuilder.Dictionary,
                Periods = GetTimePeriodStrings(timePeriods),
                AreaAverage = areaAverageAccordingToTimePeriod
            };
        }

        private void InitGrouping(int profileId, int areaTypeId, int indicatorId, int sexId, int ageId)
        {
            var groupingProvider = new SingleGroupingProvider(_groupDataReader, new GroupIdProvider(_profileReader));
            _grouping = groupingProvider.GetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(profileId, areaTypeId, indicatorId, sexId, ageId);
        }

        private IList<CategoryType> GetCategoryTypes(string areaCode, int indicatorId, int sexId, int ageId)
        {
            IList<int> categoryTypeIds = _groupDataReader.GetAllCategoryTypeIds(areaCode,
                indicatorId, sexId, ageId);
            IList<CategoryType> categoryTypes = _areasReader.GetCategoryTypes(categoryTypeIds);
            return categoryTypes;
        }

        /// <summary>
        /// Override base method to allow handling of special cases, e.g. where a specific age must be used
        /// </summary>
        protected override CoreDataSet GetSpecialCaseBenchmarkData(TimePeriod timePeriod, 
            IArea area)
        {
            // Handle special case for selecting benchmark data where specific age ID is required
            var ageId = _grouping.AgeId;
            if (_specialCase.ShouldUseSpecificAgeId())
            {
                _grouping.AgeId = _specialCase.BenchmarkAgeId;
            }

            // Get benchmark data
            CoreDataSet benchmarkData = base.GetBenchmarkData(timePeriod, area);

            if (_specialCase.ShouldUseSpecificAgeId())
            {
                _specialCaseBenchmarkDataList.Add(benchmarkData);
            }

            // Restore original age ID
            _grouping.AgeId = ageId;
            return benchmarkData;
        }
    }
}