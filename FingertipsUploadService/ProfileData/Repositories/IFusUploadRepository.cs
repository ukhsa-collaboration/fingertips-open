using FingertipsUploadService.ProfileData.Entities.Job;
using System.Collections.Generic;

namespace FingertipsUploadService.ProfileData.Repositories
{
    public interface IFusUploadRepository
    {
        IEnumerable<UploadJob> GetNotStartedOrConfirmationGivenUploadJobs();
        void UpdateJob(UploadJob uploadJob);
    }
}
