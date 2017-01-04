using System.Linq;
using PholioVisualisation.Analysis;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class TargetComparerHelper
    {
        private IGroupDataReader groupDataReader;
        private IArea nationalArea;

        public TargetComparerHelper(IGroupDataReader groupDataReader, IArea nationalArea)
        {
            this.groupDataReader = groupDataReader;
            this.nationalArea = nationalArea;
        }

        public void AssignExtraDataIfRequired(IArea parentArea, TargetComparer targetComparer, Grouping grouping, IndicatorMetadata indicatorMetadata)
        {
            var bespokeComparer = targetComparer as BespokeTargetPreviousYearEnglandValueComparer;
            if (bespokeComparer != null)
            {
                // Assign the previous year's England data
                var previousYear = TimePeriod.GetDataPoint(grouping).GetTimePeriodForYearBefore();
                bespokeComparer.BenchmarkData =
                    new BenchmarkDataProvider(groupDataReader)
                        .GetBenchmarkData(grouping, previousYear,
                            null /*would need average calculator with previous year's data*/, nationalArea);
            }

            var bespokeTargetPreviousYearEnglandValueComparer = targetComparer as BespokeTargetPercentileRangeComparer;
            if (bespokeTargetPreviousYearEnglandValueComparer != null)
            {
                //Get the Upper and Lower Benchmark ranges
                new TargetComparerHelper(groupDataReader, parentArea).GetPercentileData(targetComparer, grouping, indicatorMetadata);
            }
        }

        public void GetPercentileData(TargetComparer targetComparer, Grouping grouping, IndicatorMetadata indicatorMetadata)
        {
            var bespokeComparer = targetComparer as BespokeTargetPercentileRangeComparer;
            if (bespokeComparer != null)
            {
                // Upper tier LA values should always be used
                var utlaGrouping = new Grouping(grouping)
                {
                    AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthority
                };
                var utlaValues = groupDataReader.GetCoreDataForAllAreasOfType(utlaGrouping, TimePeriod.GetDataPoint(utlaGrouping));

                var percentileCalculator = new BespokeTargetPercentileRangeCalculator(utlaValues.Where(x => x.IsValueValid).Select(x => x.Value).ToList());

                bespokeComparer.LowerTargetPercentileBenchmarkData =
                    new CoreDataSet() { Value = percentileCalculator.GetPercentileValue(bespokeComparer.GetLowerTargetPercentile()) };

                bespokeComparer.UpperTargetPercentileBenchmarkData =
                    new CoreDataSet() { Value = percentileCalculator.GetPercentileValue(bespokeComparer.GetUpperTargetPercentile()) };
            }
        }

    }
}