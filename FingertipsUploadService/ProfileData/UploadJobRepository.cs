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
            //            CurrentSession.Clear();

            var statusCodes = new List<int> { 0, 301, 311 };
            return CurrentSession.QueryOver<UploadJob>()
                .AndRestrictionOn(x => x.Status).
                IsIn(statusCodes).List();

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
                CurrentSession.Refresh(uploadJob);
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

                CurrentSession.Flush();
                CurrentSession.Refresh(job);
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

        // Only used to clean the test environment 
        public void DeleteAllJob()
        {
            const string query = "delete from uploadjob";
            CurrentSession
                .CreateSQLQuery(query)
                .ExecuteUpdate();
        }

        public void CreateNewJob(UploadJob job)
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

    }
}
