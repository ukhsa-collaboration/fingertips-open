using System;
using Fpm.ProfileData.Entities.Logging;
using NHibernate;

namespace Fpm.ProfileData.Repositories
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

        public DatabaseLog GetDatabaseLog(int databaseLogId)
        {
           return CurrentSession.QueryOver<DatabaseLog>()
                .Where(x => x.Id == databaseLogId)
                .SingleOrDefault();
        }
    }
}
