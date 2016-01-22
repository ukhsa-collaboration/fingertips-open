using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    /// <summary>
    /// Calculates suitable step for a chart axis from a previously 
    /// rounded minimum and maximum.
    /// </summary>
    public class ChartAxisIntervalCalculator
    {
        private const int MaxIntervalCount = 9;
        private const int MinIntervalCount = 5;

        public double? Step { get; private set; }
        private double difference;

        public ChartAxisIntervalCalculator(Limits limits)
        {
            difference = limits.Max - limits.Min;

            for (int i = 1000000; i >= 10; i /= 10)
            {
                TryStep(i);
            }

            TryToFindStepDivisibleBy10();

            if (Step == null)
            {
                Step = difference / 5;
            }
        }

        private void TryToFindStepDivisibleBy10()
        {
            if (Step.HasValue == false)
            {
                // Number of ticks will be interval count + 1
                for (int intervalCount = MaxIntervalCount;
                    intervalCount >= MinIntervalCount; intervalCount--)
                {
                    var step = difference / intervalCount;

                    if ((step % 10).Equals(0))
                    {
                        Step = step;
                        break;
                    }
                }
            }
        }

        private void TryStep(double step)
        {
            if (Step.HasValue == false)
            {
                var intervalCount = difference / step;
                if (intervalCount >= MinIntervalCount && intervalCount <= MaxIntervalCount)
                {
                    Step = step;
                }
            }
        }
    }
}
