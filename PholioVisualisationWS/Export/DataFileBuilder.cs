using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Export.File;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class DataFileBuilder
    {
        // Dependencies
        private ExportAreaHelper _areaHelper;
        private IndicatorMetadataProvider _indicatorMetadataProvider;
        private IAreasReader _areasReader;
        private IFileBuilder _fileBuilder;

        private IndicatorComparer _comparer;
        private IndicatorExportParameters _parameters;
        private PholioReader _pholioReader = ReaderFactory.GetPholioReader();

        // Lazy initialisation
        private LookUpManager _lookUpManager;

        public DataFileBuilder(IndicatorMetadataProvider indicatorMetadataProvider, ExportAreaHelper areaHelper,
            IAreasReader areasReader, IFileBuilder fileBuilder)
        {
            _indicatorMetadataProvider = indicatorMetadataProvider;
            _areaHelper = areaHelper;
            _areasReader = areasReader;
            _fileBuilder = fileBuilder;
        }

        public byte[] GetFileForSpecifiedIndicators(IList<int> indicatorIds, IndicatorExportParameters parameters)
        {
            _parameters = parameters;

            _fileBuilder.AddContent(GetHeader());

            var groupDataReader = ReaderFactory.GetGroupDataReader();

            var trendMarkersProvider = new TrendMarkersProvider(ReaderFactory.GetTrendDataReader(),
                new TrendMarkerCalculator());

            var england = _areaHelper.England;
            var isParentEngland = _parameters.ParentAreaTypeId == AreaTypeIds.Country;

            var groupIds = new GroupIdProvider(ReaderFactory.GetProfileReader())
                .GetGroupIds(parameters.ProfileId);
            var singleGroupingProvider = new SingleGroupingProvider(groupDataReader, null);

            var excludedCategoryTypeIds = CategoryTypeIdsExcludedForExport.Ids;
            var includeSortableTimePeriod = _parameters.IncludeSortableTimePeriod;

            var indicatorMetadataTextOption = GetIndicatorMetadataTextOption(parameters.ProfileId);

            var childAreaTypeId = parameters.ChildAreaTypeId;
            foreach (var indicatorId in indicatorIds)
            {
                var fileWriter = new SingleIndicatorFileWriter(indicatorId, parameters);

                // Try load file
                var bytes = fileWriter.TryLoadFile();
                if (bytes != null)
                {
                    _fileBuilder.AddContent(bytes);
                    continue;
                }

                // Clear session to avoid out of memory
                groupDataReader.ClearSession();

                // Find the grouping with the latest data
                var grouping = singleGroupingProvider.GetGroupingWithLatestDataPoint(groupIds,
                    indicatorId, childAreaTypeId);

                // Ignore the indicator if no grouping found
                if (grouping == null) continue;

                // Lazy initialisation (would not be necessary if cached files are available)
                _areaHelper.Init();

                InitFileWriter(grouping, fileWriter);
                InitComparer(grouping);

                // Category comparers (e.g. quintiles)
                var categoryComparer = _comparer as ICategoryComparer;

                var indicatorMetadata = _indicatorMetadataProvider.GetIndicatorMetadata(
                    new List<Grouping> {grouping}, indicatorMetadataTextOption).First();

                var timePeriodFormatter = new TimePeriodTextFormatter(indicatorMetadata);
                var timePeriods = grouping.GetTimePeriodIterator(indicatorMetadata.YearType).TimePeriods;

                // Data collectors for calculating recent trends
                var coreDataCollector = new MultipleCoreDataCollector();

                var isLastPeriod = false;
                var count = 0;
                foreach (var timePeriod in timePeriods)
                {
                    count++;
                    if (count == timePeriods.Count)
                    {
                        isLastPeriod = true;
                    }
                    string timeString = timePeriodFormatter.Format(timePeriod);
                    int? sortableTimePeriod = includeSortableTimePeriod ?
                        timePeriod.ToSortableNumber() : (int?)null;

                    try
                    {
                        // England data
                        var englandCoreDatas = groupDataReader.GetDataIncludingInequalities(grouping, timePeriod,
                            excludedCategoryTypeIds, england.Code);
                        var englandCoreDatasForComparison =
                            englandCoreDatas.Where(x => x.CategoryTypeId == CategoryTypeIds.Undefined).ToList();

                        foreach (var coreData in englandCoreDatas)
                        {
                            // England significance
                            var toEnglandSignificance = Significance.None;
                            if (categoryComparer == null)
                            {
                                // If category type is undefined then would be comparing same value
                                if (coreData.CategoryTypeId != CategoryTypeIds.Undefined)
                                {
                                    var englandData = FindMatchingCoreDataSet(englandCoreDatasForComparison, coreData);
                                    toEnglandSignificance = GetSignificance(coreData, englandData, indicatorMetadata);
                                }
                            }

                            // Latest trend
                            TrendMarkerResult trendMarkerResult = null;
                            if (isLastPeriod)
                            {
                                var trendDataList = coreDataCollector.GetDataListForEngland(coreData);
                                trendDataList.Add(coreData);
                                trendMarkerResult = trendMarkersProvider.GetTrendMarkerResult(indicatorMetadata,
                                    coreData.YearRange, trendDataList);
                            }

                            fileWriter.WriteData(indicatorMetadata, coreData, timeString, null, trendMarkerResult,
                                toEnglandSignificance,
                                Significance.None);
                        }
                        coreDataCollector.AddEnglandDataList(englandCoreDatas);

                        // Subnational data
                        var parentCoreDatas = groupDataReader.GetDataIncludingInequalities(grouping, timePeriod,
                            excludedCategoryTypeIds, _areaHelper.ParentAreaCodes);
                        var parentCoreDatasForComparison =
                            parentCoreDatas.Where(x => x.CategoryTypeId == CategoryTypeIds.Undefined).ToList();
                        foreach (var coreData in parentCoreDatas)
                        {
                            // England significances
                            var toEnglandSignificance = Significance.None;
                            if (categoryComparer == null)
                            {
                                var englandData = FindMatchingCoreDataSet(englandCoreDatasForComparison, coreData);
                                toEnglandSignificance = GetSignificance(coreData, englandData, indicatorMetadata);
                            }

                            // Recent trend
                            TrendMarkerResult trendMarkerResult = null;
                            if (isLastPeriod)
                            {
                                var trendDataList = coreDataCollector.GetDataListForParentArea(coreData);
                                trendDataList.Add(coreData);
                                trendMarkerResult = trendMarkersProvider.GetTrendMarkerResult(indicatorMetadata,
                                    coreData.YearRange, trendDataList);
                            }

                            fileWriter.WriteData(indicatorMetadata, coreData, timeString, england, trendMarkerResult,
                                toEnglandSignificance, Significance.None);
                        }
                        coreDataCollector.AddParentDataList(parentCoreDatas);

                        // Child data
                        var childCoreDatas = groupDataReader.GetDataIncludingInequalities(grouping, timePeriod,
                            excludedCategoryTypeIds, _areaHelper.ChildAreaCodes);
                        var childAreaCodeToParentAreaMap = _areaHelper.ChildAreaCodeToParentAreaMap;

                        // Set the data for category comparators
                        CategoryComparisonManager categoryComparisonManager = null;
                        if (categoryComparer != null)
                        {
                            categoryComparisonManager = new CategoryComparisonManager(categoryComparer, childCoreDatas);
                            categoryComparisonManager.InitNationalData(englandCoreDatasForComparison);
                            categoryComparisonManager.InitSubnationalParentData(
                                _areaHelper.ParentAreaCodeToChildAreaCodesMap,
                                parentCoreDatasForComparison);
                        }

                        foreach (var coreData in childCoreDatas)
                        {
                            // Skip areas that don't map to a parent (way to avoid legacy areas)
                            if (isParentEngland == false &&
                                childAreaCodeToParentAreaMap.ContainsKey(coreData.AreaCode) == false) continue;

                            // England significances
                            Significance toEnglandSignificance;
                            if (categoryComparisonManager != null)
                            {
                                // Find category
                                toEnglandSignificance = categoryComparisonManager.GetCategory(coreData,
                                    AreaCodes.England);
                            }
                            else
                            {
                                // Benchmark comparison
                                var englandData = FindMatchingCoreDataSet(englandCoreDatasForComparison, coreData);
                                toEnglandSignificance = GetSignificance(coreData, englandData, indicatorMetadata);
                            }

                            // Subnational significances
                            Significance toSubnationalParentSignificance = Significance.None;
                            IArea parentArea = null;
                            if (isParentEngland)
                            {
                                parentArea = england;
                            }
                            else
                            {
                                if (childAreaCodeToParentAreaMap.ContainsKey(coreData.AreaCode))
                                {
                                    parentArea = childAreaCodeToParentAreaMap[coreData.AreaCode];
                                    if (categoryComparisonManager != null)
                                    {
                                        // Find category
                                        toSubnationalParentSignificance = categoryComparisonManager.GetCategory(
                                            coreData,
                                            parentArea.Code);
                                    }
                                    else
                                    {
                                        // Benchmark comparison
                                        var parentData = FindMatchingCoreDataSet(parentCoreDatasForComparison, coreData,
                                            parentArea.Code);
                                        toSubnationalParentSignificance = GetSignificance(coreData, parentData,
                                            indicatorMetadata);
                                    }
                                }
                            }

                            // Trend marker
                            TrendMarkerResult trendMarkerResult = null;
                            if (isLastPeriod)
                            {
                                var trendDataList = coreDataCollector.GetDataListForChildArea(coreData);
                                trendDataList.Add(coreData);
                                trendMarkerResult = trendMarkersProvider.GetTrendMarkerResult(indicatorMetadata,
                                    coreData.YearRange, trendDataList);
                            }

                            fileWriter.WriteData(indicatorMetadata, coreData, timeString,
                                parentArea, trendMarkerResult,
                                toEnglandSignificance, toSubnationalParentSignificance, sortableTimePeriod);
                        }
                        coreDataCollector.AddChildDataList(childCoreDatas);

                    }
                    catch (Exception ex)
                    {
                        throw new FingertipsException(string.Format(
                            "Could be build data for indicator {0} and time period {1}", indicatorId, timePeriod), ex);
                    }
                }

                // Add file contents
                var indicatorFileContents = fileWriter.GetFileContent();
                _fileBuilder.AddContent(indicatorFileContents);
            }

            return _fileBuilder.GetFileContent();
        }

        /// <summary>
        /// Whether to user generic metadata or metadata specific to the profile
        /// </summary>
        private static IndicatorMetadataTextOptions GetIndicatorMetadataTextOption(int profileId)
        {
            IndicatorMetadataTextOptions indicatorMetadataTextOption = profileId == ProfileIds.Undefined ||
                                                                       profileId == ProfileIds.Search
                ? IndicatorMetadataTextOptions.UseGeneric
                : IndicatorMetadataTextOptions.OverrideGenericWithProfileSpecific;
            return indicatorMetadataTextOption;
        }

        private void InitComparer(Grouping grouping)
        {
            _comparer = null;
            if (grouping.IsComparatorDefined())
            {
                _comparer = new IndicatorComparerFactory { PholioReader = _pholioReader }.New(grouping);
            }
        }

        private void InitFileWriter(Grouping grouping, SingleIndicatorFileWriter fileWriter)
        {
            var trendMarkerLabelProvider = new TrendMarkerLabelProvider(grouping.PolarityId);
            var significanceFormatter = new SignificanceFormatter(grouping.PolarityId, grouping.ComparatorMethodId);
            fileWriter.Init(LookUpManager, trendMarkerLabelProvider, significanceFormatter);
        }

        /// <summary>
        /// Lazily initialise look up manager. May not need these if all files are already cached.
        /// </summary>
        private LookUpManager LookUpManager
        {
            get
            {
                if (_lookUpManager == null)
                {
                    // Init look up manager
                    var allAreaTypes = new List<int>
                        {
                            AreaTypeIds.Country,
                            _parameters.ParentAreaTypeId,
                            _parameters.ChildAreaTypeId
                        };
                    var categoryTypeIds = _areasReader.GetAllCategoryTypes().Select(x => x.Id).ToList();
                    _lookUpManager = new LookUpManager(_pholioReader, _areasReader, allAreaTypes, categoryTypeIds);
                }
                return _lookUpManager;
            }
        }

        private Significance GetSignificance(CoreDataSet coreData, CoreDataSet englandData, IndicatorMetadata indicatorMetadata)
        {
            if (_comparer != null)
            {
                return _comparer.Compare(coreData, englandData, indicatorMetadata);
            }
            return Significance.None;
        }

        /// <summary>
        /// Finds the matching CoreDataSet object in the list
        /// </summary>
        private CoreDataSet FindMatchingCoreDataSet(IList<CoreDataSet> dataList, CoreDataSet coreData)
        {
            return dataList.FirstOrDefault(x =>
                            x.AgeId == coreData.AgeId &&
                            x.SexId == coreData.SexId);
        }

        /// <summary>
        /// Finds the matching CoreDataSet object in the list
        /// </summary>
        private CoreDataSet FindMatchingCoreDataSet(IList<CoreDataSet> dataList, CoreDataSet coreData, string areaCode)
        {
            return dataList.FirstOrDefault(x =>
                            x.AreaCode == areaCode &&
                            x.AgeId == coreData.AgeId &&
                            x.SexId == coreData.SexId);
        }

        /// <summary>
        /// Gets the column headers in CSV format
        /// </summary>
        private byte[] GetHeader()
        {
            var csvWriter = new CsvWriter();

            var headings = new List<object>
            {
                "Indicator ID", "Indicator Name", "Parent Code", "Parent Name",
                "Area Code", "Area Name", "Area Type", "Sex", "Age",
                "Category Type", "Category",
                "Time period", "Value", "Lower CI limit", "Upper CI limit",
                "Count", "Denominator",
                "Value note", "Recent Trend",
                "Compared to England value or percentiles",
                "Compared to subnational parent value or percentiles"
            };

            if (_parameters.IncludeSortableTimePeriod)
            {
                headings.Add("Time period Sortable");
            }

            csvWriter.AddHeader(headings.ToArray());
            var bytes = csvWriter.WriteAsBytes();
            return bytes;
        }

    }
}