
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class IndicatorMetadata
    {
        [JsonProperty(PropertyName = "IID")]
        public int IndicatorId { get; set; }

        [JsonIgnore]
        public int ValueTypeId { get; set; }

        [JsonIgnore]
        public int UnitId { get; set; }

        [JsonIgnore]
        public int ConfidenceIntervalMethodId { get; set; }

        [JsonIgnore]
        public int? DecimalPlacesDisplayed { get; set; }

        [JsonIgnore]
        public int YearTypeId { get; set; }

        [JsonProperty]
        public Unit Unit { get; set; }

        [JsonProperty]
        public ValueType ValueType { get; set; }

        [JsonProperty]
        public YearType YearType { get; set; }

        [JsonProperty]
        public double ConfidenceLevel { get; set; }

        [JsonProperty]
        public ConfidenceIntervalMethod ConfidenceIntervalMethod { get; set; }

        [JsonProperty]
        public IDictionary<string,string> Descriptive { get; set; }

        [JsonIgnore]
        public int? TargetId { get; set; }

        [JsonProperty(PropertyName = "Target")]
        public TargetConfig TargetConfig { get; set; }

        [JsonIgnore]
        public bool HasTarget {
            get { return TargetConfig != null; }
        }

        /// <summary>
        ///     Whether or not TargetConfig should be serialised to JSON.
        /// </summary>
        public bool ShouldSerializeTargetConfig()
        {
            return HasTarget;
        }
    }
}
