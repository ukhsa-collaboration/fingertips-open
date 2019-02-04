using System;
using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.DataAccess.Repositories
{
    public class MonthlyReleaseRepository : RepositoryBase
    {
        public MonthlyReleaseRepository() : this(NHibernateSessionFactory.GetSession())
        {
        }

        public MonthlyReleaseRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        /// <summary>
        /// Get all release dates in ascending order of date
        /// </summary>
        public IList<MonthlyRelease> GetReleaseDates()
        {
            return CurrentSession.CreateCriteria<MonthlyRelease>()
                .SetCacheable(true)
                .AddOrder(Order.Asc("ReleaseDate"))
                .List<MonthlyRelease>();
        }

        /// <summary>
        /// Get past release dates in descending order of date
        /// </summary>
        public IList<MonthlyRelease> GetPastReleaseDates()
        {
            return CurrentSession.QueryOver<MonthlyRelease>()
                .Where(x => x.ReleaseDate < DateTime.Now.Date.AddHours(1))
                .OrderBy(x => x.ReleaseDate).Desc()
                .Cacheable()
                .List<MonthlyRelease>();
        }

    }
}
