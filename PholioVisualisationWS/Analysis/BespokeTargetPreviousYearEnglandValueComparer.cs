using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class BespokeTargetPreviousYearEnglandValueComparer : TargetComparer
    {
        /// <summary>
        /// The previous year's England data.
        /// </summary>
        public CoreDataSet BenchmarkData { get; set; }

        public BespokeTargetPreviousYearEnglandValueComparer(TargetConfig config)
            : base(config)
        {
        }

        public override Significance CompareAgainstTarget(CoreDataSet data)
        {
            // Note: Polarity is not considered

            if (BenchmarkData == null ||
                CanComparisonGoAhead(data) == false ||
                BenchmarkData.IsValueValid == false)
            {
                return Significance.None;
            }

            return data.Value < BenchmarkData.Value
                ? Significance.Worse
                : Significance.Better;
        }
    }

}