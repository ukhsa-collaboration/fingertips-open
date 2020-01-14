using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class TargetComparerDataAssigner
    {
        private IGroupDataReader groupDataReader;

        public TargetComparerDataAssigner(IGroupDataReader groupDataReader)
        {
            this.groupDataReader = groupDataReader;
        }

        public void AssignExtraDataIfRequired(IArea england, TargetComparer targetComparer, Grouping grouping,
            IndicatorMetadata indicatorMetadata, TimePeriod timePeriod)
        {
            var bespokeComparer = targetComparer as BespokeTargetPreviousYearEnglandValueComparer;
            if (bespokeComparer != null)
            {
                // Assign the previous year's England data
                var previousYear = timePeriod.GetTimePeriodForYearBefore();
                bespokeComparer.BenchmarkData =
                    new BenchmarkDataProvider(groupDataReader)
                        .GetBenchmarkData(grouping, previousYear,
                            null /*would need average calculator with previous year's data*/, england);
            }

            var bespokeTargetPreviousYearEnglandValueComparer = targetComparer as BespokeTargetPercentileRangeComparer;
            if (bespokeTargetPreviousYearEnglandValueComparer != null)
            {
                //Get the Upper and Lower Benchmark ranges
                new TargetComparerDataAssigner(groupDataReader)
                    .AssignBenchmarkPercentileDataToTargetComparer(targetComparer, grouping, indicatorMetadata, timePeriod);
            }
        }

        public void AssignBenchmarkPercentileDataToTargetComparer(TargetComparer targetComparer, Grouping grouping, 
            IndicatorMetadata indicatorMetadata, TimePeriod timePeriod)
        {
            var bespokeComparer = targetComparer as BespokeTargetPercentileRangeComparer;
            if (bespokeComparer != null)
            {
                // Upper tier LA values should always be used
                var utlaGrouping = new Grouping(grouping)
                {
                    AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019
                };
                var utlaValues = groupDataReader.GetCoreDataForAllAreasOfType(utlaGrouping, timePeriod);

                var percentileCalculator = new BespokeTargetPercentileRangeCalculator(utlaValues.Where(x => x.IsValueValid).Select(x => x.Value).ToList());

                bespokeComparer.LowerTargetPercentileBenchmarkData =
                    new CoreDataSet { Value = percentileCalculator.GetPercentileValue(bespokeComparer.GetLowerTargetPercentile()) };

                bespokeComparer.UpperTargetPercentileBenchmarkData =
                    new CoreDataSet { Value = percentileCalculator.GetPercentileValue(bespokeComparer.GetUpperTargetPercentile()) };
            }
        }

    }
}