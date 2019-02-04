
using Fpm.ProfileData.Entities.Job;
using Fpm.ProfileData.Entities.JobError;
using System;
using System.Collections.Generic;

namespace Fpm.ProfileData.Repositories
{
    public interface IFpmUploadRepository
    {
        void CreateJob(UploadJob job);
        IEnumerable<UploadJob> GetJobsForCurrentUser(int userId, int numberOfRecords);
        IEnumerable<UploadJob> GetAllJobsProgress();
        UploadJob GetJob(Guid jobGuid);
        IList<UploadJobError> FindJobErrorsByJobGuid(Guid jobGuid);
        bool UpdateJob(UploadJob job);
    }
}
