namespace PholioVisualisation.PholioObjects
{
    public class ProfileConfig
    {
        public int ProfileId { get; set; }
        public string Name { get; set; }
        public string UrlKey { get; set; }
        public string AreaCodesIgnoredEverywhereString { get; set; }
        public string AreaCodesIgnoredForSpineChartString { get; set; }

        /// <summary>
        /// Property is referred to in HNibernate string literals
        /// </summary>
        public bool AreIndicatorsExcludedFromSearch { get; set; }

        public bool IsNational { get; set; }
        public bool ShouldBuildExcel { get; set; }
        public bool HasTrendMarkers { get; set; }
    }
}
