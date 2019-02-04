using System.Collections.Generic;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Export.FileBuilder.Wrappers;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export.FileBuilder.Containers
{
    public class BodyPeriodWriterContainer
    {
        private readonly IndicatorExportParameters _generalParameters;
        private readonly OnDemandQueryParametersWrapper _onDemandQueryParameters;
        private readonly IndicatorMetadata _indicatorMetadata;
        private readonly ExportAreaHelper _areaHelper;
        private readonly IList<TimePeriod> _timePeriods;

        private readonly BodyPeriodSearchContainer _searchContainer;
        private readonly BodyPeriodComparisonContainer _comparisonContainer;
        private readonly BodyPeriodSignificanceContainer _significanceContainer;
        private readonly BodyPeriodTrendContainer _trendContainer;

        public BodyPeriodWriterContainer(IndicatorExportParameters generalParameters, OnDemandQueryParametersWrapper onDemandQueryParameters, IndicatorMetadata indicatorMetadata, ExportAreaHelper areaHelper,
                                            IGroupDataReader groupDataReader, IList<TimePeriod> timePeriods, MultipleCoreDataCollector coreDataCollector, Grouping grouping, IndicatorComparer comparer = null)
        {
            _generalParameters = generalParameters;
            _onDemandQueryParameters = onDemandQueryParameters;
            _indicatorMetadata = indicatorMetadata;
            _areaHelper = areaHelper;
            _timePeriods = timePeriods;
            var attributesForPeriods = new CsvBuilderAttributesForPeriodsWrapper();

            _searchContainer = new BodyPeriodSearchContainer(areaHelper, onDemandQueryParameters, attributesForPeriods, groupDataReader, grouping);
            _comparisonContainer = new BodyPeriodComparisonContainer(areaHelper, comparer);
            _significanceContainer =  new BodyPeriodSignificanceContainer(areaHelper, comparer);
            _trendContainer = new BodyPeriodTrendContainer(coreDataCollector, attributesForPeriods);

        }

        public IList<CoreDataSet> WriteChildProcessedData(int indicatorId, TimePeriod timePeriod, string timeString, int sortableTimePeriod, ref SingleIndicatorFileWriter fileWriter,
            IList<CoreDataSet> englandCoreDataForComparison, IList<CoreDataSet> parentCoreDataForComparison)
        {
            // It doesn't make any filter if inequalities variable is null
            var childCoreDataList = _searchContainer.FilterCoreDataByInequalities(indicatorId, timePeriod, _areaHelper.ChildAreaCodes);

            _searchContainer.FilterChildCoreDataByChildAreaCodeList(ref childCoreDataList);

            var categoryComparisonManager = _comparisonContainer.GetCategoryComparisonManager(englandCoreDataForComparison, parentCoreDataForComparison, childCoreDataList);

            WriteChildCoreDataInFile(childCoreDataList, _areaHelper.ChildAreaCodeToParentAreaMap, categoryComparisonManager, englandCoreDataForComparison, parentCoreDataForComparison, timePeriod, timeString,
                sortableTimePeriod, ref fileWriter);

            return childCoreDataList;
        }

        public IList<CoreDataSet> WriteProcessedData(int indicatorId, TimePeriod timePeriod, string timeString, int sortableTimePeriod, ref SingleIndicatorFileWriter fileWriter,
            ref IList<CoreDataSet> coreDataForComparison, IArea parentArea, params string[] areaCode)
        {
            // It doesn't make any filter if inequalities variable is null
            var coreDataList = _searchContainer.FilterCoreDataByInequalities(indicatorId, timePeriod, areaCode);


            if (ExportAreaHelper.GetGeographicalCategory(parentArea) == ExportAreaHelper.GeographicalCategory.SubNational)
            { 
                // Filter coreData for subNational
                _searchContainer.FilterParentCoreDataByChildAreaCodeList(ref coreDataList);
            }

            var coreDataForComparisonToWrite = BodyPeriodComparisonContainer.GetCoreDataForComparisonToWrite(ref coreDataForComparison, coreDataList);

            WriteCoreDataInFile(coreDataList, coreDataForComparisonToWrite, timePeriod, timeString, sortableTimePeriod, ref fileWriter, parentArea);

            return coreDataList;
        }

        private void WriteCoreDataInFile(IEnumerable<CoreDataSet> coreDataEnumerable, IEnumerable<CoreDataSet> coreDataForComparisonEnumerable, TimePeriod timePeriod, string timeString, int sortableTimePeriod,
            ref SingleIndicatorFileWriter fileWriter, IArea parentArea = null)
        {
            foreach (var coreData in coreDataEnumerable)
            {
                TrendMarkerResult trendMarkerResult = null;
                if (IsLastPeriod(timePeriod))
                {
                    trendMarkerResult = _trendContainer.GetTrendMarker(_indicatorMetadata, coreData, ExportAreaHelper.GetGeographicalCategory(parentArea));
                }

                if (!IsPeriodToWrite(timePeriod)) continue;
                var toSignificance = _significanceContainer.GetSignificance(_indicatorMetadata, coreData, coreDataForComparisonEnumerable, parentArea);
                fileWriter.WriteData(coreData, timeString, parentArea, trendMarkerResult, toSignificance, Significance.None, sortableTimePeriod);
            }
        }

        private void WriteChildCoreDataInFile(IEnumerable<CoreDataSet> childCoreData, IReadOnlyDictionary<string, Area> childAreaCodeToParentAreaMap, CategoryComparisonManager categoryComparisonManager,
            IEnumerable<CoreDataSet> englandCoreDataForComparison, IEnumerable<CoreDataSet> parentCoreDataForComparison, TimePeriod timePeriod, string timeString,
            int sortableTimePeriod, ref SingleIndicatorFileWriter fileWriter)
        {
            foreach (var coreData in childCoreData)
            {
                // Skip areas that don't map to a parent (way to avoid legacy areas)
                if (IsNotAreaMappingWithParent(childAreaCodeToParentAreaMap, coreData)) continue;

                // Only calculated when is the last period
                TrendMarkerResult trendMarkerResult = null;
                if (IsLastPeriod(timePeriod))
                {
                    trendMarkerResult = _trendContainer.GetTrendMarker(_indicatorMetadata, coreData, ExportAreaHelper.GeographicalCategory.Local);
                }

                // Write when is all periods or the last one
                if (!IsPeriodToWrite(timePeriod)) continue;

                IArea parentArea;
                var toEnglandSignificance = _significanceContainer.GetSignificance(categoryComparisonManager, englandCoreDataForComparison, _indicatorMetadata, coreData);
                var toSubNationalParentSignificance = _significanceContainer.GetSubNationalParentSignificance(childAreaCodeToParentAreaMap, categoryComparisonManager, parentCoreDataForComparison, _indicatorMetadata,
                    coreData, IsParentEngland, out parentArea);

                fileWriter.WriteData(coreData, timeString, parentArea, trendMarkerResult, toEnglandSignificance, toSubNationalParentSignificance, sortableTimePeriod);
            }
        }

        private bool IsParentEngland
        {
            get { return _generalParameters.ParentAreaTypeId == AreaTypeIds.Country; }
        }

        private bool IsNotAreaMappingWithParent(IReadOnlyDictionary<string, Area> childAreaCodeToParentAreaMap, CoreDataSet coreData)
        {
            return IsParentEngland == false && childAreaCodeToParentAreaMap.ContainsKey(coreData.AreaCode) == false;
        }

        private bool IsPeriodToWrite(TimePeriod timePeriod)
        {
            return _onDemandQueryParameters.AllPeriods || IsLastPeriod(timePeriod);
        }

        private bool IsLastPeriod(TimePeriod period)
        {
            return _timePeriods.IndexOf(period) == _timePeriods.Count - 1;
        }
    }
}
