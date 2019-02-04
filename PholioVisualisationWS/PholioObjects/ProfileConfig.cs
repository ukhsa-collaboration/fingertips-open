using Newtonsoft.Json;

namespace PholioVisualisation.PholioObjects
{
    public class ProfileConfig
    {
        [JsonProperty]
        public int ProfileId { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Key")]
        public string UrlKey { get; set; }

        [JsonIgnore]
        public string AreaCodesIgnoredEverywhereString { get; set; }

        [JsonIgnore]
        public string AreaCodesIgnoredForSpineChartString { get; set; }

        /// <summary>
        /// Property is referred to in HNibernate string literals
        /// </summary>
        [JsonIgnore]
        public bool AreIndicatorsExcludedFromSearch { get; set; }

        [JsonProperty]
        public bool IsNational { get; set; }

        [JsonIgnore]
        public bool ShouldBuildExcel { get; set; }

        [JsonIgnore]
        public bool IsProfileLive { get; set; }

        [JsonProperty]
        public bool HasTrendMarkers { get; set; }

        [JsonIgnore]
        public int NewDataDeploymentCount { get; set; }

        [JsonIgnore]
        public int DefaultAreaTypeId { get; set; }

        [JsonIgnore]
        public bool HasAnyData { get; set; }
        
    }
}
