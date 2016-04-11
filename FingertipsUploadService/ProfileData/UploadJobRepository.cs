using FingertipsUploadService.ProfileData.Entities.Job;
using FingertipsUploadService.ProfileData.Repositories;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;

namespace FingertipsUploadService.ProfileData
{
    public class UploadJobRepository : RepositoryBase, IFusUploadRepository
    {
        public UploadJobRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public UploadJobRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public IEnumerable<UploadJob> GetNotStartedOrConfirmationGivenUploadJobs()
        {
            // Must clear the session otherwise old object will be returned.
            CurrentSession.Clear();

            return CurrentSession
                .CreateCriteria<UploadJob>()
                .Add(Restrictions.Or(
                    Restrictions.Eq("Status", UploadJobStatus.NotStart),
                    Restrictions.Eq("Status", UploadJobStatus.ConfirmationGiven)
                ))
                .SetCacheMode(CacheMode.Refresh)
                .SetCacheRegion("")
                .List<UploadJob>();
        }

        public UploadJob FindUploadJobByGuid(Guid guid)
        {
            return CurrentSession
                .CreateCriteria<UploadJob>()
                .Add(Restrictions.Eq("Guid", guid))
                .UniqueResult<UploadJob>();

        }

        public void UpdateJob(UploadJob uploadJob)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Update(uploadJob);
                CurrentSession.Flush();

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public UploadJob SaveJob(UploadJob job)
        {
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Save(job);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }

            return job;
        }

        public void DeleteJob(Guid guid)
        {
            const string query = "delete from uploadjob  where guid = :guid";

            CurrentSession
                .CreateSQLQuery(query)
                .SetParameter("guid", guid)
                .ExecuteUpdate();
        }
    }
}
