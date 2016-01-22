
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class IndicatorStatsPercentilesFormatted
    {
        [JsonProperty]
        public string Min { get; set; }

        [JsonProperty]
        public string Max { get; set; }

        [JsonProperty(PropertyName = "P25")]
        public string Percentile25 { get; set; }

        [JsonProperty(PropertyName = "P75")]
        public string Percentile75 { get; set; }
    }
}
