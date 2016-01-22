
namespace FingertipsUploadService.Entities.Job
{
    public enum UploadJobStatus
    {
        NotStart,
        InProgress,
        Cancelled,
        FailedValidation,
        SuccessfulUpload
    }
}
