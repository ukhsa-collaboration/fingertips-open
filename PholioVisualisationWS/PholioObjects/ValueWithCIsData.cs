
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class ValueWithCIsData : ValueData
    {
        public const double CountMinusOne = -1.000001;

        public ValueWithCIsData GetValueWithCIsData()
        {
            return new ValueWithCIsData
            {
                Value = Value,
                ValueFormatted = ValueFormatted,
                LowerCI95 = LowerCI95,
                LowerCI95F = LowerCI95F,
                UpperCI95 = UpperCI95,
                UpperCI95F = UpperCI95F,
                Count = Count
            };
        }

        [JsonIgnore]
        public bool HasFormattedCIs
        {
            get { return LowerCI95F != null && UpperCI95F != null; }
        }

        [JsonProperty(PropertyName = "LoCI")]
        public double? LowerCI95 { get; set; }

        [JsonProperty(PropertyName = "UpCI")]
        public double? UpperCI95 { get; set; }

        [JsonProperty(PropertyName = "LoCIF")]
        public string LowerCI95F { get; set; }

        [JsonProperty(PropertyName = "UpCIF")]
        public string UpperCI95F { get; set; }

        [JsonProperty(PropertyName = "LoCI99_8")]
        public double? LowerCI99_8 { get; set; }

        [JsonProperty(PropertyName = "UpCI99_8")]
        public double? UpperCI99_8 { get; set; }

        [JsonProperty(PropertyName = "LoCI99_8F")]
        public string LowerCI99_8F { get; set; }

        [JsonProperty(PropertyName = "UpCI99_8F")]
        public string UpperCI99_8F { get; set; }


        [JsonIgnore]
        public bool HasBeenTruncated { get; set; }

        /// <summary>
        /// Whether or not LowerCIF should be serialised by JSON.NET.
        /// </summary>
        public bool ShouldSerializeLowerCI95F()
        {
            return Are95CIsValid && LowerCI95F != null;
        }

        /// <summary>
        /// Whether or not UpperCIF should be serialised by JSON.NET.
        /// </summary>
        public bool ShouldSerializeUpperCI95F()
        {
            return Are95CIsValid && UpperCI95F != null;
        }

        /// <summary>
        /// Whether or not LowerCI should be serialised by JSON.NET.
        /// </summary>
        public bool ShouldSerializeLowerCI95()
        {
            return Are95CIsValid;
        }

        /// <summary>
        /// Whether or not UpperCI should be serialised by JSON.NET.
        /// </summary>
        public bool ShouldSerializeUpperCI95()
        {
            return Are95CIsValid;
        }

        /// <summary>
        /// Whether or not LowerCIF should be serialised by JSON.NET.
        /// </summary>
        public bool ShouldSerializeLowerCI99_8F()
        {
            return Are99_8CIsValid && LowerCI99_8F != null;
        }

        /// <summary>
        /// Whether or not UpperCIF should be serialised by JSON.NET.
        /// </summary>
        public bool ShouldSerializeUpperCI99_8F()
        {
            return Are99_8CIsValid && UpperCI99_8F != null;
        }

        /// <summary>
        /// Whether or not LowerCI should be serialised by JSON.NET.
        /// </summary>
        public bool ShouldSerializeLowerCI99_8()
        {
            return Are99_8CIsValid;
        }

        /// <summary>
        /// Whether or not UpperCI should be serialised by JSON.NET.
        /// </summary>
        public bool ShouldSerializeUpperCI99_8()
        {
            return Are99_8CIsValid;
        }

        [JsonIgnore]
        public bool Are95CIsValid
        {
            get
            {
                return UpperCI95.HasValue && LowerCI95.HasValue;
            }
        }

        [JsonIgnore]
        public bool Are99_8CIsValid
        {
            get
            {
                return UpperCI99_8.HasValue && LowerCI99_8.HasValue;
            }
        }

        public static ValueWithCIsData GetNullObject()
        {
            ValueWithCIsData data = new ValueWithCIsData();
            SetValueWithCIsDataDefaults(data);
            return data;
        }

        protected static void SetValueWithCIsDataDefaults(ValueWithCIsData data)
        {
            data.Value = NullValue;
            data.LowerCI95 = null;
            data.UpperCI95 = null;
        }

        /// <summary>
        /// Parse list of values.
        /// </summary>
        /// <param name="valueListString">e.g. 1,3,4</param>
        public static ValueWithCIsData Parse(string valueListString)
        {
            string[] bits = valueListString.Split(',');

            ValueWithCIsData data = null;

            if (bits.Length == 1)
            {
                data = new ValueWithCIsData { Value = double.Parse(bits[0]) };
            }
            else if (bits.Length == 3)
            {
                data = GetNullObject();

                if (CanValueBeParsed(bits[0]))
                {
                    // No data
                }
                else if (CanValueBeParsed(bits[1]))
                {
                    // No CIs
                    data.Value = double.Parse(bits[0]);
                }
                else
                {
                    data.Value = double.Parse(bits[0]);
                    data.LowerCI95 = double.Parse(bits[1]);
                    data.UpperCI95 = double.Parse(bits[2]);
                }
            }

            return data;
        }

        private static bool CanValueBeParsed(string s)
        {
            return s == "-" || string.IsNullOrWhiteSpace(s);
        }
    }
}
