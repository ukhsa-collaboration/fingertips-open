using System;

namespace Fpm.ProfileData.Entities.Profile
{
    public class IndicatorMetadataReviewAudit
    {
        public int Id { get; set; }
        public int IndicatorId { get; set; }
        public int AuditTypeId { get; set; }
        public string Notes { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
    }
}
