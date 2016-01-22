using System.Collections.Generic;
using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class RootBase
    {
        [JsonProperty(PropertyName = "IID")]
        public int IndicatorId { get; set; }

        [JsonIgnore]
        public int AreaTypeId { get; set; }

        [JsonProperty]
        public int SexId { get; set; }

        [JsonProperty]
        public bool StateSex { get; set; }

        [JsonProperty]
        public string AgeLabel { get; set; }

        [JsonProperty]
        public int PolarityId { get; set; }

        [JsonProperty]
        public int AgeId { get; set; }

        [JsonProperty]
        public int ComparatorMethodId { get; set; }

        [JsonProperty]
        public Dictionary<string, TrendMarker> TrendMarkers { get; set; }
    }
}
