using System;

namespace Fpm.ProfileData.Entities.Profile
{
    public class IndicatorAudit
    {
        public int Id { get; set; }
        public int IndicatorId { get; set; }
        public int GroupId { get; set; }
        public DateTime Timestamp { get; set; }
        public string User { get; set; }
        public string ReasonForChange { get; set; }
        public string AuditType { get; set; }
    }
}
