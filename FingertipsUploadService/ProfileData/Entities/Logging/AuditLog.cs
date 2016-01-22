using System;

namespace FingertipsUploadService.ProfileData.Entities.Logging
{
    public class AuditLog
    {
        public int Id { get; set; }
        
        public int IndicatorId { get; set; }

        public int GroupId { get; set; }

        public string ReasonForChange { get; set; }

        public string AuditType { get; set; }

        public DateTime Timestamp { get; set; }

        public string UserName { get; set; }

    }
}
