using NHibernate;
using NHibernate.Transform;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataAccess.Repositories
{
    public interface ICoreDataAuditRepository
    {
        IList<CoreDataDeleteAudit> GetLatestDeleteAuditData();
        CoreDataDeleteAudit GetLatestDeleteAuditData(int indicatorId);
        CoreDataUploadAudit GetLatestUploadAuditData(int indicatorId);
    }

    public class CoreDataAuditRepository : RepositoryBase, ICoreDataAuditRepository
    {
        public CoreDataAuditRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public CoreDataAuditRepository() : this(NHibernateSessionFactory.GetSession())
        {
        }

        public IList<CoreDataDeleteAudit> GetLatestDeleteAuditData()
        {
            return CurrentSession.QueryOver<CoreDataDeleteAudit>()
                .OrderBy(x => x.Id).Desc
                .Take(1).List();
        }

        public CoreDataDeleteAudit GetLatestDeleteAuditData(int indicatorId)
        {
            return CurrentSession.QueryOver<CoreDataDeleteAudit>()
                .Where(x => x.IndicatorId == indicatorId)
                .OrderBy(x => x.DateDeleted).Desc
                .Take(1)
                .SingleOrDefault();
        }

        public CoreDataUploadAudit GetLatestUploadAuditData(int indicatorId)
        {
            var q = CurrentSession.GetNamedQuery("GetLastSuccessfulUploadAuditForAnIndicator");
            q.SetParameter("indicatorId", indicatorId);
            q.SetResultTransformer(Transformers.AliasToBean<CoreDataUploadAudit>());
            return q.List<CoreDataUploadAudit>().FirstOrDefault();
        }
    }
}