using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.DataAccess.Repositories.Fpm.ProfileData.Repositories;
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

        public IList<MonthlyRelease> GetReleaseDates()
        {
            return CurrentSession.CreateCriteria<MonthlyRelease>()
                .AddOrder(Order.Asc("ReleaseDate"))
                .List<MonthlyRelease>();
        }

    }
}
