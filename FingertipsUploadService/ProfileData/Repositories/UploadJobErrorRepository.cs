
using FingertipsUploadService.ProfileData.Entities.JobError;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;

namespace FingertipsUploadService.ProfileData.Repositories
{
    public class UploadJobErrorRepository : RepositoryBase
    {
        public UploadJobErrorRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public UploadJobErrorRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public int Log(UploadJobError error)
        {
            error.CreatedAt = DateTime.Now;
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession.Save(error);

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }

            return error.Id;
        }

        public void DeleteLog(Guid jobGuid)
        {
            const string query = "delete from uploadjoberror where jobguid = :jobguid";
            try
            {
                transaction = CurrentSession.BeginTransaction();

                CurrentSession
                    .CreateSQLQuery(query)
                    .SetParameter("jobguid", jobGuid)
                    .ExecuteUpdate();

                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public IList<UploadJobError> FindJobErrorsByJobGuid(Guid jobGuid)
        {
            return CurrentSession
                .CreateCriteria<UploadJobError>()
                .Add(Restrictions.Eq("JobGuid", jobGuid))
                .List<UploadJobError>();
        }
    }
}
