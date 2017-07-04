﻿using System;
using System.ComponentModel.DataAnnotations;

namespace FingertipsUploadService.ProfileData.Entities.Profile
{
    public class IndicatorMetadata
    {
        public int IndicatorId { get; set; }
        public int DenominatorTypeId { get; set; }
        public int CIMethodId { get; set; }
        public double ConfidenceLevel { get; set; }

        [Display(Name = "ValueTypeId")]
        public int ValueTypeId { get; set; }

        public int UnitId { get; set; }
        public int YearTypeId { get; set; }

        public int OwnerProfileId { get; set; }
        public int? DecimalPlacesDisplayed { get; set; }
        public int? TargetId { get; set; }
        public DateTime DateEntered { get; set; }
        public int DisclosureControlId { get; set; }
    }
}