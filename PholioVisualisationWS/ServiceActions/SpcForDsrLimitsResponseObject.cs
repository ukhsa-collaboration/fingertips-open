using PholioVisualisation.Analysis;
using System.Collections.Generic;

namespace PholioVisualisation.ServiceActions
{
    public class SpcForDsrLimitsResponseObject
    {
        public double ComparatorValue { get; set; }
        public List<ControlLimits> Points { get; set; }
    }
}
