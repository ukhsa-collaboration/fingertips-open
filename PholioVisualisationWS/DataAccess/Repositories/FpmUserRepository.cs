using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess.Repositories
{
    public class FpmUserRepository : RepositoryBase
    {
        public FpmUserRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public FpmUserRepository() : this(NHibernateSessionFactory.GetSession())
        {
        }

        public FpmUser GetUserById(int id)
        {
            return CurrentSession
                .CreateCriteria<FpmUser>()
                .Add(Restrictions.Eq("Id", id))
                .UniqueResult<FpmUser>();
        }

        public FpmUser GetUserDisplayNameFromUsername(string username)
        {
            return CurrentSession
                .CreateCriteria<FpmUser>()
                .Add(Restrictions.Eq("UserName", username))
                .UniqueResult<FpmUser>();
        }
    }
}
