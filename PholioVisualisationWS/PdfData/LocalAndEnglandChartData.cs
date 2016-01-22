using System.Collections.Generic;
using PholioVisualisation.Analysis;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class LocalAndEnglandChartData
    {
        public List<double> Local { get; set; }
        public List<double> England { get; set; }
        public LimitsWithStep YAxis { get; set; }

        public void CalculateLimitsAndStep(double min, double max)
        {
            Limits roundedLimits = new MinMaxRounder(min, max).Limits;
            double step = new ChartAxisIntervalCalculator(roundedLimits).Step.Value;
            YAxis = new LimitsWithStep(roundedLimits, step);
        }
    }
}