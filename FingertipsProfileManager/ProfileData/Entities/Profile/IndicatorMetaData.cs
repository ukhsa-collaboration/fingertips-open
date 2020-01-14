using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fpm.ProfileData.Entities.Profile
{
    public class IndicatorMetadata
    {
        public int IndicatorId { get; set; }
        public int DenominatorTypeId { get; set; }
        public int CIMethodId { get; set; }

        [Display(Name = "ValueTypeId")]
        public int ValueTypeId { get; set; }

        public int UnitId { get; set; }
        public int YearTypeId { get; set; }
        public int OwnerProfileId { get; set; }
        public int? DecimalPlacesDisplayed { get; set; }
        public int? TargetId { get; set; }
        public DateTime DateEntered { get; set; }
        public int DisclosureControlId { get; set; }
        public DateTime? LatestChangeTimestampOverride { get; set; }
        public string PartitionAgeIds { get; set; }
        public string PartitionSexIds { get; set; }
        public string PartitionAreaTypeIds { get; set; }
        public bool AlwaysShowSexWithIndicatorName { get; set; }
        public bool AlwaysShowAgeWithIndicatorName { get; set; }
        public bool AlwaysShowSpineChart { get; set; }
        public bool ShouldAveragesBeCalculated { get; set; }
        public DateTime? NextReviewTimestamp { get; set; }
        public string Status { get; set; }
        public int DestinationProfileId { get; set; }

        /// <summary>
        /// View model property
        /// </summary>
        public DateTime? NextReviewTimestampInitialValue { get; set; }
    }
}
