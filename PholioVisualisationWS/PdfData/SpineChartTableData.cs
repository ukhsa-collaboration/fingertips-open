using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.PdfData
{
    public class SpineChartTableData : DomainData
    {
        public List<SpineChartTableRowData> IndicatorData { get; set; }

        public SpineChartTableData()
        {
            IndicatorData = new List<SpineChartTableRowData>();
        }
    }
}
