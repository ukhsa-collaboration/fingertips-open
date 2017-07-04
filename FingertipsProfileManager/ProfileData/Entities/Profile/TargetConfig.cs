using System.ComponentModel.DataAnnotations;

namespace Fpm.ProfileData.Entities.Profile
{
    public class TargetConfig
    {
        public int Id { get; set; }

        [Display(Name = "Lower limit")]
        public double? LowerLimit { get; set; }

        [Display(Name = "Upper limit")]
        public double? UpperLimit { get; set; }

        public string BespokeTargetKey { get; set; }

        [Display(Name = "Legend HTML (optional)")]
        [StringLength(300)]
        public string LegendHtml { get; set; }

        [Display(Name = "Polarity")]
        public int PolarityId { get; set; }

        [Required]
        [Display(Name = "Description")]
        [StringLength(150)]
        public string Description { get; set; }

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