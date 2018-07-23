using FingertipsUploadService.ProfileData.Entities.Logging;
using NHibernate;
using System;
using System.Linq;

namespace FingertipsUploadService.ProfileData.Repositories
{
    public class LoggingRepository : RepositoryBase
    {

        // poor man injection, should be removed when we use DI containers
        public LoggingRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public LoggingRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        /// <summary>
        /// Updates the database log to record when FUS last checked for jobs. Record to help identify when FUS is unavailable
        /// </summary>
        public void UpdateFusCheckedJobs()
        {
            var log =  CurrentSession.QueryOver<DatabaseLog>()
                .Where(x => x.Id == DatabaseLogIds.FusCheckedJobs)
                .SingleOrDefault();

            log.Timestamp = DateTime.Now;

            try
            {
                transaction = CurrentSession.BeginTransaction();
                CurrentSession.SaveOrUpdate(log);
                transaction.Commit();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }
    }
}
