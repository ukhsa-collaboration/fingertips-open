using System.Collections.Generic;
using System.Linq;
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

        public BodyPeriodWriterContainer(IndicatorExportParameters generalParameters, OnDemandQueryParametersWrapper onDemandQueryParameters,
            IndicatorMetadata indicatorMetadata, ExportAreaHelper areaHelper, IGroupDataReader groupDataReader, IAreasReader areasReader, IList<TimePeriod> timePeriods,
            MultipleCoreDataCollector coreDataCollector, Grouping grouping, IndicatorComparer comparer = null)
        {
            _generalParameters = generalParameters;
            _onDemandQueryParameters = onDemandQueryParameters;
            _indicatorMetadata = indicatorMetadata;
            _areaHelper = areaHelper;
            _timePeriods = timePeriods;
            var attributesForPeriods = new CsvBuilderAttributesForPeriodsWrapper();

            _searchContainer = new BodyPeriodSearchContainer(areaHelper, generalParameters, onDemandQueryParameters, attributesForPeriods, groupDataReader, areasReader, grouping);
            _comparisonContainer = new BodyPeriodComparisonContainer(areaHelper, comparer);
            _significanceContainer = new BodyPeriodSignificanceContainer(areaHelper, comparer);
            _trendContainer = new BodyPeriodTrendContainer(coreDataCollector, attributesForPeriods);

        }

        public IList<CoreDataSet> WriteChildProcessedData(int indicatorId, TimePeriod timePeriod, string timeString, int sortableTimePeriod, SingleIndicatorFileWriter fileWriter,
            IList<CoreDataSet> englandCoreDataForComparison, IList<CoreDataSet> parentCoreDataForComparison, Grouping grouping)
        {
            // It doesn't make any filter if inequalities variable is null
            var childCoreDataList = _searchContainer.GetFilterCoreDataForChildArea(indicatorId, timePeriod);

            var categoryComparisonManager = _comparisonContainer.GetCategoryComparisonManager(englandCoreDataForComparison, parentCoreDataForComparison, childCoreDataList);

            WriteChildCoreDataInFile(childCoreDataList, categoryComparisonManager, englandCoreDataForComparison,
                parentCoreDataForComparison, timePeriod, timeString, sortableTimePeriod, fileWriter, grouping);

            return childCoreDataList;
        }

        public IList<CoreDataSet> WriteProcessedNationalData(int indicatorId, TimePeriod timePeriod, string timeString, int sortableTimePeriod, SingleIndicatorFileWriter fileWriter,
            ref IList<CoreDataSet> coreDataForComparison, Grouping grouping, params string[] areaCode)
        {
            // It doesn't make any filter if inequalities variable is null
            var coreDataList = _searchContainer.FilterCoreDataByInequalitiesNational(indicatorId, timePeriod, areaCode);

            var coreDataForComparisonToWrite = BodyPeriodComparisonContainer.GetCoreDataForComparisonToWrite(ref coreDataForComparison, coreDataList, CategoryTypeIds.Undefined);

            WriteCoreDataInFile(coreDataList, coreDataForComparisonToWrite, timePeriod, timeString, sortableTimePeriod,
                fileWriter, grouping);

            return coreDataList;
        }

        public IList<CoreDataSet> WriteProcessedSubNationalData(int indicatorId, TimePeriod timePeriod, string timeString, int sortableTimePeriod, SingleIndicatorFileWriter fileWriter,
            ref IList<CoreDataSet> coreDataForComparison, Grouping grouping)
        {
            var areaCodes = _areaHelper.GetAreaCodes();
            var parentArea = _areaHelper.GetParentArea();

            // It doesn't make any filter if inequalities variable is null
            var coreDataList = _searchContainer.FilterCoreDataByInequalitiesSubNational(indicatorId, timePeriod, areaCodes);

            coreDataList = _searchContainer.GetFilteredCoreDataSetsForSubNational(indicatorId, timePeriod, parentArea, areaCodes, _indicatorMetadata, coreDataList);

            var coreDataForComparisonToWrite = BodyPeriodComparisonContainer.GetCoreDataForComparisonToWrite(ref coreDataForComparison, coreDataList, _areaHelper.CategoryTypeId);

            WriteCoreDataInFile(coreDataList, coreDataForComparisonToWrite, timePeriod, timeString, sortableTimePeriod,
                fileWriter, grouping, parentArea);

            return coreDataList;
        }

        private void WriteCoreDataInFile(IEnumerable<CoreDataSet> coreDataEnumerable, IEnumerable<CoreDataSet> coreDataForComparisonEnumerable, TimePeriod timePeriod, string timeString, int sortableTimePeriod,
            SingleIndicatorFileWriter fileWriter, Grouping grouping, IArea parentArea = null)
        {
            foreach (var coreData in coreDataEnumerable)
            {
                TrendMarkerResult trendMarkerResult = null;
                if (IsLastPeriod(timePeriod))
                {
                    trendMarkerResult = _trendContainer.GetTrendMarker(_indicatorMetadata, coreData, ExportAreaHelper.GetGeographicalCategory(parentArea), grouping);
                }

                if (!IsPeriodToWrite(timePeriod)) continue;
                var toSignificance = _significanceContainer.GetSignificance(_indicatorMetadata, coreData, coreDataForComparisonEnumerable, parentArea);
                fileWriter.WriteData(coreData, timeString, parentArea, trendMarkerResult, toSignificance,
                    Significance.None, sortableTimePeriod, grouping);
            }
        }

        private void WriteChildCoreDataInFile(IEnumerable<CoreDataSet> childCoreData, CategoryComparisonManager categoryComparisonManager,
            IEnumerable<CoreDataSet> englandCoreDataForComparison, IEnumerable<CoreDataSet> parentCoreDataForComparison, TimePeriod timePeriod, string timeString,
            int sortableTimePeriod, SingleIndicatorFileWriter fileWriter, Grouping grouping)
        {
            foreach (var coreData in childCoreData)
            {
                // Skip areas that don't map to a parent (way to avoid legacy areas)
                if (IsNotAreaMappingWithParent(coreData) && IsNotAreaMappingWithInCategoryAreaId(coreData)) continue;

                // Only calculated when is the last period
                TrendMarkerResult trendMarkerResult = null;
                if (IsLastPeriod(timePeriod))
                {
                    trendMarkerResult = _trendContainer.GetTrendMarker(_indicatorMetadata, coreData, ExportAreaHelper.GeographicalCategory.Local, grouping);
                }

                // Write when is all periods or the last one
                if (!IsPeriodToWrite(timePeriod)) continue;

                IArea parentArea;
                var toEnglandSignificance = _significanceContainer.GetSignificance(categoryComparisonManager, englandCoreDataForComparison, _indicatorMetadata, coreData);
                var toSubNationalParentSignificance = _significanceContainer.GetSubNationalParentSignificance(categoryComparisonManager, parentCoreDataForComparison, _indicatorMetadata,
                    coreData, IsParentEngland, out parentArea);

                fileWriter.WriteData(coreData, timeString, parentArea, trendMarkerResult, toEnglandSignificance,
                    toSubNationalParentSignificance, sortableTimePeriod, grouping);
            }
        }

        private bool IsParentEngland
        {
            get { return _generalParameters.ParentAreaTypeId == AreaTypeIds.Country; }
        }

        private bool IsNotAreaMappingWithParent(CoreDataSet coreData)
        {
            return IsParentEngland == false && _areaHelper.ChildAreaCodeToParentAreaMap.ContainsKey(coreData.AreaCode) == false;
        }

        private bool IsNotAreaMappingWithInCategoryAreaId(CoreDataSet coreData)
        {
            foreach (var areaCodes in _areaHelper.ParentCategoryAreaIdToChildAreaCodesMap)
            {
                if (areaCodes.Value.Contains(coreData.AreaCode)) return false;
            }

            return true;
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