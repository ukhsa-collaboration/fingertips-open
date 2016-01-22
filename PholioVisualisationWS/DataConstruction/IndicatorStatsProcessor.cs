using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class IndicatorStatsProcessor
    {
        public void Truncate(IndicatorStatsPercentiles statsPercentiles)
        {
            if (statsPercentiles != null)
            {
                statsPercentiles.Max = RoundStats(statsPercentiles.Max);
                statsPercentiles.Min = RoundStats(statsPercentiles.Min);
                statsPercentiles.Percentile25 = RoundStats(statsPercentiles.Percentile25);
                statsPercentiles.Percentile75 = RoundStats(statsPercentiles.Percentile75);
            }
        }

        private static double RoundStats(double val)
        {
            return Math.Round(val, 5, MidpointRounding.AwayFromZero);
        }

    }
}
