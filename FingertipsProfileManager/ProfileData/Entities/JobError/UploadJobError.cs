
using System;

namespace Fpm.ProfileData.Entities.JobError
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

    public enum UploadJobErrorType
    {
        DuplicateRowInSpreadsheetError = 40003,
        DuplicateRowInDatabaseError = 40004
    }
}
