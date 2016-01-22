using FingertipsUploadService.ProfileData.Entities.Logging;
using System;

namespace FingertipsUploadService.Entities.Job
{
    public class UploadJob
    {
        public Guid Id { get; set; }
        public UploadJobStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public int RowsUploadedCount { get; set; }
        public UploadJobType JobType { get; set; }
        public UploadAudit UploadAudit { get; set; }
        public string Filename { get; set; }
        public int FileSize { get; set; }
        public string UploadedBy { get; set; }
    }
}