using System.Web.Mvc;

namespace Fpm.ProfileData.Entities.Profile
{
    public class IndicatorMetadataTextValue
    {
        public int Id { get; set; }

        public int IndicatorId { get; set; }
        public int? ProfileId { get; set; }
        public string Name { get; set; }
        [AllowHtml]
        public string Definition { get; set; }
        [AllowHtml]
        public string Rationale { get; set; }
        [AllowHtml]
        public string DataSource { get; set; }
        [AllowHtml]
        public string Producer { get; set; }
        [AllowHtml]
        public string IndSourceLink { get; set; }
        [AllowHtml]
        public string IndMethod { get; set; }
        public string StandardPop { get; set; }
        [AllowHtml]
        public string CIMethod { get; set; }
        [AllowHtml]
        public string CountSource { get; set; }
        [AllowHtml]
        public string CountDefinition { get; set; }
        [AllowHtml]
        public string DenomSource { get; set; }
        [AllowHtml]
        public string DenomDefinition { get; set; }
        [AllowHtml]
        public string DiscControl { get; set; }
        [AllowHtml]
        public string Caveats { get; set; }
        [AllowHtml]
        public string Copyright { get; set; }
        [AllowHtml]
        public string Reuse { get; set; }
        [AllowHtml]
        public string Links { get; set; }
        public string RefNum { get; set; }
        [AllowHtml]
        public string Notes { get; set; }
        [AllowHtml]
        public string Frequency { get; set; }
        public string Rounding { get; set; }
        public string DataQuality { get; set; }
        public string IndicatorContent { get; set; }
        [AllowHtml]
        public string SpecificRationale { get; set; }
        public string Keywords { get; set; }
        public string EvidOfVariability { get; set; }
        public string JustifConfIntMeth { get; set; }
        public string QualityAssurance { get; set; }
        public string QualityImprPlan { get; set; }
        public string JustiOfExclusions { get; set; }
        public string JustifOfDataSources { get; set; }
        public string SponsorStakeholders { get; set; }
        public string IndOwnerContDet { get; set; }
        public string Comments { get; set; }
    }
}