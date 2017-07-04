using FingertipsUploadService.ProfileData.Entities.LookUps;
using NHibernate;
using NHibernate.Transform;
using System.Collections.Generic;

namespace FingertipsUploadService.ProfileData.Repositories
{
    public class AreaTypeRepository : RepositoryBase
    {
        public AreaTypeRepository() : this(NHibernateSessionFactory.GetSession()) { }

        public AreaTypeRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public virtual bool ShouldWarnAboutSmallNumbersForArea(string areaCode)
        {
            IQuery q = CurrentSession.GetNamedQuery("ShouldWarnAboutSmallNumbersForArea");
            q.SetParameter("areaCode", areaCode);
            return q.UniqueResult<bool>();
        }
    }
}
