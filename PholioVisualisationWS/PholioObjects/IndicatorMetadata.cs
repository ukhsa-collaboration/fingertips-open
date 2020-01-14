
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        public bool AlwaysShowSpineChart { get; set; }

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
        public ConfidenceIntervalMethod ConfidenceIntervalMethod { get; set; }

        [JsonProperty]
        public IDictionary<string, string> Descriptive { get; set; }

        [JsonIgnore]
        public int? TargetId { get; set; }

        [JsonProperty(PropertyName = "Target")]
        public TargetConfig TargetConfig { get; set; }

        [JsonProperty]
        public DateTime? LatestChangeTimestampOverride { get; set; }

        /// <summary>
        /// String of comma-separated key value pairs
        /// </summary>
        [JsonProperty]
        public string SpecialCases { get; set; }

        [JsonProperty]
        public bool AlwaysShowSexWithIndicatorName { get; set; }

        [JsonProperty]
        public bool AlwaysShowAgeWithIndicatorName { get; set; }

        [JsonProperty]
        public bool ShouldAveragesBeCalculated { get; set; }

        [JsonProperty]
        public string Status { get; set; }

        [JsonProperty]
        public int DestinationProfileId { get; set; }

        [JsonIgnore]
        public bool HasSpecialCases
        {
            get { return SpecialCases != null; }
        }

        [JsonIgnore]
        public bool ShouldNewDataBeHighlighted { get; set; }

        [JsonIgnore]
        public bool HasTarget
        {
            get { return TargetConfig != null; }
        }

        [JsonIgnore]
        public string Name
        {
            get
            {
                return Descriptive[IndicatorMetadataTextColumnNames.Name];
            }
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
