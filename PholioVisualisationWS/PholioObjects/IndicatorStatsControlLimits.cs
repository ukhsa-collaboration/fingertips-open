using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class IndicatorStatsControlLimits : Limits
    {
        [JsonProperty(PropertyName = "U2")]
        public double UpperLimit2 { get; set; }

        [JsonProperty(PropertyName = "L2")]
        public double LowerLimit2 { get; set; }

        [JsonProperty(PropertyName = "U3")]
        public double UpperLimit3 { get; set; }

        [JsonProperty(PropertyName = "L3")]
        public double LowerLimit3 { get; set; }
    }
}
