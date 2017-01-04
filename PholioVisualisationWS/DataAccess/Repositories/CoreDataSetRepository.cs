using NHibernate;
using PholioVisualisation.DataAccess.Repositories.Fpm.ProfileData.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess.Repositories
{
    public interface ICoreDataSetRepository
    {
        void Save(CoreDataSet coreDataSet);
    }

    public class CoreDataSetRepository : RepositoryBase, ICoreDataSetRepository
    {
        // poor man injection, should be removed when we use DI containers
        public CoreDataSetRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public CoreDataSetRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public void Save(CoreDataSet coreDataSet)
        {
            SaveNewObject(coreDataSet);
        }

        public void Delete(CoreDataSet coreDataSet)
        {
            DeleteObject(coreDataSet);
        }
    }
}
