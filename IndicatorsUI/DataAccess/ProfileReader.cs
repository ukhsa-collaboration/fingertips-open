using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using Profiles.DomainObjects;

namespace Profiles.DataAccess
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
                .Add(Restrictions.Eq("ProfileUrlKey", urlKey))
                .UniqueResult<ProfileDetails>();

            return profileDetails;
        }

        public ProfileDetails GetProfileDetails(int profileId)
        {
            var profileDetails = CurrentSession.CreateCriteria<ProfileDetails>()
                .Add(Restrictions.Eq("Id", profileId))
                .UniqueResult<ProfileDetails>();

            return profileDetails;
        }

        public IList<Domain> GetProfileDomains(IEnumerable<int> groupIds)
        {
            var domains = CurrentSession.CreateCriteria<Domain>()
                .Add(Restrictions.In("GroupId", groupIds.ToList()))
                .AddOrder(Order.Asc("Number"))
               .List<Domain>();

            return domains;
        }

        public IList<int> GetDomainIds(int profileId)
        {
            IQuery q = CurrentSession.CreateQuery("select  g.id from Domain g where g.ProfileId = :profileId");
            q.SetParameter("profileId", profileId);
            return q.List<int>();
        }

        public Skin GetSkin(string Environment, string host)
        {
            var skin = CurrentSession.CreateCriteria<Skin>()
                .Add(Restrictions.Eq(Environment + "Host", host))
                .UniqueResult<Skin>();
            return skin;
        }

        public Skin GetSkinFromName(string skinName)
        {
            var skin = CurrentSession.CreateCriteria<Skin>()
                .Add(Restrictions.Eq("Name", skinName))
                .UniqueResult<Skin>();
            return skin;
        }

        public Skin GetSkinFromId(int skinId)
        {
            var skin = CurrentSession.CreateCriteria<Skin>()
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
                .Add(Restrictions.Eq("ContentKey", contentKey))
                .Add(Restrictions.Eq("ProfileId", profileId))
                .UniqueResult<ContentItem>();
            return contentItem;
        }

        public IList<ProfileCollectionItem> GetProfileCollectionItems(int profilecollectionid)
        {
            return CurrentSession.CreateCriteria<ProfileCollectionItem>()
                .Add(Restrictions.Eq("ProfileCollectionId", profilecollectionid))
                .AddOrder(new Order("Sequence",true))
                .List<ProfileCollectionItem>();
        }

        public ProfileCollection GetProfileCollection(int collectionId)
        {
            IQuery q = CurrentSession.CreateQuery("from ProfileCollection pc where pc.Id = :collectionId");
            q.SetParameter("collectionId", collectionId);
            return q.UniqueResult<ProfileCollection>();
        }

        public IList<SkinProfileCollection> GetSkinProfileCollections(int skinId)
        {
            IQuery q = CurrentSession.CreateQuery("from SkinProfileCollection spc where spc.SkinId = :skinId");
            q.SetParameter("skinId", skinId);
            return q.List<SkinProfileCollection>();
        }
    }
}
