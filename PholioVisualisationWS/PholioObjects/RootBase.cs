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

        [JsonIgnore]
        public int SexId { get; set; }

        [JsonProperty]
        public bool StateSex { get; set; }

        [JsonProperty]
        public Sex Sex { get; set; }

        [JsonProperty]
        public Age Age { get; set; }

        [JsonProperty]
        public bool StateAge { get; set; }

        [JsonIgnore]
        public int AgeId { get; set; }

        [JsonProperty]
        public int YearRange { get; set; }

        [JsonProperty]
        public int PolarityId { get; set; }

        [JsonProperty]
        public int ComparatorMethodId { get; set; }

        [JsonProperty(PropertyName = "RecentTrends")]
        public Dictionary<string, TrendMarkerResult> RecentTrends { get; set; }
    }
}
