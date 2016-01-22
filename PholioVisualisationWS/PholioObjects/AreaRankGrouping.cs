using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    /// <summary>
    /// Provides context of indicator and limits for area rank 
    /// </summary>
    public class AreaRankGrouping
    {
        [JsonProperty(PropertyName = "Period")]
        public string TimePeriodText { get; set; }

        [JsonProperty]
        public AreaRank AreaRank { get; set; }

        [JsonProperty]
        public AreaRank Min { get; set; }

        [JsonProperty]
        public AreaRank Max { get; set; }

    }
}
