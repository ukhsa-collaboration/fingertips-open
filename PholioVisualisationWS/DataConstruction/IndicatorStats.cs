using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class IndicatorStats
    {
        public int IID { get; set; }
        public Sex Sex { get; set; }
        public Age Age { get; set; }
        public IndicatorStatsPercentiles Stats { get; set; }
        public IndicatorStatsPercentilesFormatted StatsF { get; set; }
        public bool? HaveRequiredValues { get; set; }
        public string Period { get; set; }
        public Limits Limits { get; set; }
    }

}
