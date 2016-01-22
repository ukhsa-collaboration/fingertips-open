using System;

namespace FingertipsUploadService.ProfileData.Entities.User
{
    public class UserAudit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public bool IsAdministrator { get; set; }
        public DateTime Timestamp { get; set; }
        public string User { get; set; }
        public string AuditType { get; set; }
    }
}
