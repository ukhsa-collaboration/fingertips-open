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
        private IndicatorStatsProcessor indicatorStatsProcessor = new IndicatorStatsProcessor();
        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
        private IAreasReader areasReader = ReaderFactory.GetAreasReader();
        private PholioReader pholioReader = ReaderFactory.GetPholioReader();
        private IProfileReader profileReader = ReaderFactory.GetProfileReader();

        private TimePeriodTextFormatter _timePeriodFormatter;
        private bool _isInitialised;
        private int childAreaCount;
        private CcgPopulation ccgPopulation;
        private IList<string> areaCodesToIgnore;
        private bool? shouldShowSpineChart;
        private IArea _parentArea;

        public IndicatorStats GetIndicatorStats(TimePeriod timePeriod, Grouping grouping, 
            IndicatorMetadata indicatorMetadata, ParentArea parentArea, int profileId)
        {
            if(_isInitialised == false) Init(parentArea, profileId, indicatorMetadata);

            var formattedTimePeriod = _timePeriodFormatter.Format(timePeriod);
            IEnumerable<double> values = GetValuesForStats(grouping, timePeriod, indicatorMetadata);

            IndicatorStats indicatorStats;
            if (values != null)
            {
                IndicatorStatsPercentiles statsPercentiles = new IndicatorStatsCalculator(values).GetStats();
                var formatter = NumericFormatterFactory.New(indicatorMetadata, groupDataReader);
                indicatorStatsProcessor.Truncate(statsPercentiles);

                indicatorStats = new IndicatorStats
                {
                    IID = indicatorMetadata.IndicatorId,
                    Sex = grouping.Sex,
                    Age = grouping.Age,
                    Stats = statsPercentiles,
                    StatsF = formatter.FormatStats(statsPercentiles),
                    HaveRequiredValues = shouldShowSpineChart,
                    Period = formattedTimePeriod
                };
            }
            else
            {
                // No stats calculated
                indicatorStats = new IndicatorStats
                {
                    IID = indicatorMetadata.IndicatorId,
                    Sex = grouping.Sex,
                    Age = grouping.Age,
                    HaveRequiredValues = shouldShowSpineChart,
                    Period = formattedTimePeriod
                };
            }
            return indicatorStats;
        }

        private void Init(ParentArea parentArea, int profileId, IndicatorMetadata indicatorMetadata)
        {
            _isInitialised = true;

            _timePeriodFormatter = new TimePeriodTextFormatter(indicatorMetadata);

            // Set area codes to ignore
            areaCodesToIgnore = profileReader.GetAreaCodesToIgnore(profileId).AreaCodesIgnoredForSpineChart;

            _parentArea = AreaFactory.NewArea(areasReader, parentArea.AreaCode);
            if (_parentArea.IsCcg)
            {
                ccgPopulation = new CcgPopulationProvider(pholioReader).GetPopulation(_parentArea.Code);
            }

            childAreaCount = new ChildAreaCounter(areasReader)
                .GetChildAreasCount(_parentArea, parentArea.ChildAreaTypeId);
        }

        private IEnumerable<double> GetValuesForStats(Grouping grouping, TimePeriod timePeriod, IndicatorMetadata metadata)
        {
            IList<CoreDataSet> data = null;
            IList<double> values;

            if (_parentArea.IsCountry)
            {
                // Optimisation for large number of areas
                values = groupDataReader.GetOrderedCoreDataValidValuesForAllAreasOfType(grouping, timePeriod,
                    areaCodesToIgnore);
            }
            else
            {
                data = new CoreDataSetListProvider(groupDataReader).GetChildAreaData(grouping, _parentArea, timePeriod);
                data = new CoreDataSetFilter(data).RemoveWithAreaCode(areaCodesToIgnore).ToList();
                data = data.OrderBy(x => x.Value).ToList();
                values = new ValueListBuilder(data).ValidValues;
            }

            // Apply rules
            int areaTypeId = grouping.AreaTypeId;
            if (areaTypeId != AreaTypeIds.GpPractice)
            {
                shouldShowSpineChart = IsRequiredNumberOfAreaValues(values) || metadata.AlwaysShowSpineChart;

                if (shouldShowSpineChart == false)
                {
                    values = null;
                }
            }
            else if (_parentArea.IsCcg)
            {
                // CCG average of GP practices
                if (RuleShouldCcgAverageBeCalculated.Validates(grouping, data, ccgPopulation) == false)
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
            if (childAreaCount > 0)
            {
                double fractionOfAreasWithValues = Convert.ToDouble(values.Count()) /
                    Convert.ToDouble(childAreaCount);

                return fractionOfAreasWithValues >= 0.75;
            }

            return false;
        }
    }
}