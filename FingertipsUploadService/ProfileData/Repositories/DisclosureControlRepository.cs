using FingertipsUploadService.ProfileData.Entities.Profile;
using NHibernate;
using System.Collections.Generic;
using NHibernate.Criterion;

namespace FingertipsUploadService.ProfileData.Repositories
{
    public interface IDisclosureControlRepository
    {
        DisclosureControl GetDisclosureControlById(int disclosureControlId);
    }

    public class DisclosureControlRepository : RepositoryBase, IDisclosureControlRepository
    {
        public DisclosureControlRepository() : this(NHibernateSessionFactory.GetSession())
        {
        }

        public DisclosureControlRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public virtual DisclosureControl GetDisclosureControlById(int disclosureControlId)
        {
            return CurrentSession.CreateCriteria<DisclosureControl>()
                .Add(Restrictions.Eq("Id", disclosureControlId))
                .UniqueResult<DisclosureControl>();
        }

    }
}
