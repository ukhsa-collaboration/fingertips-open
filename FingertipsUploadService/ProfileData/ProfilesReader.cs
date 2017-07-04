using FingertipsUploadService.ProfileData.Entities.Core;
using FingertipsUploadService.ProfileData.Entities.Profile;
using FingertipsUploadService.ProfileData.Entities.User;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Linq;

namespace FingertipsUploadService.ProfileData
{
    public class ProfilesReader : BaseReader
    {
        /// <summary>
        /// Parameterless constructor to enable mocking.
        /// </summary>
        public ProfilesReader() { }

        public ProfilesReader(ISessionFactory sessionFactory)
            : base(sessionFactory)
        { }

        public IList<ProfileDetails> GetProfiles()
        {
            IQuery q = CurrentSession.CreateQuery("from ProfileDetails p where p.Id != " + ProfileIds.Search);
            return q.List<ProfileDetails>();
        }

        public ProfileDetails GetOwnerProfilesByIndicatorIds(int indicatorId)
        {
            IQuery q = CurrentSession.CreateQuery(
                 "select p from ProfileDetails p, IndicatorMetadata i where p.Id = i.OwnerProfileId and i.IndicatorId = :indicatorId");
            q.SetParameter("indicatorId", indicatorId);
            return q.UniqueResult<ProfileDetails>();
        }

        public IList<UserGroupPermissions> GetUserGroupPermissionsByUserId(int userId)
        {
            IQuery q = CurrentSession.CreateQuery("from UserGroupPermissions up where up.UserId = :userId");
            q.SetParameter("userId", userId);
            return q.List<UserGroupPermissions>();
        }

        public virtual IndicatorMetadata GetIndicatorMetadata(int indicatorId)
        {
            IQuery q = CurrentSession.CreateQuery("from IndicatorMetadata i where i.IndicatorId = :indicatorId");
            q.SetParameter("indicatorId", indicatorId);
            IndicatorMetadata metadata = q.UniqueResult<IndicatorMetadata>();
            return metadata;
        }

        public Dictionary<int, int> GetIndicatorIdsByProfileIds(List<int> profileIds)
        {
            var result = CurrentSession.CreateCriteria<IndicatorMetadata>()
                .Add(Restrictions.In("OwnerProfileId", profileIds))
                .List<IndicatorMetadata>()
                .ToDictionary(x => x.IndicatorId, x => x.OwnerProfileId);
            return result;
        }

        public virtual IList<int> GetAllAgeIds()
        {
            IQuery q = CurrentSession.CreateQuery("select a.AgeID from Age a");
            return q.List<int>();
        }

        public virtual IList<int> GetAllSexIds()
        {
            IQuery q = CurrentSession.CreateQuery("select s.SexID from Sex s");
            return q.List<int>();
        }

        public virtual IList<int> GetAllValueNoteIds()
        {
            IQuery q = CurrentSession.CreateQuery("select vn.ValueNoteId from ValueNote vn");
            return q.List<int>();
        }

        public virtual IList<string> GetAllAreaCodes()
        {
            IQuery q = CurrentSession.CreateQuery("select a.AreaCode from Area a ");
            return q.List<string>();
        }

        public virtual IList<Category> GetAllCategories()
        {
            return CurrentSession.CreateCriteria<Category>()
                .List<Category>();
        }

        public IList<CoreDataSet> GetCoreDataForIndicatorIds(List<int> indicatorId)
        {
            IQuery q = CurrentSession.CreateQuery("from CoreDataSet cds where cds.IndicatorId in (:indicatorId)");
            q.SetParameterList("indicatorId", indicatorId);
            return q.List<CoreDataSet>();
        }

        public IList<CoreDataSetArchive> GetCoreDataArchiveForIndicatorIds(List<int> indicatorId)
        {
            IQuery q = CurrentSession.CreateQuery("from CoreDataSetArchive cdsa where cdsa.IndicatorId in (:indicatorId)");
            q.SetParameterList("indicatorId", indicatorId);
            return q.List<CoreDataSetArchive>();
        }

        public FpmUser GetUserByUserId(int userId)
        {
            IQuery q = CurrentSession.CreateQuery("from FpmUser fu where fu.Id = :userId");
            q.SetParameter("userId", userId);
            FpmUser user = q.UniqueResult<FpmUser>();
            return user;
        }

        public void ClearCache()
        {
            CurrentSession.Clear();
        }
    }
}
