using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.ServiceActions
{
    public class PartitionDataForAllCategoriesBuilder : PartitionDataBuilderBase
    {
        public PartitionDataForAllCategories GetPartitionData(int profileId,
            string areaCode, int indicatorId, int sexId, int ageId, int areaTypeId)
        {
            var partitionData = new PartitionDataForAllCategories
            {
                AreaCode = areaCode,
                IndicatorId = indicatorId,
                AgeId = ageId,
                SexId = sexId
            };

            // Check grouping exists
            InitGrouping(profileId, areaTypeId, indicatorId, sexId, ageId);
            if (_grouping == null)
            {
                return partitionData;
            }

            InitMetadata(_grouping);

            var timePeriod = TimePeriod.GetDataPoint(_grouping);

            // Get Data
            IList<CoreDataSet> categoryDataList = _groupDataReader.GetAllCategoryDataWithinParentArea(areaCode,
                indicatorId, sexId, ageId, timePeriod);

            CalculateSignificances(areaCode, timePeriod, categoryDataList);
            FormatData(categoryDataList);

            partitionData.CategoryTypes = GetCategoryTypes(categoryDataList);
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
                IList<CoreDataSet> dataList = _groupDataReader.GetAllCategoryDataWithinParentArea(areaCode,
                indicatorId, sexId, ageId, timePeriod).Where(x => x.CategoryTypeId == categoryTypeId).ToList();
                dictionaryBuilder.AddDataForNextTimePeriod(dataList);

                // Get coredata for selected area
                var coreDataSetForSelectedArea = _groupDataReader.GetCoreData(_grouping, timePeriod, areaCode).Where(x => x.CategoryTypeId == -1).ToList();
                if (coreDataSetForSelectedArea.Any())
                {
                    areaAverage.Add(coreDataSetForSelectedArea.First());
                }
            }

            // Remove entities without data from dictionary
            var allData = dictionaryBuilder.AllDataAsList;

            timePeriods = RemoveEarlyEmptyTimePeriods(dictionaryBuilder, timePeriods);

            var areaAverageAccordingToTimePeriod = new List<CoreDataSet>();
            // Remove unwanted period data
            foreach (var timePeriod in timePeriods)
            {
                foreach (var dataSet in areaAverage)
                {
                    if (dataSet.Year == timePeriod.Year)
                    {
                        areaAverageAccordingToTimePeriod.Add(dataSet);
                    }
                }
            }


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

        private IList<CategoryType> GetCategoryTypes(IList<CoreDataSet> categoryDataList)
        {
            List<int> categoryTypeIds = categoryDataList.Select(x => x.CategoryTypeId).Distinct().ToList();
            IList<CategoryType> categoryTypes = _areasReader.GetCategoryTypes(categoryTypeIds);
            return categoryTypes;
        }
    }
}