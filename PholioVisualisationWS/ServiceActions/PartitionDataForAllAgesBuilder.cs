using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class PartitionDataForAllAgesBuilder : PartitionDataBuilderBase
    {
        private int _chartAverageLineAgeId = -1;
        private IList<CoreDataSet> _dataList = new List<CoreDataSet>();

        public PartitionDataForAllAges GetPartitionData(int profileId,
            string areaCode, int indicatorId, int sexId, int areaTypeId)
        {
            InitGrouping(profileId, areaTypeId, indicatorId, sexId);
            if (_grouping == null)
            {
                // No inequality data available
                return new PartitionDataForAllAges
                {
                    AreaCode = areaCode,
                    IndicatorId = indicatorId,
                    Ages = new List<Age>(),
                    SexId = sexId,
                    Data = new List<CoreDataSet>()
                };
            }

            InitMetadata(_grouping);
            if (_specialCase != null && _specialCase.ShouldUseForAverageLineOnChart())
            {
                _chartAverageLineAgeId = _specialCase.ChartAverageLineAgeId;
            }

            InitTimePeriod(profileId, _grouping);

            // Get Data
            _dataList = _groupDataReader.GetCoreDataForAllAges(indicatorId,
                _timePeriod, areaCode, sexId);
            _dataList = FilterOutAgeIdsIfRequired(_dataList);

            // Define and order ages
            var ages = GetAgesFromDataList(_dataList);

            // Process data list
            _dataList = new CoreDataSetSorter(_dataList).SortByAgeId(ages);
            CalculateSignificances(areaCode, _timePeriod, _dataList);
            FormatData(_dataList);

            return new PartitionDataForAllAges
            {
                AreaCode = areaCode,
                IndicatorId = indicatorId,
                SexId = sexId,
                Ages = ages,
                Data = _dataList,
                ChartAverageLineAgeId = _chartAverageLineAgeId
            };
        }

        private IList<CoreDataSet> FilterOutAgeIdsIfRequired(IList<CoreDataSet> dataList)
        {
            if (_specialCase != null && _specialCase.ShouldOmitSpecificAgeId())
            {
                dataList = new CoreDataSetFilter(dataList)
                    .RemoveWithAgeId(new List<int> {_specialCase.AgeIdToOmit})
                    .ToList();
            }

            return dataList;
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

        protected override void CalculateSignificances(string areaCode, TimePeriod timePeriod, IList<CoreDataSet> dataList)
        {
            // Get benchmark data
            var area = AreaFactory.NewArea(_areasReader, areaCode);
            var benchmarkData = GetBenchmarkData(timePeriod, area);

            if (_indicatorMetadata.HasSpecialCases)
            {
                benchmarkData = GetSpecialCaseBenchmarkData(timePeriod, area);
            }

            var indicatorComparisonHelper = GetIndicatorComparisonHelper();
            foreach (CoreDataSet coreDataSet in dataList)
            {
                coreDataSet.SignificanceAgainstOneBenchmark = indicatorComparisonHelper.GetSignificance(coreDataSet, benchmarkData);
            }
        }

        /// <summary>
        /// Override base method to allow handling of special cases
        /// </summary>
        protected override CoreDataSet GetSpecialCaseBenchmarkData(TimePeriod timePeriod,
            IArea area)
        {
            foreach (var data in _dataList)
            {
                if (data.AgeId == _chartAverageLineAgeId)
                {
                    return data;
                }
            }

            return null;
        }
    }
}