using System;
using System.Collections.Generic;
using Fpm.ProfileData.Entities.Job;
using Fpm.ProfileData.Entities.JobError;

namespace Fpm.ProfileData.Repositories
{
    public interface IUploadJobRepository
    {
        void CreateJob(UploadJob job);
        IEnumerable<UploadJob> GetJobsForCurrentUser(int userId, int numberOfRecords);
        IEnumerable<UploadJob> GetNotStartedOrConfirmationGivenUploadJobs();
        UploadJob GetJob(Guid guid);
        IList<UploadJobError> FindJobErrorsByJobGuid(Guid jobGuid);
        bool UpdateJob(UploadJob job);
        IEnumerable<UploadJob> GetAllJobsProgress();
        void DeleteAllJobs();
    }
}