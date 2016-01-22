
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.Analysis
{
    public class ControlLimitsBuilderSpcForDsr
    {
        public const double EventLimit = 101.0;

        // Comparator Method 6 
        private const double ControlLimit2 = 0.025; // 95%
        private const double ControlLimit3 = 0.001; // 99.8%

        public double YearRange { get; set; }
        public double UnitValue { get; set; }

        public List<ControlLimits> GetSpcForDsrLimits(double comparatorValue, double populationMin, double populationMax)
        {
            if (populationMin <= 0)
            {
                return null;
            }

            double minEvent = Math.Floor(populationMin * comparatorValue / UnitValue * YearRange);
            double maxEvent = Math.Ceiling(populationMax * comparatorValue / UnitValue * YearRange);

            double maxDenominator = populationMax * YearRange;

            double E1 = new[] { 1.0, minEvent }.Max();

            List<ControlLimits> points = new List<ControlLimits>();

            for (double rowNumber = 1; rowNumber < EventLimit; rowNumber++)
            {
                if (rowNumber > 1)
                {
                    double a = (maxEvent / E1);
                    double b = 1.0 / (EventLimit - rowNumber);
                    double optionB = Math.Round(Math.Pow(a, b) * E1);

                    E1 = new[] { E1 + 1, optionB }.Max();
                }

                double denominator = (E1 / comparatorValue) * UnitValue;

                points.Add(GetPoint(E1, denominator));

                if (denominator > maxDenominator)
                {
                    break;
                }
            }

            return points;
        }

        private ControlLimits GetPoint(double E1, double denominator)
        {
            ControlLimits point = new ControlLimits { Population = Math.Floor(denominator / YearRange) };

            // Round to reduce JSON footprint
            point.L2 = Math.Round((StatisticalMethods.PoisLow(E1, ControlLimit2) / denominator) * UnitValue, 3, MidpointRounding.AwayFromZero);
            point.U2 = Math.Round((StatisticalMethods.PoisHigh(E1, ControlLimit2) / denominator) * UnitValue, 3, MidpointRounding.AwayFromZero);
            point.L3 = Math.Round((StatisticalMethods.PoisLow(E1, ControlLimit3) / denominator) * UnitValue, 3, MidpointRounding.AwayFromZero);
            point.U3 = Math.Round((StatisticalMethods.PoisHigh(E1, ControlLimit3) / denominator) * UnitValue, 3, MidpointRounding.AwayFromZero);

            return point;
        }
    }
}
