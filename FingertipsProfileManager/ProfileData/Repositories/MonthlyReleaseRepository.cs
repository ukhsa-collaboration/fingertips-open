using System;
using System.Collections;
using System.Collections.Generic;
using Fpm.ProfileData.Entities.MonthlyRelease;
using NHibernate;

namespace Fpm.ProfileData.Repositories
{
    public interface IMonthlyReleaseRepository
    {
        IList<MonthlyRelease> GetUpcomingMonthlyReleases(int count);
    }

    public class MonthlyReleaseRepository : RepositoryBase, IMonthlyReleaseRepository
    {
        // poor man injection, should be removed when we use DI containers
        public MonthlyReleaseRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public MonthlyReleaseRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public IList<MonthlyRelease> GetUpcomingMonthlyReleases(int count)
        {
            return CurrentSession.QueryOver<MonthlyRelease>()
                .Where(x => x.ReleaseDate > DateTime.Now)
                .Take(count)
                .Cacheable()
                .List();
        }
    }
}