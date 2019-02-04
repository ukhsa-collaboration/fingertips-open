using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess.Repositories
{
    public interface IDatabaseLogRepository
    {
        IList<DatabaseLog> GetDatabaseLogs();
        DateTime GetPholioBackUpTimestamp();
    }

    public class DatabaseLogRepository : RepositoryBase, IDatabaseLogRepository
    {
        public DatabaseLogRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public DatabaseLogRepository() : this(NHibernateSessionFactory.GetSession())
        {
        }

        public IList<DatabaseLog> GetDatabaseLogs()
        {
            return CurrentSession.QueryOver<DatabaseLog>()
                .List();
        }

        public DateTime GetPholioBackUpTimestamp()
        {
            return GetDatabaseLogs()
                .First(x => x.Event.ToLower().StartsWith("backed up"))
                .Timestamp;
        }
    }
}