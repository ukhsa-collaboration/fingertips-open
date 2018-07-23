using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Transform;
using PholioVisualisation.DataAccess.Repositories.Fpm.ProfileData.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess.Repositories
{
    public interface IIndicatorMetadataRepository
    {
        IList<int> GetIndicatorsUploadedSinceDate(DateTime dateTime);
        IList<int> GetIndicatorsDeletedSinceDate(DateTime dateTime);
        IList<IndicatorMetadataSearchExpectation> GetIndicatorMetadataSearchExpectations(string searchTerm);
        IList<INamedEntity> GetIndicatorNames(IEnumerable<int> indicatorIds);
        bool DoesIndicatorIdExist(int indicatorId);
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

        public IList<int> GetIndicatorsUploadedSinceDate(DateTime dateTime)
        {
            return CurrentSession.GetNamedQuery("SelectIndicatorsUploadedSinceDate")
                .SetParameter("date", dateTime)
                .List<int>();
        }

        public IList<int> GetIndicatorsDeletedSinceDate(DateTime dateTime)
        {
            return CurrentSession.GetNamedQuery("SelectIndicatorsDeletedSinceDate")
                .SetParameter("date", dateTime)
                .List<int>();
        }

        public IList<INamedEntity> GetIndicatorNames(IEnumerable<int> indicatorIds)
        {
            if (indicatorIds.Any() == false) return new List<INamedEntity>();

            return CurrentSession.GetNamedQuery("SelectIndicatorNames")
                .SetParameterList("indicatorIds", indicatorIds)
                .SetResultTransformer(Transformers.AliasToBean<NamedEntity>())
                .List<INamedEntity>();
        }

        public IList<IndicatorMetadataSearchExpectation> GetIndicatorMetadataSearchExpectations(string searchTerm)
        {
            return CurrentSession
                .QueryOver<IndicatorMetadataSearchExpectation>()
                .Where(x => x.SearchTerm == searchTerm)
                .List();
        }

        public bool DoesIndicatorIdExist(int indicatorId)
        {
            return CurrentSession.QueryOver<IndicatorMetadata>()
                       .Where(x => x.IndicatorId == indicatorId) != null;
        }
    }
}
