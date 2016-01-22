
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class IndicatorMetadata
    {
        [JsonProperty(PropertyName = "IID")]
        public int IndicatorId { get; set; }

        [JsonProperty]
        public int ValueTypeId { get; set; }

        [JsonIgnore]
        public int UnitId { get; set; }

        [JsonIgnore]
        public int? DecimalPlacesDisplayed { get; set; }

        [JsonProperty]
        public int YearTypeId { get; set; }

        [JsonProperty]
        public Unit Unit { get; set; }

        [JsonProperty]
        public ValueType ValueType { get; set; }

        [JsonIgnore]
        public YearType YearType { get; set; }

        [JsonProperty]
        public double ConfidenceLevel { get; set; }

        [JsonProperty(PropertyName = "CIMethodId")]
        public int ConfidenceIntervalMethodId { get; set; }

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
