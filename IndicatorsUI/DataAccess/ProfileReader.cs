using NHibernate;
using NHibernate.Criterion;
using IndicatorsUI.DomainObjects;
using System.Collections.Generic;
using System.Linq;

namespace IndicatorsUI.DataAccess
{
    public class ProfileReader : BaseReader
    {
        public bool IsContentCachedInMemory { get; set; }

        internal ProfileReader(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public ProfileDetails GetProfileDetails(string urlKey)
        {
            var profileDetails = CurrentSession.CreateCriteria<ProfileDetails>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("ProfileUrlKey", urlKey))
                .UniqueResult<ProfileDetails>();

            return profileDetails;
        }

        public ProfileDetails GetProfileDetails(int profileId)
        {
            var profileDetails = CurrentSession.CreateCriteria<ProfileDetails>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("Id", profileId))
                .UniqueResult<ProfileDetails>();

            return profileDetails;
        }

        public IList<Domain> GetProfileDomains(IEnumerable<int> groupIds)
        {
            var domains = CurrentSession.CreateCriteria<Domain>()
                .SetCacheable(true)
                .Add(Restrictions.In("GroupId", groupIds.ToList()))
                .AddOrder(Order.Asc("Number"))
                .List<Domain>();

            return domains;
        }

        public IList<int> GetDomainIds(int profileId)
        {
            return CurrentSession.CreateQuery("select  g.id from Domain g where g.ProfileId = :profileId")
                .SetCacheable(true)
                .SetParameter("profileId", profileId)
                .List<int>();
        }

        public Skin GetSkin(string Environment, string host)
        {
            var skin = CurrentSession.CreateCriteria<Skin>()
                .SetCacheable(true)
                .Add(Restrictions.Eq(Environment + "Host", host))
                .UniqueResult<Skin>();
            return skin;
        }

        public Skin GetSkinFromName(string skinName)
        {
            var skin = CurrentSession.CreateCriteria<Skin>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("Name", skinName))
                .UniqueResult<Skin>();
            return skin;
        }

        public Skin GetSkinFromId(int skinId)
        {
            var skin = CurrentSession.CreateCriteria<Skin>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("Id", skinId))
                .UniqueResult<Skin>();
            return skin;
        }

        public ContentItem GetContentItem(string contentKey, int profileId)
        {
            if (IsContentCachedInMemory == false)
            {
                CurrentSession.Clear();
            }

            var contentItem = CurrentSession.CreateCriteria<ContentItem>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("ContentKey", contentKey))
                .Add(Restrictions.Eq("ProfileId", profileId))
                .UniqueResult<ContentItem>();
            return contentItem;
        }

        public IList<ProfileCollectionItem> GetProfileCollectionItems(int profilecollectionid)
        {
            return CurrentSession.CreateCriteria<ProfileCollectionItem>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("ProfileCollectionId", profilecollectionid))
                .AddOrder(new Order("Sequence", true))
                .List<ProfileCollectionItem>();
        }

        public ProfileCollection GetProfileCollection(int collectionId)
        {
            return CurrentSession.CreateQuery("from ProfileCollection pc where pc.Id = :collectionId")
                .SetCacheable(true)
                .SetParameter("collectionId", collectionId)
                .UniqueResult<ProfileCollection>();
        }

        public IList<SkinProfileCollection> GetSkinProfileCollections(int skinId)
        {
            return CurrentSession.CreateQuery("from SkinProfileCollection spc where spc.SkinId = :skinId")
                .SetCacheable(true)
                .SetParameter("skinId", skinId)
                .List<SkinProfileCollection>();
        }
    }
}
