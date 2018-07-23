namespace IndicatorsUI.DomainObjects
{
    public class LongerLivesProfileDetails
    {
        public int ProfileId { get; set; }
        public int SupportingProfileId { get; set; }
        public int DomainsToDisplay { get; set; }
        public bool HasPracticeData { get; set; }
        public string ExtraJsFiles { get; set; }
        public string Title { get; set; }
        public ProfileDetails SupportingProfileDetails { get; set; }
        public bool ShowCallOutBoxPopulation { get; set; }
        public bool ShowRankingInfoBox1 { get; set; }
        public bool ShowRankingInfoBox2 { get; set; }
        public bool ShowQuintilesLegend { get; set; }
    }
}