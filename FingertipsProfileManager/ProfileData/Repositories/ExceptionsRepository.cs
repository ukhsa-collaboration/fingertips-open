using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData.Entities.Exceptions;
using NHibernate;

namespace Fpm.ProfileData.Repositories
{
    public class ExceptionsRepository : RepositoryBase
    {
        /// <summary>
        /// APIs may generate lots of exceptions which would prevent the exception
        /// list being displayed unless limited.
        /// </summary>
        public const int MaxExceptionCount = 500;

        // poor man injection, should be removed when we use DI containers
        public ExceptionsRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public ExceptionsRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public IList<ExceptionLog> GetExceptionsByServer(int exceptionDays, string exceptionServer)
        {
            DateTime initDate = DateTime.Today.AddDays(exceptionDays * -1);

            IQuery q = CurrentSession.CreateQuery(
                "from ExceptionLog e where e.Date > :initDate and e.Server = :exceptionServer order by e.Date desc");
            q.SetParameter("initDate", initDate);
            q.SetParameter("exceptionServer", exceptionServer);
            q.SetMaxResults(MaxExceptionCount);
            return q.List<ExceptionLog>();
        }

        public IList<ExceptionLog> GetExceptionsForAllServers(int exceptionDays)
        {
            DateTime initDate = DateTime.Today.AddDays(exceptionDays * -1);

            IQuery q = CurrentSession.CreateQuery(
                "from ExceptionLog el where el.Date > :initDate order by el.Id desc");
            q.SetParameter("initDate", initDate);
            q.SetMaxResults(MaxExceptionCount);
            return q.List<ExceptionLog>();
        }

        public ExceptionLog GetException(int exceptionId)
        {
            IQuery q = CurrentSession.CreateQuery("from ExceptionLog el where el.Id = :exceptionId");
            q.SetParameter("exceptionId", exceptionId);
            var exception = q.UniqueResult<ExceptionLog>();
            return exception;
        }

        public IEnumerable<string> GetDistinctExceptionServers()
        {
            IQuery q = CurrentSession.CreateQuery("select distinct el.Server from ExceptionLog el order by el.Server");
            return q.List<string>();
        }

    }
}
