using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class SpineChartTableRowData : IndicatorData
    {
        public string LongName { get; set; }
        public int ComparatorMethodId { get; set; }
        public bool HasEnoughValuesForSpineChart { get; set; }

        public double? Min { get; set; }
        public string MinF { get; set; }
        public double? Max { get; set; }
        public string MaxF { get; set; }

        public double? Percentile25 { get; set; }
        public double? Percentile75 { get; set; }

        public Dictionary<string, CoreDataSet> AreaData = new Dictionary<string, CoreDataSet>();
        public Dictionary<string, TrendMarkerResult> AreaRecentTrends = new Dictionary<string, TrendMarkerResult>();
    }
}
