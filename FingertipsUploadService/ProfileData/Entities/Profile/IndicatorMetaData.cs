using System;
using System.ComponentModel.DataAnnotations;

namespace FingertipsUploadService.ProfileData.Entities.Profile
{
    public class IndicatorMetadata
    {
        public int IndicatorId { get; set; }
        public int OwnerProfileId { get; set; }
        public int DisclosureControlId { get; set; }
    }
}
