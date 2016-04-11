
using Fpm.ProfileData.Entities.Job;
using Fpm.ProfileData.Entities.JobError;

namespace Fpm.MainUI.ViewModels.Upload
{
    public class UploadSummaryViewModel
    {
        public UploadJobStatus JobStatus { get; set; }
        public UploadJobErrorType ErrorType { get; set; }
        public string ErrorText { get; set; }
        public string ErrorJson { get; set; }
    }
}