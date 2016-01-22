using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Analysis
{
    public class MinMaxFinder
    {
        private List<double> allValues = new List<double>();

        public void AddRange(IEnumerable<double> values)
        {
            allValues.AddRange(values);
        }

        public Limits GetLimits()
        {
            if (allValues.Count <= 1)
            {
                return null;
            }

            return new Limits
            {
                Min = allValues.Min(),
                Max = allValues.Max()
            };
        }
    }
}