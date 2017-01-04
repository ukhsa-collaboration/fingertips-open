using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using PholioVisualisation.DataAccess.Repositories.Fpm.ProfileData.Repositories;

namespace PholioVisualisation.DataAccess.Repositories
{
    public interface IIndicatorMetadataRepository
    {
        IList<int> GetIndicatorsForWhichDataHasChangedInPreviousDays(int days);
    }

    public class IndicatorMetadataRepository : RepositoryBase, IIndicatorMetadataRepository
    {
        // poor man injection, should be removed when we use DI containers
        public IndicatorMetadataRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public IndicatorMetadataRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public IList<int> GetIndicatorsForWhichDataHasChangedInPreviousDays(int days)
        {
            return CurrentSession.GetNamedQuery("SelectIndicatorsChangedWithPreviousDays")
                .SetParameter("days", days)
                .List<int>();
        }

    }
}
