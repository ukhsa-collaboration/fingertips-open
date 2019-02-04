
using Fpm.ProfileData.Entities.Job;
using Fpm.ProfileData.Entities.JobError;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;

namespace Fpm.ProfileData.Repositories
{
    public class UploadJobRepository : RepositoryBase, IFpmUploadRepository
    {
        public UploadJobRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public UploadJobRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public void CreateJob(UploadJob job)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Save(job);
                // Remove job from cache
                CurrentSession.Flush();
                CurrentSession.Refresh(job);
                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public IEnumerable<UploadJob> GetJobsForCurrentUser(int userId, int numberOfRecords)
        {
            return CurrentSession
                .CreateCriteria<UploadJob>()
                .Add(Restrictions.Eq("UserId", userId))
                .AddOrder(Order.Desc("DateCreated"))
                .SetMaxResults(numberOfRecords)
                .List<UploadJob>();
        }

        public IEnumerable<UploadJob> GetNotStartedOrConfirmationGivenUploadJobs()
        {
            // Must clear the session otherwise old object will be returned.
            //CurrentSession.Clear();

            return CurrentSession
                .CreateCriteria<UploadJob>()
                .Add(Restrictions.Or(
                    Restrictions.Eq("Status", UploadJobStatus.NotStarted),
                    Restrictions.Eq("Status", UploadJobStatus.OverrideDatabaseDuplicatesConfirmationGiven)
                ))
                .SetCacheMode(CacheMode.Refresh)
                .SetCacheRegion("")
                .List<UploadJob>();
        }

        public UploadJob GetJob(Guid guid)
        {
            return CurrentSession
                .CreateCriteria<UploadJob>()
                .Add(Restrictions.Eq("Guid", guid))
                .UniqueResult<UploadJob>();
        }

        public IList<UploadJobError> FindJobErrorsByJobGuid(Guid jobGuid)
        {
            return CurrentSession
                .CreateCriteria<UploadJobError>()
                .Add(Restrictions.Eq("JobGuid", jobGuid))
                .AddOrder(Order.Desc("CreatedAt"))
                .List<UploadJobError>();
        }

        public bool UpdateJob(UploadJob job)
        {
            var isOk = false;
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Update(job);
                CurrentSession.Flush();
                // Remove object from cache
                CurrentSession.Flush();
                CurrentSession.Refresh(job);

                transaction.Commit();
                isOk = true;
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
            return isOk;
        }

        public IEnumerable<UploadJob> GetAllJobsProgress()
        {
            var statuscode = new List<int>
            {
               (int) UploadJobStatus.NotStarted,
               (int) UploadJobStatus.InProgress,
               (int) UploadJobStatus.OverrideDatabaseDuplicatesConfirmationGiven,
               (int) UploadJobStatus.SmallNumberWarningConfirmationGiven
            };

            return CurrentSession.CreateCriteria<UploadJob>()
                .Add(Restrictions.In("Status", statuscode))
                .AddOrder(Order.Asc("DateCreated"))
                .List<UploadJob>();
        }

        public void DeleteAllJobs()
        {
            CurrentSession.CreateSQLQuery("delete from UploadJob").ExecuteUpdate();
        }

    }
}
