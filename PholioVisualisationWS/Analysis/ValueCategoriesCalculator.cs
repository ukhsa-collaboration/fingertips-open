using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.Analysis
{
    public abstract class ValueCategoriesCalculator
    {
        private List<double> values;

        protected ValueCategoriesCalculator(IEnumerable<double> validvalues)
        {
            if (validvalues != null)
            {
                values = validvalues.ToList();
                values.Sort();
            }
        }

        protected bool AreEnoughValuesToCalculatePercentiles
        {
            get { return values != null && values.Count > 3; }
        }

        protected bool AreAnyValues
        {
            get { return values != null && values.Any(); }
        }

        protected double Min
        {
            get { return values.Min<double>(); }
        }

        protected double Max
        {
            get { return values.Max<double>(); }
        }

        protected double CalculatePercentile(double percentileFraction)
        {
            int count = values.Count;
            double n = (Convert.ToDouble(count) - 1) * percentileFraction + 1;

            // Is Min
            if (n == 1d) return values[0];

            // Is Max
            if (n == count) return values[count - 1];

            int k = (int)n;
            double d = n - k;
            var percentile = values[k - 1] + d * (values[k] - values[k - 1]);

            // Correct for double calculation artifacts, e.g. 30.8 returned as 30.8000000000000000006
            return Math.Round(percentile, 10, MidpointRounding.AwayFromZero);
        }

        public IList<double> GetCategoryBounds(int categoryCount)
        {
            var list = new List<double>();

            if (AreEnoughValuesToCalculatePercentiles)
            {
                var step = 1.0 / Convert.ToDouble(categoryCount);

                list.Add(Min);
                for (int i = 1; i < categoryCount; i++)
                {
                    var fraction = step * i;
                    var percentile = CalculatePercentile(fraction);
                    list.Add(percentile);
                }
                list.Add(Max);
            }

            return list;
        }
    }
}
