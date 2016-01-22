

using Newtonsoft.Json;

namespace PholioVisualisation.Analysis
{
    public class ControlLimits
    {
        // Property name is lowercase x to be consistent with Highcharts
        [JsonProperty(PropertyName = "x")]
        public double Population { get; set; }

        [JsonProperty]
        public double L2 { get; set; }

        [JsonProperty]
        public double L3 { get; set; }

        [JsonProperty]
        public double U2 { get; set; }

        [JsonProperty]
        public double U3 { get; set; }
    }
}
