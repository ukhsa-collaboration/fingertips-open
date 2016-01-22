
using FingertipsUploadService.Entities.Job;
using System.Collections.Generic;

namespace FingertipsUploadService.Repositories
{
    public interface IFusUploadRepository
    {
        List<UploadJob> GetNotStartedUploadJobs();
        UploadJobStatus UpdateJob(UploadJob uploadJob);
        void SaveValidationFailure(UploadJobStatus status);
    }
}
