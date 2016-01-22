
namespace FingertipsUploadService.Repositories
{
    interface IFpmUploadRepository
    {
        void SaveJob();
        void GetJobsForUser(int userId);
        void GetJob(int jobId);
        void GetValidationFailuresForJob(int jobId);
    }
}
