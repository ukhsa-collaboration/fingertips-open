using System.Collections.Generic;
using IndicatorsUI.DomainObjects;
using NHibernate;

namespace IndicatorsUI.DataAccess.Repository
{
    public interface IRelativeUrlRedirectRepository
    {

    }

    public class RelativeUrlRedirectRepository : RepositoryBase, IRelativeUrlRedirectRepository
    {
        public RelativeUrlRedirectRepository() : this(NHibernateSessionFactory.GetSession()) { }

        public RelativeUrlRedirectRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

        public IList<RelativeUrlRedirect> GetAllRelativeUrlRedirects()
        {
            return CurrentSession.QueryOver<RelativeUrlRedirect>()
                .List();
        } 
    }
}