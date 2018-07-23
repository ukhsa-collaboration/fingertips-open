using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class ValueData
    {
        public const double NullValue = -1.0;

        [JsonProperty(PropertyName = "Val")]
        public double Value { get; set; }

        [JsonProperty(PropertyName = "ValF")]
        public string ValueFormatted { get; set; }

        [JsonProperty(PropertyName = "Count")]
        public double? Count { get; set; }

        [JsonIgnore]
        public bool HasFormattedValue
        {
            get { return ValueFormatted != null; }
        }

        [JsonIgnore]
        public bool IsValueValid
        {
            get { return Value != NullValue; }
        }

        public ValueData GetValueData()
        {
            return new ValueData
            {
                Value = Value,
                ValueFormatted = ValueFormatted,
                Count = Count
            };
        }

        /// <summary>
        ///     Whether or not ValF should be serialised by JSON.NET.
        /// </summary>
        public bool ShouldSerializeValueFormatted()
        {
            return ValueFormatted != null;
        }

        protected static void WarnNotToSet()
        {
            throw new FingertipsException("This property should never be set");
        }
    }
}