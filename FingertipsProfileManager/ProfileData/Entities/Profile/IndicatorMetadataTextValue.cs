namespace Fpm.ProfileData.Entities.Profile
{
    public class IndicatorMetadataTextValue
    {
        public int Id { get; set; }

        public int IndicatorId { get; set; }
        public int? ProfileId { get; set; }
        public string Name { get; set; }
        public string NameLong { get; set; }
        public string Definition { get; set; }
        public string Rationale { get; set; }
        public string Policy { get; set; }
        public string DataSource { get; set; }
        public string Producer { get; set; }
        public string IndSourceLink { get; set; }
        public string IndMethod { get; set; }
        public string StandardPop { get; set; }
        public string CIMethod { get; set; }
        public string CountSource { get; set; }
        public string CountDefinition { get; set; }
        public string DenomSource { get; set; }
        public string DenomDefinition { get; set; }
        public string DiscControl { get; set; }
        public string Caveats { get; set; }
        public string Copyright { get; set; }
        public string Reuse { get; set; }
        public string Links { get; set; }
        public string RefNum { get; set; }
        public string Notes { get; set; }
        public string Frequency { get; set; }
        public string Rounding { get; set; }
        public string DataQuality { get; set; }
        public string IndicatorContent { get; set; }
        public string SpecificRationale { get; set; }
    }
}