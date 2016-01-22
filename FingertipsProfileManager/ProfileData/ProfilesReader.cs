using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData.Entities.Core;
using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Fpm.ProfileData
{
    public class ProfilesReader : BaseReader
    {
        public const int MaxAreaResults = 1000;

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

        public IEnumerable<ProfileCollection> GetProfileCollections()
        {
            IQuery q = CurrentSession.CreateQuery("from ProfileCollection pc");
            return q.List<ProfileCollection>();
        }

        public ProfileCollection GetProfileCollection(int collectionId)
        {
            IQuery q = CurrentSession.CreateQuery("from ProfileCollection pc where pc.id = :collectionId");
            q.SetParameter("collectionId", collectionId);
            return q.UniqueResult<ProfileCollection>();
        }

        public ProfileDetails GetProfileDetails(string urlKey)
        {
            IQuery q = CurrentSession.CreateQuery("from ProfileDetails p where p.UrlKey = :urlKey");
            q.SetParameter("urlKey", urlKey);
            return q.UniqueResult<ProfileDetails>();
        }
        
        public ProfileDetails GetProfileDetailsByProfileId(int profileId)
        {
            return CurrentSession.CreateCriteria<ProfileDetails>()
                .Add(Restrictions.Eq("Id", profileId))
                .UniqueResult<ProfileDetails>();
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

        public IList<UserGroupPermissions> GetUserGroupPermissionsByProfileId(int profileId)
        {
            IQuery q = CurrentSession.CreateQuery("from UserGroupPermissions ugp where ugp.ProfileId = :profileId");
            q.SetParameter("profileId", profileId);
            return q.List<UserGroupPermissions>();
        }

        public IList<Grouping> GetGroupings(int groupId)
        {
            IQuery q = CurrentSession.CreateQuery("from Grouping g where g.GroupId = :groupId order by g.Sequence");
            q.SetParameter("groupId", groupId);
            return q.List<Grouping>();
        }

        public IList<int> GetGroupingIds(int profileId)
        {
            IQuery q = CurrentSession.CreateQuery("select g.id from GroupingMetadata g where g.ProfileId = :profileId");
            q.SetParameter("profileId", profileId);
            return q.List<int>();
        }

        public IList<Grouping> GetGroupingsByGroupIdAndAreaTypeId(int groupId, int areaTypeId)
        {
            return CurrentSession.CreateCriteria<Grouping>()
                .Add(Restrictions.Eq("GroupId", groupId))
                .Add(Restrictions.Eq("AreaTypeId", areaTypeId))
                .AddOrder(Order.Asc("Sequence"))
                .List<Grouping>();
        }

        public IList<Grouping> GetGroupingsByGroupIdAreaTypeIdIndicatorIdAndSexId(int groupId, int areaTypeId,
            int indicatorId, int sexId)
        {
            return CurrentSession.CreateCriteria<Grouping>()
                .Add(Restrictions.Eq("GroupId", groupId))
                .Add(Restrictions.Eq("AreaTypeId", areaTypeId))
                .Add(Restrictions.Eq("IndicatorId", indicatorId))
                .Add(Restrictions.Eq("SexId", sexId))
                .List<Grouping>();
        }

        public IList<Grouping> GetGroupings(IList<int> groupIds, int areaTypeId,
            int indicatorId, int sexId, int ageId, int yearRange)
        {
            return CurrentSession.CreateCriteria<Grouping>()
                .Add(Restrictions.In("GroupId", groupIds.ToList()))
                .Add(Restrictions.Eq("AreaTypeId", areaTypeId))
                .Add(Restrictions.Eq("IndicatorId", indicatorId))
                .Add(Restrictions.Eq("SexId", sexId))
                .Add(Restrictions.Eq("AgeId", ageId))
                .Add(Restrictions.Eq("YearRange", yearRange))
                .List<Grouping>();
        }

        public IList<GroupingMetadata> GetGroupingMetadataList(IList<int> groupIds)
        {
            IQuery q = CurrentSession.CreateQuery("from GroupingMetadata g where g.GroupId in (:groupIds) order by g.Sequence");
            q.SetParameterList("groupIds", groupIds);
            return q.List<GroupingMetadata>();
        }

        public GroupingMetadata GetGroupingMetadata(int groupId)
        {
            return GetGroupingMetadataList(new List<int> { groupId }).ToList().FirstOrDefault();
        }

        public IList<Grouping> GetGroupingByIndicatorId(List<int> indicatorIds)
        {
            IQuery q = CurrentSession.CreateQuery("from Grouping g where g.IndicatorId in (:indicatorIds) order by g.GroupId");
            q.SetParameterList("indicatorIds", indicatorIds);
            return q.List<Grouping>();
        }

        public IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextProperties()
        {
            IQuery q = CurrentSession.CreateQuery("from IndicatorMetadataTextProperty i");
            return q.List<IndicatorMetadataTextProperty>();
        }

        public IList<IndicatorMetadataTextValue> SearchIndicatorMetadataTextValuesByText(string searchText)
        {
            return CurrentSession.CreateCriteria<IndicatorMetadataTextValue>()
                .Add(Restrictions.Like("Name", searchText))
                .Add(Restrictions.IsNull("ProfileId"))
             .List<IndicatorMetadataTextValue>();
        }

        public IList<IndicatorMetadataTextValue> SearchIndicatorMetadataTextValuesByIndicatorId(int indicatorId)
        {
            return CurrentSession.CreateCriteria<IndicatorMetadataTextValue>()
                .Add(Restrictions.Like("IndicatorId", indicatorId))
                .Add(Restrictions.IsNull("ProfileId"))
             .List<IndicatorMetadataTextValue>();
        }

        public IList GetIndicatorDefaultTextValues(int indicatorId)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetIndicatorDefaultMetadataTextValues");
            q.SetParameter("indicatorId", indicatorId);
            IList results = q.List();
            return results;
        }

        public IList<IndicatorText> GetIndicatorTextValues(int indicatorId, IList<IndicatorMetadataTextProperty> properties, int? profileId)
        {
            // Integer parameter required
            if (profileId.HasValue == false)
            {
                profileId = -1;
            }

            IQuery q = CurrentSession.GetNamedQuery("GetIndicatorMetadataTextValues");
            q.SetParameter("indicatorId", indicatorId);
            q.SetParameter("profileId", profileId);
            IList results = q.List();

            if (results.Count == 0)
            {
                return new List<IndicatorText>();
            }

            return HydrateIndicatorTextList(properties, results);
        }

        public IList<IndicatorText> GetOverriddenIndicatorTextValuesForSpecificProfileId(int indicatorId, IList<IndicatorMetadataTextProperty> properties, int? profileId)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetOverriddenIndicatorMetadataTextValuesForProfileId");
            q.SetParameter("indicatorId", indicatorId);
            q.SetParameter("profileId", profileId);
            IList results = q.List();

            if (results.Count == 0)
            {
                return new List<IndicatorText>();
            }

            return HydrateIndicatorTextList(properties, results);
        }

        private static List<IndicatorText> HydrateIndicatorTextList(IList<IndicatorMetadataTextProperty> properties, IList results)
        {
            IList<object> genericResults = (IList<object>)results[0];
            IList<object> specificResults = (results.Count == 2)
                ? (IList<object>)results[1]
                : null;

            List<IndicatorText> indicatorTextList = new List<IndicatorText>();
            for (int i = 0; i < properties.Count; i++)
            {
                var property = properties[i];

                IndicatorText indicatorText = new IndicatorText { IndicatorMetadataTextProperty = property };
                indicatorTextList.Add(indicatorText);

                // Add 1 to skip IndicatorID and GroupID
                int valueIndex = property.PropertyId + 1;
                indicatorText.ValueGeneric = (string)genericResults[valueIndex];

                if (specificResults != null)
                {
                    indicatorText.ValueSpecific = (string)specificResults[valueIndex];
                }
            }
            return indicatorTextList;
        }
       
        public IndicatorMetadata GetIndicatorMetadata(int indicatorId)
        {
            IQuery q = CurrentSession.CreateQuery("from IndicatorMetadata i where i.IndicatorId = :indicatorId");
            q.SetParameter("indicatorId", indicatorId);
            IndicatorMetadata metadata = q.UniqueResult<IndicatorMetadata>();
            return metadata;
        }

        public Dictionary<int,int> GetIndicatorIdsByProfileIds(List<int> profileIds)
        {
            var result = CurrentSession.CreateCriteria<IndicatorMetadata>()
                .Add(Restrictions.In("OwnerProfileId", profileIds))
                .List<IndicatorMetadata>()
                .ToDictionary(x => x.IndicatorId, x => x.OwnerProfileId);       
            return result;
        }

        public IEnumerable<Grouping> DoesIndicatorExistInMoreThanOneGroup(int indicatorId, int ageId, int sexId)
        {
            IQuery q = CurrentSession.CreateQuery("from Grouping g where g.IndicatorId = :indicatorId and g.SexId = :sexId and g.AgeId = :ageId");
            q.SetParameter("indicatorId", indicatorId);
            q.SetParameter("ageId", ageId);
            q.SetParameter("sexId", sexId);
            return q.List<Grouping>();
        }

        public virtual IList<double> GetAllComparatorConfidences()
        {
            return CurrentSession.CreateCriteria<ComparatorConfidence>()
                .SetProjection(Projections.Distinct(Projections.Property("ConfidenceValue")))
                .List<double>();
        }

        public virtual IList<int> GetAllAgeIds()
        {
            IQuery q = CurrentSession.CreateQuery("select a.AgeID from Age a");
            return q.List<int>();
        }

        public virtual IList<Age> GetAllAges()
        {
            return CurrentSession.CreateCriteria<Age>()
                .List<Age>();
        }

        public virtual IList<int> GetAllSexIds()
        {
            IQuery q = CurrentSession.CreateQuery("select s.SexID from Sex s");
            return q.List<int>();
        }

        public virtual IList<Sex> GetAllSexes()
        {
            return CurrentSession.CreateCriteria<Sex>()
                .List<Sex>();
        }

        public virtual IList<int> GetAllValueNoteIds()
        {
            IQuery q = CurrentSession.CreateQuery("select vn.ValueNoteId from ValueNote vn");
            return q.List<int>();
        }

        public virtual IList<ValueNote> GetAllValueNotes()
        {
            return CurrentSession.CreateCriteria<ValueNote>()
                .List<ValueNote>();
        }

        public virtual IList<string> GetAllAreaCodes()
        {
            IQuery q = CurrentSession.CreateQuery("select a.AreaCode from Area a ");
            return q.List<string>();
        }

        public virtual IList<TargetConfig> GetAllTargets()
        {
            return CurrentSession.CreateCriteria<TargetConfig>()
                .List<TargetConfig>();
        }

        public virtual TargetConfig GetTargetById(int targetId)
        {
            return CurrentSession.CreateCriteria<TargetConfig>()
                .Add(Restrictions.Eq("Id", targetId))
                .UniqueResult<TargetConfig>();
        }
        
        public virtual IList<Category> GetCategoriesByCategoryTypeId(int categoryTypeId)
        {
            return CurrentSession.CreateCriteria<Category>()
                .Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                .List<Category>();
        }

        public virtual IList<Category> GetAllCategories()
        {
            return CurrentSession.CreateCriteria<Category>()
                .List<Category>();
        }

        public IEnumerable<Area> GetAreas(string searchText, int? areaTypeId)
        {
            searchText = "%" + searchText + "%";

            var criteria = CurrentSession.CreateCriteria<Area>()
                .Add(Restrictions.And(
                    Restrictions.Eq("IsCurrent", true),
                    Restrictions.Or(
                        Restrictions.Like("AreaCode", searchText),
                        Restrictions.Like("AreaName", searchText))
                    ));

            if (areaTypeId.HasValue)
            {
                criteria.Add(Restrictions.In("AreaTypeId", new AreaTypeIdSplitter(areaTypeId.Value).Ids));
            }

            var areas = criteria.AddOrder(Order.Asc("AreaTypeId"))
                .AddOrder(Order.Asc("AreaCode"))
                .SetMaxResults(MaxAreaResults)
                .List<Area>();

            // Remove areas with non current area types
            var areaTypeIds = GetNonCurrentAreaTypes().Select(x => x.Id);
            return areas.Where(x => areaTypeIds.Contains(x.AreaTypeId) == false);
        }

        public IList<ComparatorMethod> GetAllComparatorMethods()
        {
            return CurrentSession.CreateCriteria<ComparatorMethod>()
                .Add(Restrictions.Eq("IsCurrent", true))
                .AddOrder(Order.Asc("Name"))
                .List<ComparatorMethod>();
        }

        public IList<Polarity> GetAllPolarities()
        {
            return CurrentSession.CreateCriteria<Polarity>()
                .AddOrder(Order.Asc("Name"))
                .List<Polarity>();
        }

        public IList<AreaType> GetAllAreaTypes()
        {
            return CurrentSession.CreateCriteria<AreaType>()
                .Add(Restrictions.Eq("IsCurrent", true))
                .AddOrder(Order.Asc("ShortName"))
                .List<AreaType>();
        }

        private IList<AreaType> GetNonCurrentAreaTypes()
        {
            return CurrentSession.CreateCriteria<AreaType>()
                .Add(Restrictions.Eq("IsCurrent", false))
                .List<AreaType>();
        }

        public IList<AreaType> GetSpecificAreaTypes(List<int> areaTypeIds)
        {
            return CurrentSession.CreateCriteria<AreaType>()
                .Add(Restrictions.In("Id", areaTypeIds))
                .AddOrder(Order.Asc("ShortName"))
                .List<AreaType>();
        }

        public IList<AreaType> GetSupportedAreaTypes()
        {
            return CurrentSession.CreateCriteria<AreaType>()
                .Add(Restrictions.Eq("IsCurrent", true))
                .Add(Restrictions.Eq("IsSupported", true))
                .AddOrder(Order.Asc("ShortName"))
                .List<AreaType>();
        }
        
        public IEnumerable<IndicatorAudit> GetIndicatorAudit(List<int> indicatorIds)
        {
            IQuery q = CurrentSession.CreateQuery("from IndicatorAudit ia where ia.IndicatorId in (:indicatorIds) order by ia.Timestamp desc");
            q.SetParameterList("indicatorIds", indicatorIds);
            return q.List<IndicatorAudit>();
        }

        public IEnumerable<ContentAudit> GetContentAuditList(List<int> contentIds)
        {
            IQuery q = CurrentSession.CreateQuery("from ContentAudit ca where ca.ContentId in (:contentIds) order by ca.Timestamp desc");
            q.SetParameterList("contentIds", contentIds);
            return q.List<ContentAudit>();
        }

        public ContentAudit GetContentAuditFromId(int contentAuditId)
        {
            return CurrentSession.CreateCriteria<ContentAudit>()
                .Add(Restrictions.Eq("Id", contentAuditId))
                .UniqueResult<ContentAudit>();
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


        public IList<TimePeriod> GetCoreDataSetTimePeriods(GroupingPlusName groupingPlusNames)
        {
            const string query = "select distinct d.Year, d.Quarter, d.Month from CoreDataSet d, Area a" +
              " where d.AreaCode = a.AreaCode and d.IndicatorId = :indicatorId and d.YearRange = :yearRange" +
              " and d.SexId = :sexId and d.AgeId = :ageId and a.AreaTypeId in (:areaTypeIds)" +
              " order by d.Year, d.Quarter, d.Month";

            IQuery q = CurrentSession.CreateQuery(query);
            q.SetParameter("indicatorId", groupingPlusNames.IndicatorId);
            q.SetParameter("sexId", groupingPlusNames.SexId);
            q.SetParameter("ageId", groupingPlusNames.AgeId);
            q.SetParameter("yearRange", groupingPlusNames.YearRange);
            q.SetParameterList("areaTypeIds", new AreaTypeIdSplitter(new List<int> { groupingPlusNames.AreaTypeId }).Ids);
            var rows = q.List<object[]>();

            List<TimePeriod> periods = new List<TimePeriod>();

            if (rows.LongCount() > 0 && rows[0] != null)
            {
                foreach (object[] row in rows)
                {
                    periods.Add(new TimePeriod
                    {
                        YearRange = groupingPlusNames.YearRange,
                        Year = (int) row[0],
                        Quarter = (int) row[1],
                        Month = (int) row[2]
                    });
                }
            }
            return periods;
        }

        public IList<ContentItem> GetProfileContentItems(int profileId)
        {
            IQuery q = CurrentSession.CreateQuery("from ContentItem fc where fc.ProfileId = :profileid");
            q.SetParameter("profileid", profileId);
            return q.List<ContentItem>();
        }

        public ContentItem GetContentItem(int contentId)
        {
            IQuery q = CurrentSession.CreateQuery("from ContentItem fc where fc.Id = :contentId");
            q.SetParameter("contentId", contentId);
            ContentItem contentItem = q.UniqueResult<ContentItem>();
            return contentItem;
        }

        public ContentItem GetContentItem(string contentKey, int profileId)
        {
            //Always get the latest contentItem
            CurrentSession.Clear();

            return CurrentSession.CreateCriteria<ContentItem>()
                .Add(Restrictions.Eq("ContentKey", contentKey))
                .Add(Restrictions.Eq("ProfileId", profileId))
                .UniqueResult<ContentItem>();
        }

        public IEnumerable<UserGroupPermissions> GetUserPermissions()
        {
            IQuery q = CurrentSession.CreateQuery("from UserGroupPermissions ugp");
            return q.List<UserGroupPermissions>();
        }

        public FpmUser GetUserByUserName(string userName)
        {
            IQuery q = CurrentSession.CreateQuery("from FpmUser fu where fu.UserName = :userName");
            q.SetParameter("userName", userName);
            FpmUser user = q.UniqueResult<FpmUser>();
            return user;
        }

        public FpmUser GetUserByUserId(int userId)
        {
            IQuery q = CurrentSession.CreateQuery("from FpmUser fu where fu.Id = :userId");
            q.SetParameter("userId", userId);
            FpmUser user = q.UniqueResult<FpmUser>();
            return user;
        }

       public IList<UserGroupPermissions> GetUserGroupPermissionsByUserAndProfile(int userId, int profileId)
        {
            IQuery q = CurrentSession.CreateQuery("from UserGroupPermissions ugp where ugp.ProfileId = :profileid and ugp.UserId = :userid");
            q.SetParameter("profileid", profileId);
            q.SetParameter("userid", userId);
            return q.List<UserGroupPermissions>();
        }

        public void ClearCache()
        {
            CurrentSession.Clear();
        }

      public IList<ProfileCollectionItem> GetProfileCollectionItems(int profilecollectionid)
        {
            IQuery q = CurrentSession.CreateQuery("from ProfileCollectionItem pci where pci.ProfileCollectionId = :profilecollectionid");
            q.SetParameter("profilecollectionid", profilecollectionid);
            return q.List<ProfileCollectionItem>();
        }

        public IList<ExceptionLog> GetExceptionsByServer(int exceptionDays, string exceptionServer)
        {
            DateTime initDate = DateTime.Today.AddDays(exceptionDays * -1);

            IQuery q = CurrentSession.CreateQuery(
                "from ExceptionLog e where e.Date > :initDate and e.Server = :exceptionServer order by e.Date desc");
            q.SetParameter("initDate", initDate);
            q.SetParameter("exceptionServer", exceptionServer);
            return q.List<ExceptionLog>();
        }

        public IList<ExceptionLog> GetExceptionsForAllServers(int exceptionDays)
        {
            DateTime initDate = DateTime.Today.AddDays(exceptionDays * -1);

            IQuery q = CurrentSession.CreateQuery(
                "from ExceptionLog el where el.Date > :initDate order by el.Id desc");
            q.SetParameter("initDate", initDate);
            return q.List<ExceptionLog>();
        }

        public ExceptionLog GetException(int exceptionId)
        {
            IQuery q = CurrentSession.CreateQuery("from ExceptionLog el where el.Id = :exceptionId");
            q.SetParameter("exceptionId", exceptionId);
            var exception = q.UniqueResult<ExceptionLog>();
            return exception;
        }

        public IEnumerable<string> GetDistinctExceptionServers()
        {
            IQuery q = CurrentSession.CreateQuery("select distinct el.Server from ExceptionLog el order by el.Server");
            return q.List<string>();
        }


        public IndicatorMetadataTextValue GetMetadataTextValueForAnIndicatorById(int indicatorId, int profileId)
        {
            return CurrentSession.CreateCriteria<IndicatorMetadataTextValue>()
                .Add(Restrictions.Eq("IndicatorId", indicatorId))
                .Add(Restrictions.Eq("ProfileId", profileId))
                .UniqueResult<IndicatorMetadataTextValue>();
        }

        public IList<Document> GetDocuments(int profileId)
        {
            return CurrentSession.CreateCriteria<Document>()
                    .Add(Restrictions.Eq("ProfileId", profileId))
                    .List<Document>();
        }

        public Document GetDocument(int id)
        {
            Document documentTypeCast = null; // used for transforming object[] to DocumentType via Transform
            var doc = CurrentSession.QueryOver<Document>()
            .Where(d=> d.Id == id)
            .Select(
                    Projections.Property<Document>(d => d.Id).WithAlias(() => documentTypeCast.Id),
                    Projections.Property<Document>(d => d.ProfileId).WithAlias(() => documentTypeCast.ProfileId),                    
                    Projections.Property<Document>(d => d.FileName).WithAlias(() => documentTypeCast.FileName),
                    Projections.Property<Document>(d => d.FileData).WithAlias(() => documentTypeCast.FileData),
                    Projections.Property<Document>(d => d.UploadedOn).WithAlias(() => documentTypeCast.UploadedOn),
                    Projections.Property<Document>(d => d.UploadedBy).WithAlias(() => documentTypeCast.UploadedBy)
            )
            .TransformUsing(Transformers.AliasToBean<Document>())
            .SingleOrDefault<Document>();

            return doc;
        }

        public IList<Document> GetDocuments(string fileName)
        {
            return CurrentSession.CreateCriteria<Document>()
                .Add(Restrictions.Eq("FileName", fileName))
                .List<Document>();
        }

        public IEnumerable<SpineChartMinMaxLabel> GetSpineChartMinMaxLabelOptions()
        {
            return CurrentSession.CreateCriteria<SpineChartMinMaxLabel>()
                .List<SpineChartMinMaxLabel>();
        }

        public IList<AreaType> GetAreaTypes(int profileId)
        {
            var query = CurrentSession.GetNamedQuery("GetAreaTypesForProfileId")
                .SetParameter("profileId", profileId)
                .SetResultTransformer(Transformers.AliasToBean(typeof(AreaType)));
            return query.List<AreaType>();
        }

        public IList<AreaType> GetAreaTypesWhichContainsPdf(int profileId)
        {
            var query = CurrentSession.GetNamedQuery("GetPDFEnabledAreaTypesForProfileId")
                .SetParameter("profileId", profileId)
                .SetResultTransformer(Transformers.AliasToBean(typeof(AreaType)));
            return query.List<AreaType>();
        }

        public IList<UserGroupPermissions> GetProfileUsers(int profileId)
        {
            IQuery q = CurrentSession.CreateQuery("from UserGroupPermissions ugp where ugp.ProfileId = :profileid");
            q.SetParameter("profileid", profileId);
            return q.List<UserGroupPermissions>();
        }
    }
}
