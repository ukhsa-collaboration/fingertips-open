using System;

namespace FingertipsUploadService.ProfileData.Entities.Job
{
    public class UploadJob
    {
        public UploadJob()
        {
            // Only is one type of job now
            JobType = UploadJobType.Batch;
        }

        public int Id { get; set; }
        public Guid Guid { get; set; }
        public UploadJobStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int TotalRows { get; set; }
        public UploadJobType JobType { get; set; }
        public string Filename { get; set; }
        public ProgressStage ProgressStage { get; set; }
        public int TotalRowsCommitted { get; set; }
        public int TotalSmallNumberWarnings { get; set; }
        public bool IsCellValidationPerRowDone { get; set; }
        public bool IsSmallNumberOverrideApplied { get; set; }
        public bool IsDuplicateOverrideApplied { get; set; }
    }
}