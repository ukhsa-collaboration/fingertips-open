using System.ComponentModel.DataAnnotations;
using Foolproof;

namespace Fpm.ProfileData.Entities.Profile
{
    public class TargetConfig
    {
        public int Id { get; set; }

        [Display(Name = "Lower limit")]
        [RequiredIfTrue("UseCIsForLimitComparison", ErrorMessage = "Lower limit must be set for CIs comparison")]
        public double? LowerLimit { get; set; }

        [Display(Name = "Upper limit")]
        public double? UpperLimit { get; set; }

        [Display(Name = "Bespoke target key")]
        public string BespokeTargetKey { get; set; }

        [Display(Name = "Legend HTML")]
        [StringLength(300)]
        public string LegendHtml { get; set; }

        [Display(Name = "Polarity")]
        public int PolarityId { get; set; }

        [Required]
        [Display(Name = "Description")]
        [StringLength(150)]
        public string Description { get; set; }

        /// <summary>
        /// Default would be to use the area values.
        /// </summary>
        [Display(Name = "Assess limit against area CIs")]
        public bool UseCIsForLimitComparison { get; set; }

        [Display(Name = "Bespoke target")]
        public bool IsBespokeTarget
        {
            get { return string.IsNullOrEmpty(BespokeTargetKey) == false; }
        }

        public string GetLowerLimitDisplayValue()
        {
            return GetLimitString(LowerLimit);
        }

        public string GetUpperLimitDisplayValue()
        {
            return GetLimitString(UpperLimit);
        }

        private static string GetLimitString(double? limit)
        {
            return limit.HasValue
                ? limit.Value.ToString()
                : "n/a";
        }
    }
}