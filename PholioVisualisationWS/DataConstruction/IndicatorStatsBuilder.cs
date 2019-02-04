using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataSorting;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class IndicatorStatsBuilder
    {
        private IndicatorStatsProcessor _indicatorStatsProcessor = new IndicatorStatsProcessor();
        private IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();
        private IAreasReader _areasReader = ReaderFactory.GetAreasReader();
        private PholioReader _pholioReader = ReaderFactory.GetPholioReader();
        private IProfileReader _profileReader = ReaderFactory.GetProfileReader();

        private TimePeriodTextFormatter _timePeriodFormatter;
        private int _childAreaCount;
        private CcgPopulation _ccgPopulation;
        private IList<string> _areaCodesToIgnore;
        private bool? _shouldShowSpineChart;
        private IArea _parentArea;
        private IndicatorMetadata _indicatorMetadata;

        public IndicatorStatsBuilder(IndicatorMetadata indicatorMetadata,
            ParentArea parentArea, int profileId)
        {
            _indicatorMetadata = indicatorMetadata;
            Init(parentArea, profileId);
        }

        public IndicatorStats GetIndicatorStats(Grouping grouping, TimePeriod timePeriod)
        {
            var formattedTimePeriod = _timePeriodFormatter.Format(timePeriod);
            IEnumerable<double> values = GetValuesForStats(grouping, timePeriod, _indicatorMetadata);

            IndicatorStats indicatorStats;
            if (values != null)
            {
                IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsCalculator(values).GetStats();
                var formatter = new NumericFormatterFactory(_groupDataReader).New(_indicatorMetadata);
                _indicatorStatsProcessor.Truncate(statsPercentiles);

                indicatorStats = new IndicatorStats
                {
                    IID = _indicatorMetadata.IndicatorId,
                    Sex = grouping.Sex,
                    Age = grouping.Age,
                    Stats = statsPercentiles,
                    StatsF = formatter.FormatStats(statsPercentiles),
                    HaveRequiredValues = _shouldShowSpineChart,
                    Period = formattedTimePeriod,
                    Limits = new LimitsBuilder().GetLimits(values.ToList())
                };
            }
            else
            {
                // No stats calculated
                indicatorStats = new IndicatorStats
                {
                    IID = _indicatorMetadata.IndicatorId,
                    Sex = grouping.Sex,
                    Age = grouping.Age,
                    HaveRequiredValues = _shouldShowSpineChart,
                    Period = formattedTimePeriod
                };
            }
            return indicatorStats;
        }

        private void Init(ParentArea parentArea, int profileId)
        {
            _timePeriodFormatter = new TimePeriodTextFormatter(_indicatorMetadata);

            // Set area codes to ignore
            _areaCodesToIgnore = _profileReader.GetAreaCodesToIgnore(profileId).AreaCodesIgnoredForSpineChart;

            _parentArea = AreaFactory.NewArea(_areasReader, parentArea.AreaCode);
            if (_parentArea.IsCcg)
            {
                _ccgPopulation = new CcgPopulationProvider(_pholioReader).GetPopulation(_parentArea.Code);
            }

            _childAreaCount = new ChildAreaCounter(_areasReader)
                .GetChildAreasCount(_parentArea, parentArea.ChildAreaTypeId);
        }

        private IEnumerable<double> GetValuesForStats(Grouping grouping, TimePeriod timePeriod,
            IndicatorMetadata metadata)
        {
            IList<CoreDataSet> data = null;
            IList<double> values;

            if (_parentArea.IsCountry)
            {
                // Optimisation for large number of areas
                values = _groupDataReader.GetOrderedCoreDataValidValuesForAllAreasOfType(grouping, timePeriod,
                    _areaCodesToIgnore);
            }
            else
            {
                data = new CoreDataSetListProvider(_groupDataReader).GetChildAreaData(grouping, _parentArea, timePeriod);
                data = new CoreDataSetFilter(data).RemoveWithAreaCode(_areaCodesToIgnore).ToList();
                data = data.OrderBy(x => x.Value).ToList();
                values = new ValueListBuilder(data).ValidValues;
            }

            // Apply rules
            _shouldShowSpineChart = IsRequiredNumberOfAreaValues(values) || metadata.AlwaysShowSpineChart;
            int areaTypeId = grouping.AreaTypeId;
            if (areaTypeId != AreaTypeIds.GpPractice)
            {
                if (_shouldShowSpineChart == false)
                {
                    values = null;
                }
            }
            else if (_parentArea.IsCcg)
            {
                // CCG average of GP practices
                if (RuleShouldCcgAverageBeCalculated.Validates(grouping, data, _ccgPopulation) == false)
                {
                    values = null;
                }
            }

            return values;
        }

        /// <summary>
        /// Rule that values must be available for at least 75% of the child areas.
        /// </summary>
        private bool IsRequiredNumberOfAreaValues(IEnumerable<double> values)
        {
            if (_childAreaCount > 0)
            {
                double fractionOfAreasWithValues = Convert.ToDouble(values.Count()) /
                    Convert.ToDouble(_childAreaCount);

                return fractionOfAreasWithValues >= 0.75;
            }

            return false;
        }
    }
}