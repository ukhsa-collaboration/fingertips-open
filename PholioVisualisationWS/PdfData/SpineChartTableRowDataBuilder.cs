using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class SpineChartTableRowDataBuilder : IndicatorDataBuilder<SpineChartTableRowData>
    {
        public bool IncludeTrendMarkers;

        private IList<IArea> _areas;
        private readonly Dictionary<string, CoreDataSetProvider> _coreDataSetProviders =
            new Dictionary<string, CoreDataSetProvider>();

        private readonly PholioReader _pholioReader = ReaderFactory.GetPholioReader();
        private readonly IAreasReader _areasReader = ReaderFactory.GetAreasReader();
        private readonly ITrendMarkersProvider _trendMarkersProvider = TrendMarkersProvider.New();

        private int _profileId;
        private SpineChartTableRowData _currentRow;

        public SpineChartTableRowDataBuilder(int profileId, IList<string> areaCodes)
        {
            _profileId = profileId;
            InitAreas(areaCodes);
            InitCoreDataSetProviders();
        }

        private void InitAreas(IList<string> areaCodes)
        {
            var areaFactory = new AreaFactory(_areasReader);
            _areas = new List<IArea>();
            foreach (string areaCode in areaCodes)
            {
                var area = areaFactory.NewArea(areaCode);
                _areas.Add(area);
            }
        }

        protected override IndicatorData IndicatorData
        {
            get { return _currentRow; }
        }

        public override SpineChartTableRowData GetIndicatorData(GroupRoot groupRoot, IndicatorMetadata metadata,
            IList<IArea> benchmarkAreas)
        {
            _currentRow = new SpineChartTableRowData();

            // Subnational spine charts not supported
            Grouping grouping = groupRoot.GetNationalGrouping();

            SetIndicatorData(groupRoot, metadata, benchmarkAreas);
            ReformatIndicatorDataForHealthProfiles();
            SetLongIndicatorName(groupRoot, metadata);
            _currentRow.ComparatorMethodId = grouping.ComparatorMethodId;
            SetStats(grouping, timePeriod, metadata);
            AssignChildAreaData(grouping, timePeriod, metadata);

            return _currentRow;
        }

        private void ReformatIndicatorDataForHealthProfiles()
        {
            if (_profileId == ProfileIds.HealthProfiles)
            {
                // Overwrite formatter defined in SetIndicatorData (for Health Profiles PDF)
                formatter = new FixedDecimalPlaceFormatter(1);
                // Reformat benchmark data defined in SetIndicatorData (for Health Profiles PDF)
                ReformatBenchmarkDataForHealthProfiles();
            }
        }

        private void ReformatBenchmarkDataForHealthProfiles()
        {
            var dataList = IndicatorData.BenchmarkData;
            var dataProcessor = new CoreDataProcessor(formatter);
            foreach (var benchmarkData in dataList.Values)
            {
                formatter.Format(benchmarkData);
                dataProcessor.Truncate(benchmarkData);
            }
        }

        private void SetLongIndicatorName(GroupRoot groupRoot, IndicatorMetadata metadata)
        {
            var textMetadata = metadata.Descriptive;
            if (textMetadata.ContainsKey(IndicatorMetadataTextColumnNames.NameLong) == false)
            {
                throw new FingertipsException("Indicator long name not defined for " + metadata.IndicatorId);
            }
            string longName = metadata.Descriptive[IndicatorMetadataTextColumnNames.NameLong];
            _currentRow.LongName = SexTextAppender.GetIndicatorName(longName,
                groupRoot.SexId, groupRoot.StateSex);
        }

        private void InitCoreDataSetProviders()
        {
            var providerfactory = new CoreDataSetProviderFactory();
            foreach (IArea area in _areas)
            {
                _coreDataSetProviders.Add(area.Code, providerfactory.New(area));
            }
        }

        private void SetStats(Grouping grouping, TimePeriod timePeriod, IndicatorMetadata indicatorMetadata)
        {
            var parentArea = new ParentArea(AreaCodes.England, grouping.AreaTypeId);
            var indicatorStatsResponse = new IndicatorStatsBuilder().GetIndicatorStats(timePeriod, grouping,
                indicatorMetadata, parentArea, _profileId);
            var stats = indicatorStatsResponse.Stats;
            if (stats != null)
            {
                _currentRow.Min = Truncate(stats.Min);
                _currentRow.Percentile25 = Truncate(stats.Percentile25);
                _currentRow.Percentile75 = Truncate(stats.Percentile75);
                _currentRow.Max = Truncate(stats.Max);

                var formattedStats = indicatorStatsResponse.StatsF;
                _currentRow.MaxF = NumberCommariser.CommariseFormattedValue(formattedStats.Max);
                _currentRow.MinF = NumberCommariser.CommariseFormattedValue(formattedStats.Min);

                var haveRequiredValues = indicatorStatsResponse.HaveRequiredValues;
                _currentRow.HasEnoughValuesForSpineChart = haveRequiredValues ?? false;
            }
        }

        private double Truncate(double d)
        {
            return DataProcessor<ValueData>.Round(d);
        }

        private void AssignChildAreaData(Grouping grouping, TimePeriod timePeriod, IndicatorMetadata metadata)
        {
            var targetComparerProvider = GetTargetComparerProvider();
            var indicatorComparisonHelper = new IndicatorComparisonHelper(metadata, grouping, groupDataReader,
                _pholioReader, targetComparerProvider);
            var dataProcessor = new CoreDataProcessor(formatter);

            var areaData = _currentRow.AreaData;
            var areaRecentTrends = _currentRow.AreaRecentTrends;

            foreach (IArea area in _areas)
            {
                var areaCode = area.Code;
                CoreDataSet data = _coreDataSetProviders[areaCode].GetData(grouping, timePeriod, metadata);
                TrendMarkerResult trendMarkerResult = null;

                if (data != null)
                {
                    indicatorComparisonHelper.AssignCategoryDataIfRequired(null
                        /*data assumed to always be national*/);

                    data.SignificanceAgainstOneBenchmark =
                        indicatorComparisonHelper.GetSignificance(data, grouping.ComparatorData);

                    if (IncludeTrendMarkers)
                    {
                        trendMarkerResult = _trendMarkersProvider.GetTrendResults(
                            new List<IArea> {area}, metadata, grouping).Values.First();
                    }

                    // Truncate after significance has been calculated
                    dataProcessor.FormatAndTruncate(data);
                }

                areaData.Add(areaCode, data);
                if (IncludeTrendMarkers)
                {
                    areaRecentTrends.Add(areaCode, trendMarkerResult);
                }
            }
        }

        private TargetComparerProvider GetTargetComparerProvider()
        {
            // Never benchmark against target in health profiles PDFs but use target for all other profiles
            return _profileId == ProfileIds.HealthProfiles
                ? null
                : new TargetComparerProvider(ReaderFactory.GetGroupDataReader(), _areasReader);
        }
    }
}