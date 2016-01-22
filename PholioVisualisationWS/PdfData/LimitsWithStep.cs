using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class LimitsWithStep : Limits
    {
        public double? Step { get; set; }

        public LimitsWithStep(Limits limits, double? step)
        {
            if (limits != null)
            {
                Min = limits.Min;
                Max = limits.Max;
            }

            if (step.HasValue)
            {
                Step = step;
            }
        }
    }
}
