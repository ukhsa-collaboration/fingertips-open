
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class IndicatorStatsCalculator : ValueCategoriesCalculator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="validValuesList">A list of valid values</param>
        public IndicatorStatsCalculator(IEnumerable<double> validValuesList)
            : base(validValuesList) { }

        public IndicatorStatsPercentiles GetStats()
        {
            if (AreEnoughValuesToCalculatePercentiles)
            {
                return new IndicatorStatsPercentiles
                           {
                               Min = Min,
                               Max = Max,
                               Percentile25 = CalculatePercentile(0.25),
                               Percentile75 = CalculatePercentile(0.75)
                           };
            }
            return null;
        }

        public IndicatorStatsPercentiles GetStatsWithoutPercentiles()
        {
            if (AreAnyValues)
            {
                return new IndicatorStatsPercentiles
                    {
                        Min = Min,
                        Max = Max
                    };
            }
            return null;
        }
    }
}
