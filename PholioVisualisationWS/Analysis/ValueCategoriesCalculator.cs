using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.Analysis
{
    public abstract class ValueCategoriesCalculator
    {
        /// <summary>
        /// List of sorted valid values
        /// </summary>
        private List<double> _values;

        protected ValueCategoriesCalculator(IEnumerable<double> validvalues)
        {
            if (validvalues != null)
            {
                _values = validvalues.ToList();
                _values.Sort();
            }
        }

        protected bool AreEnoughValuesToCalculatePercentiles
        {
            get { return _values != null && _values.Count > 3; }
        }

        protected bool AreAnyValues
        {
            get { return _values != null && _values.Any(); }
        }

        protected double Min
        {
            get { return _values.Min<double>(); }
        }

        protected double Max
        {
            get { return _values.Max<double>(); }
        }

        protected double CalculatePercentile(double percentileFraction)
        {
            int count = _values.Count;
            double n = (Convert.ToDouble(count) - 1) * percentileFraction + 1;

            // Is Min
            if (n == 1d) return _values[0];

            // Is Max
            if (n == count) return _values[count - 1];

            int k = (int)n;
            double d = n - k;
            var percentile = _values[k - 1] + d * (_values[k] - _values[k - 1]);

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
