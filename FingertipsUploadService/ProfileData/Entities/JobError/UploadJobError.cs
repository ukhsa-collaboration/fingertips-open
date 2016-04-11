
using System;

namespace FingertipsUploadService.ProfileData.Entities.JobError
{
    public class UploadJobError
    {
        public int Id { get; set; }
        public Guid JobGuid { get; set; }
        public UploadJobErrorType ErrorType { get; set; }
        public string ErrorText { get; set; }
        public string ErrorJson { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
