
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class IndicatorMetadataTextProperty
    {
        [JsonIgnore]
        public int PropertyId { get; set; }

        [JsonProperty]
        public string ColumnName { get; set; }

        [JsonProperty]
        public string DisplayName { get; set; }

        [JsonIgnore]
        public string Definition { get; set; }

        [JsonIgnore]
        public bool IsHtml { get; set; }

        [JsonIgnore]
        public bool IsMandatory { get; set; }

        [JsonIgnore]
        public bool IsSystemContent { get; set; }

        [JsonProperty(PropertyName = "Order")]
        public int DisplayOrder { get; set; }

        [JsonIgnore]
        public int SearchBoost { get; set; }
    }
}
