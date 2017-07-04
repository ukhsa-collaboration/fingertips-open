using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class PartitionDataForAllSexesBuilder : PartitionDataBuilderBase
    {
        public PartitionDataForAllSexes GetPartitionData(int profileId,
            string areaCode, int indicatorId, int ageId, int areaTypeId)
        {
            InitGrouping(profileId, areaTypeId, indicatorId, ageId);
            InitMetadata(_grouping);

            var timePeriod = TimePeriod.GetDataPoint(_grouping);

            // Get Data
            IList<CoreDataSet> dataList = _groupDataReader.GetCoreDataForAllSexes(indicatorId,
                timePeriod, areaCode, ageId);

            // Define and order sexes
            var sexes = GetSexesFromDataList(dataList);

            // Process data list
            dataList = new CoreDataSetSorter(dataList).SortBySexId(sexes);
            CalculateSignificances(areaCode, timePeriod, dataList);
            FormatData(dataList);

            return new PartitionDataForAllSexes
            {
                AreaCode = areaCode,
                IndicatorId = indicatorId,
                AgeId = ageId,
                Sexes = sexes,
                Data = dataList
            };
        }

        public PartitionTrendData GetPartitionTrendData(int profileId,
            string areaCode, int indicatorId, int ageId, int areaTypeId)
        {
            InitGrouping(profileId, areaTypeId, indicatorId, ageId);
            InitMetadata(_grouping);
            var sexes = _pholioReader.GetAllSexes();
            var dictionaryBuilder = new PartitionTrendDataDictionaryBuilder(
                sexes.Cast<INamedEntity>().ToList(), PartitionDataType.Sex);

            // Add data for each time period
            var timePeriods = _grouping.GetTimePeriodIterator(_indicatorMetadata.YearType).TimePeriods;
            foreach (var timePeriod in timePeriods)
            {
                IList<CoreDataSet> dataList = _groupDataReader.GetCoreDataForAllSexes(
                    indicatorId, timePeriod, areaCode, ageId);
                dictionaryBuilder.AddDataForNextTimePeriod(dataList);
            }

            // Remove entities without data from dictionary
            var allData = dictionaryBuilder.AllDataAsList;
            var sexesWithData = GetSexesFromDataList(allData);
            foreach (var sex in sexes)
            {
                if (sexesWithData.Contains(sex) == false)
                {
                    dictionaryBuilder.RemoveEntity(sex.Id);
                }
            }

            // Return trend data
            timePeriods = RemoveEarlyEmptyTimePeriods(dictionaryBuilder, timePeriods);
            FormatData(allData);
            var limits = new LimitsBuilder().GetLimits(allData);
            return new PartitionTrendData
            {
                Limits = limits,
                Labels = sexesWithData.Cast<INamedEntity>().ToList(),
                TrendData = dictionaryBuilder.Dictionary,
                Periods = GetTimePeriodStrings(timePeriods)
            };
        }

        private IList<Sex> GetSexesFromDataList(IList<CoreDataSet> dataList)
        {
            List<int> sexIds = new CoreDataSetFilter(dataList).SelectDistinctSexIds().ToList();
            IList<Sex> sexes = _pholioReader.GetSexesByIds(sexIds);
            return sexes;
        }

        private void InitGrouping(int profileId, int areaTypeId, int indicatorId, int ageId)
        {
            var groupingProvider = new SingleGroupingProvider(_groupDataReader, new GroupIdProvider(_profileReader));
            _grouping = groupingProvider.GetGroupingByProfileIdAndAreaTypeIdAndIndicatorIdAndAgeId(profileId, areaTypeId, indicatorId, ageId);
        }

        protected override void CalculateSignificances(string areaCode, TimePeriod timePeriod, IList<CoreDataSet> categoryDataList)
        {
            var personsData = categoryDataList.FirstOrDefault(x => x.SexId == SexIds.Persons);

            if (personsData == null && _indicatorMetadata.HasTarget == false)
            {
                // Do not calculate significance
                foreach (var coreDataSet in categoryDataList)
                {
                    coreDataSet.SignificanceAgainstOneBenchmark = (int)Significance.None;
                }
            }
            else
            {
                // Calculate significance (is data for persons or there is a target)
                var targetComparerProvider = new TargetComparerProvider(_groupDataReader, _areasReader);
                var indicatorComparisonHelper = new IndicatorComparisonHelper(_indicatorMetadata,
                    _grouping, _groupDataReader, _pholioReader, targetComparerProvider);
                foreach (var coreDataSet in categoryDataList)
                {
                    coreDataSet.SignificanceAgainstOneBenchmark =
                        indicatorComparisonHelper.GetSignificance(coreDataSet, personsData);
                }
            }
        }
    }
}