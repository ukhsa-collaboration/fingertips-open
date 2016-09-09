using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataHelpers;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class PartitionDataForAllAgesBuilder : PartitionDataBuilderBase
    {
        public PartitionDataForAllAges GetPartitionData(int profileId,
            string areaCode, int indicatorId, int sexId, int areaTypeId)
        {
            InitGrouping(profileId, areaTypeId, indicatorId, sexId);
            InitMetadata(_grouping);

            var timePeriod = TimePeriod.GetDataPoint(_grouping);

            // Get Data
            var dataList = _groupDataReader.GetCoreDataForAllAges(indicatorId,
                timePeriod, areaCode, sexId);

            // Define and order ages
            var ages = GetAgesFromDataList(dataList);

            // Process data list
            dataList = new CoreDataSetSorter(dataList).SortByAgeId(ages);
            CalculateSignificances(areaCode, timePeriod, dataList);
            FormatData(dataList);

            return new PartitionDataForAllAges
            {
                AreaCode = areaCode,
                IndicatorId = indicatorId,
                SexId = sexId,
                Ages = ages,
                Data = dataList
            };
        }

        public PartitionTrendData GetPartitionTrendData(int profileId, string areaCode,
            int indicatorId, int sexId, int areaTypeId)
        {
            InitGrouping(profileId, areaTypeId, indicatorId, sexId);
            InitMetadata(_grouping);
            var allAges = _pholioReader.GetAllAges();
            var dictionaryBuilder = new PartitionTrendDataDictionaryBuilder(
                allAges.Cast<INamedEntity>().ToList(), PartitionDataType.Age);

            // Add data for each time period
            var timePeriods = _grouping.GetTimePeriodIterator(_indicatorMetadata.YearType).TimePeriods;
            foreach (var timePeriod in timePeriods)
            {
                IList<CoreDataSet> dataList = _groupDataReader.GetCoreDataForAllAges(
                    indicatorId, timePeriod, areaCode, sexId);
                dictionaryBuilder.AddDataForNextTimePeriod(dataList);
            }

            // Remove entities without data from dictionary
            var allData = dictionaryBuilder.AllDataAsList;
            var agesWithData = GetAgesFromDataList(allData);
            foreach (var age in allAges)
            {
                if (agesWithData.Contains(age) == false)
                {
                    dictionaryBuilder.RemoveEntity(age.Id);
                }
            }

            // Return trend data
            timePeriods = RemoveEarlyEmptyTimePeriods(dictionaryBuilder, timePeriods);
            FormatData(allData);
            var limits = new LimitsBuilder().GetLimits(allData);
            return new PartitionTrendData
            {
                Limits = limits,
                Labels = agesWithData.Cast<INamedEntity>().ToList(),
                TrendData = dictionaryBuilder.Dictionary,
                Periods = GetTimePeriodStrings(timePeriods)
            };
        }

        private IList<Age> GetAgesFromDataList(IList<CoreDataSet> dataList)
        {
            var ageIds = new CoreDataSetFilter(dataList).SelectDistinctAgeIds().ToList();
            var ages = _pholioReader.GetAgesByIds(ageIds);
            ages = new AgeSorter().SortByAge(ages);
            return ages;
        }

        private void InitGrouping(int profileId, int areaTypeId, int indicatorId, int sexId)
        {
            var groupingProvider = new SingleGroupingProvider(_groupDataReader, new GroupIdProvider(_profileReader));
            _grouping = groupingProvider.GetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndSexId(profileId, areaTypeId, indicatorId, sexId);
        }

    }
}