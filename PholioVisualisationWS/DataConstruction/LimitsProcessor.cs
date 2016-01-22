using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class LimitsProcessor
    {
        public void TruncateList(IList<Limits> limitsList)
        {
            foreach (var limits in limitsList)
            {
                if (limits != null)
                {
                    limits.Min = Round(limits.Min);
                    limits.Max = Round(limits.Max);
                }
            }
        }

        private double Round(double d)
        {
            return DataProcessor<object>.Round(d);
        }

    }
}
