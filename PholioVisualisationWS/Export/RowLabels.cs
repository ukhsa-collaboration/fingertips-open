using System.Collections.Generic;

namespace PholioVisualisation.Export
{
    public class RowLabels
    {
        public string IndicatorName { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public string TimePeriod { get; set; }
        public IDictionary<int,string> ValueNoteLookUp { get; set; }
    }
}