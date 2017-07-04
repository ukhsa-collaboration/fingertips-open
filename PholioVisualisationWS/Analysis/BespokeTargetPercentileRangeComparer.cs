using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class BespokeTargetPercentileRangeComparer : TargetComparer
    {
        public CoreDataSet BenchmarkData { get; set; }
        public CoreDataSet LowerTargetPercentileBenchmarkData { get; set; }
        public CoreDataSet UpperTargetPercentileBenchmarkData { get; set; }

        public BespokeTargetPercentileRangeComparer(TargetConfig config)
            : base(config)
        {
        }

        public double GetLowerTargetPercentile()
        {
            return (double) Config.LowerLimit / 100;
        }


        public double GetUpperTargetPercentile()
        {
            return (double) Config.UpperLimit / 100;
        }

        public override Significance CompareAgainstTarget(CoreDataSet data)
        {

            if (UpperTargetPercentileBenchmarkData == null 
                || LowerTargetPercentileBenchmarkData == null 
                || CanComparisonGoAhead(data) == false 
                || UpperTargetPercentileBenchmarkData.IsValueValid == false 
                || LowerTargetPercentileBenchmarkData.IsValueValid == false)
            {
                return Significance.None;
            }

            if (Config.PolarityId == PolarityIds.RagHighIsGood)
            {
                return data.Value < LowerTargetPercentileBenchmarkData.Value
                    ? Significance.Worse
                    : (data.Value >= UpperTargetPercentileBenchmarkData.Value ? Significance.Better : Significance.Same);
            }

            if (Config.PolarityId == PolarityIds.RagLowIsGood)
            {
                return data.Value < LowerTargetPercentileBenchmarkData.Value
                    ? Significance.Better
                    : (data.Value >= UpperTargetPercentileBenchmarkData.Value ? Significance.Worse : Significance.Same);

            }
            return Significance.None;
        }
    }

}