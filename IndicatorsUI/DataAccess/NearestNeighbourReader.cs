using NHibernate;
using NHibernate.Criterion;
using Profiles.DomainObjects;
using System.Collections.Generic;

namespace Profiles.DataAccess
{
    public class NearestNeighbourReader : BaseReader
    {
        internal NearestNeighbourReader(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public IList<ProfileNearestNeighbourAreaTypeMapping> GetProfileNearestNeighbourAreaTypeMapping(int profileId)
        {
            var profileIds = new List<int> { -1, profileId };
            return CurrentSession.CreateCriteria<ProfileNearestNeighbourAreaTypeMapping>()
                .Add(Restrictions.In("ProfileId", profileIds))
                .List<ProfileNearestNeighbourAreaTypeMapping>();
        }

        public IList<NeighbourType> GetNearestNeighbourAreaType(List<int> neighbourId)
        {
            return CurrentSession.CreateCriteria<NeighbourType>()
                .Add(Restrictions.In("NeighbourTypeId", neighbourId))
                .List<NeighbourType>();
        }
    }
}
