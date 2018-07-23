using System.Collections.Generic;
using NHibernate;
using PholioVisualisation.DataAccess.Repositories.Fpm.ProfileData.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess.Repositories
{
    public interface IAreaTypeComponentRepository
    {
        IList<AreaTypeComponent> GetAreaTypeComponents(int areaTypeId);
    }

    public class AreaTypeComponentRepository : RepositoryBase, IAreaTypeComponentRepository
    {
        public AreaTypeComponentRepository() : this(NHibernateSessionFactory.GetSession())
        {
        }

        public AreaTypeComponentRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public IList<AreaTypeComponent> GetAreaTypeComponents(int areaTypeId)
        {
            return CurrentSession.QueryOver<AreaTypeComponent>()
                .Where(x => x.AreaTypeId == areaTypeId)
                .Cacheable()
                .List();
        }
    }
}