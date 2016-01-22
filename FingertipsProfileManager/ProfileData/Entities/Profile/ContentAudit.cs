using System;

namespace Fpm.ProfileData.Entities.Profile
{
    public class ContentAudit
    {
        public int Id { get; set; }
        public int ContentId { get; set; }
        public string ContentKey { get; set; }
        public string FromContent { get; set; }
        public string ToContent { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserName { get; set; }
    }
}
