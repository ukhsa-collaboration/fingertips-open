
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    /// <summary>
    /// Properties derived from a series of CoreDataSeries objects.
    /// </summary>
    public class IndicatorStatsPercentiles : Limits
    {
        [JsonProperty(PropertyName = "P25")]
        public double Percentile25 { get; set; }

        [JsonProperty(PropertyName = "P75")]
        public double Percentile75 { get; set; }
    }
}
