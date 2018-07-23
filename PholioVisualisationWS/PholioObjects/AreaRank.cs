using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class AreaRank
    {
        [JsonProperty]
        public IArea Area { get; set; }

        [JsonProperty(PropertyName = "Val")]
        public double Value { get; set; }

        [JsonProperty(PropertyName = "ValF")]
        public string ValueFormatted { get; set; }

        [JsonProperty]
        public double Count { get; set; }

        [JsonProperty]
        public int? Rank { get; set; }

        [JsonProperty]
        public double? Denom { get; set; }

        [JsonProperty]
        public ValueNote ValueNote { get; set; }
    }
}
