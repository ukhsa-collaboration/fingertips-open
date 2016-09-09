using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class PartitionDataForAllCategoriesBuilder : PartitionDataBuilderBase
    {
        public PartitionDataForAllCategories GetPartitionData(int profileId,
            string areaCode, int indicatorId, int sexId, int ageId, int areaTypeId)
        {
            InitGrouping(profileId, areaTypeId, indicatorId, sexId, ageId);
            InitMetadata(_grouping);

            var timePeriod = TimePeriod.GetDataPoint(_grouping);

            // Get Data
            IList<CoreDataSet> categoryDataList = _groupDataReader.GetAllCategoryDataWithinParentArea(areaCode,
                indicatorId, sexId, ageId, timePeriod);

            CalculateSignificances(areaCode, timePeriod, categoryDataList);
            FormatData(categoryDataList);

            return new PartitionDataForAllCategories
            {
                AreaCode = areaCode,
                IndicatorId = indicatorId,
                AgeId = ageId,
                SexId = sexId,
                CategoryTypes = GetCategoryTypes(categoryDataList),
                Data = categoryDataList
            };
        }

        public PartitionTrendData GetPartitionTrendData(int profileId,
            string areaCode, int indicatorId, int ageId, int sexId, int categoryTypeId, int areaTypeId)
        {
            InitGrouping(profileId, areaTypeId, indicatorId, sexId, ageId);
            InitMetadata(_grouping);
            var categories = _areasReader.GetCategories(categoryTypeId);
            var dictionaryBuilder = new PartitionTrendDataDictionaryBuilder(
                categories.Cast<INamedEntity>().ToList(), PartitionDataType.Category);

            // Add data for each time period
            var timePeriods = _grouping.GetTimePeriodIterator(_indicatorMetadata.YearType).TimePeriods;
            foreach (var timePeriod in timePeriods)
            {
                IList<CoreDataSet> dataList = _groupDataReader.GetAllCategoryDataWithinParentArea(areaCode,
                indicatorId, sexId, ageId, timePeriod).Where(x => x.CategoryTypeId == categoryTypeId).ToList();
                dictionaryBuilder.AddDataForNextTimePeriod(dataList);
            }

            // Remove entities without data from dictionary
            var allData = dictionaryBuilder.AllDataAsList;

            // Return trend data
            timePeriods = RemoveEarlyEmptyTimePeriods(dictionaryBuilder, timePeriods);
            FormatData(allData);
            var limits = new LimitsBuilder().GetLimits(allData);
            return new PartitionTrendData
            {
                Limits = limits,
                Labels = categories.Cast<INamedEntity>().ToList(),
                TrendData = dictionaryBuilder.Dictionary,
                Periods = GetTimePeriodStrings(timePeriods)
            };
        }

        private void InitGrouping(int profileId, int areaTypeId, int indicatorId, int sexId, int ageId)
        {
            var groupingProvider = new SingleGroupingProvider(_groupDataReader, new GroupIdProvider(_profileReader));
            _grouping = groupingProvider.GetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(profileId, areaTypeId, indicatorId, sexId, ageId);
        }

        private IList<CategoryType> GetCategoryTypes(IList<CoreDataSet> categoryDataList)
        {
            List<int> categoryTypeIds = categoryDataList.Select(x => x.CategoryTypeId).Distinct().ToList();
            IList<CategoryType> categoryTypes = _areasReader.GetCategoryTypes(categoryTypeIds);
            return categoryTypes;
        }
    }
}