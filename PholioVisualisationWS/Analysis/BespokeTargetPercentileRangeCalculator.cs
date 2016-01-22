using System.Collections.Generic;

namespace PholioVisualisation.Analysis
{
    public class BespokeTargetPercentileRangeCalculator : ValueCategoriesCalculator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="validValuesList">A list of valid values</param>
        public BespokeTargetPercentileRangeCalculator(IEnumerable<double> validValuesList)
            : base(validValuesList) { }

        public double GetPercentileValue(double percentile)
        {
            if (AreEnoughValuesToCalculatePercentiles)
            {
                return CalculatePercentile(percentile);
            }

            return 0;
        }
    }
}
