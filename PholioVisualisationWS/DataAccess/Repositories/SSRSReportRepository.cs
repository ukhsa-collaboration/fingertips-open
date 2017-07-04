using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.DataAccess.Repositories.Fpm.ProfileData.Repositories;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace PholioVisualisation.DataAccess.Repositories
{
    public class SSRSReportRepository : RepositoryBase
    {

        public SSRSReportRepository() : this(NHibernateSessionFactory.GetSession())
        {
        }

        public SSRSReportRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public IEnumerable<SSRSReportsProfileMapping> GetMappingByProfileId(int profileId)
        {
            return CurrentSession
              .CreateCriteria<SSRSReportsProfileMapping>()
              .Add(Restrictions.Eq("ProfileId", profileId))
              .List<SSRSReportsProfileMapping>();

        }

        public SSRSReport GetReportById(int reportId)
        {
            return CurrentSession
                .CreateCriteria<SSRSReport>()
                .Add(Restrictions.Eq("Id", reportId))
                .UniqueResult<SSRSReport>();
        }
    }
}
