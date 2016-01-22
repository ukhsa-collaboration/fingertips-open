
using System;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    /// <summary>
    /// Determines appropriate axis limits for a chart based on
    /// the minimum and maximum data values.
    /// </summary>
    public class MinMaxRounder
    {
        public Limits Limits { get; private set; }

        public MinMaxRounder(double min, double max)
        {
            // Perhaps add some padding with max *= 0.05
            double diff = Math.Abs(max - min);

            int order = 0;

            if (diff == 0)
            {
                // Skip
            }
            else if (diff >= 1)
            {
                while (diff >= 1)
                {
                    diff /= 10;
                    order++;
                }
            }
            else
            {
                while (diff <= 0.1)
                {
                    diff *= 10;
                    order--;
                }
            }

            double step = GetStep(diff);
            double bit = Math.Pow(10, order - 2);
            step *= Convert.ToDouble(bit);

            Limits = new Limits
            {
                Min = FloorNearest(min, step),
                Max = CeilNearest(max, step)
            };
        }

        private static double GetStep(double diff)
        {
            if (diff < 0.2)
            {
                return diff < 0.1
                    ? 2
                    : 5;
            }

            return diff < 0.5 ?
                10 :
                20;
        }

        private static double CeilNearest(double num, double step)
        {
            if (Math.Abs(step) < 0.0)
            {
                num *= step;
                num = Math.Ceiling(num);
                num /= step;
                return num;
            }

            // Greater than 1
            num /= step;
            num = Math.Ceiling(num);
            num *= step;
            return num;
        }

        private static double FloorNearest(double num, double step)
        {
            if (Math.Abs(step) < 0.0)
            {
                num *= step;
                num = Math.Floor(num);
                num /= step;
                return num;
            }

            // Greater than 1
            num /= step;
            num = Math.Floor(num);
            num *= step;
            return num;
        }
    }
}
