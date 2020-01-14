using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.DataAccess
{
    public interface IGroupDataReader
    {
        IList<int> GetIndicatorIdsByGroupIdAndAreaTypeId(int groupId, int areaTypeId);
        GroupingMetadata GetGroupingMetadata(int groupId);
        IList<GroupingMetadata> GetGroupingMetadataList(IList<int> groupIds);
        IList<int> GetGroupIdsOfProfile(int profileId);
        Grouping GetGroupingsByGroupIdAndIndicatorId(int groupId, int indicatorId);
        IList<Grouping> GetGroupingListByGroupIdIndicatorIdAreaType(int groupId, int indicatorId, int areaTypeId);
        IList<Grouping> GetGroupingListByIndicatorIdAreaType(int indicatorId, int areaTypeId);
        int GetCommonestPolarityForIndicator(int indicatorId);
        IList<Grouping> GetGroupingsByGroupIdsAndIndicatorId(List<int> groupIds, int indicatorId);
        IList<Grouping> GetGroupings(int groupId, int indicatorId, int areaTypeId, int sexId, int ageId);
        IList<Grouping> GetGroupings(int groupId, int indicatorId, int sexId, int ageId);
        IList<Grouping> GetGroupingsByGroupIdIndicatorIdSexId(int groupId, int areaTypeId, int indicatorId, int sexId);
        IList<Grouping> GetGroupingsByGroupIdIndicatorIdAgeId(int groupId, int areaTypeId, int indicatorId, int ageId);
        IList<Grouping> GetGroupingsByGroupIdAndAreaTypeId(int groupId, int areaTypeId);
        IList<Grouping> GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(int groupId, int areaTypeId);
        IList<Grouping> GetGroupingsByIndicatorIds(IList<int> indicatorIds, IList<int> groupingIds);
        IList<Grouping> GetGroupingsByIndicatorId(int indicatorId);
        IList<Grouping> GetGroupingsByGroupIdsAndIndicatorIdsAndAreaType(IList<int> groupIds,
        IList<int> indicatorIds, int areaTypeId);
        IList<Grouping> GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(int areaTypeId,
            int indicatorId, int sexId, int ageId);
        IList<GroupingData> GetAvailableDataByIndicatorIdAndAreaTypeId(int? indicatorId, int? areaTypeId);
        IList<Grouping> GetGroupingsByGroupIdsAndIndicatorIds(IList<int> groupIds, IList<int> indicatorIds);

        /// <summary>
        /// NOTE: should use AgeID - need to change in all calling services
        /// </summary>
        IList<Grouping> GetGroupingsWithoutAgeId(int groupId, int indicatorId, int areaTypeId, int sexId);

        IList<int> GetDistinctGroupingAreaTypeIdsForAllProfiles();
        IList<int> GetDistinctGroupingAreaTypeIds(IList<int> groupIds);
        IList<Grouping> GetGroupingsByGroupId(int id);
        IList<Grouping> GetGroupingsByGroupIds(IList<int> ids);

        /// <summary>
        /// Key is child area code, value is category ID
        /// </summary>
        Dictionary<string, int> GetCategoriesWithinParentArea(int categoryTypeId,
            string parentAreaCode, int childAreaTypeId, int parentAreaType);

        IList<int> GetAllAgeIdsForIndicator(int indicatorId);

        /// <summary>
        /// Selects data without discriminating by ageId. To only be used when multiple age bands are
        /// required together, e.g. for population pyramids.
        /// </summary>
        IList<CoreDataSet> GetCoreDataForAllAges(int indicatorId, TimePeriod period,
            string areaCode, int sexId);

        IList<CoreDataSet> GetCoreDataForAllSexes(int indicatorId, TimePeriod period,
            string areaCode, int ageId);

        IList<CoreDataSet> GetCoreData(int indicatorId, string areaCode);
        IList<CoreDataSet> GetCoreData(Grouping grouping, TimePeriod period, params string[] areaCodes);

        CoreDataSet GetCoreDataForCategoryArea(Grouping grouping,
            TimePeriod timePeriod, ICategoryArea categoryArea);

        CoreDataSet GetCoreDataForCategory(Grouping grouping,
            TimePeriod timePeriod, string areaCode, int categoryTypeId, int categoryId);

        IList<CoreDataSet> GetAllCategoryDataWithinParentArea(string parentAreaCode,
            int indicatorId, int sexId, int ageId, TimePeriod timePeriod);

        IList<CoreDataSet> GetCoreDataListForChildrenOfArea(Grouping grouping, TimePeriod period, string parentAreaCode);

        IList<CoreDataSet> GetCoreDataListForChildrenOfCategoryArea(Grouping grouping,
            TimePeriod period, CategoryArea categoryArea);

        IList<CoreDataSet> GetCoreDataListForChildrenOfNearestNeighbourArea(Grouping grouping,
            TimePeriod period, NearestNeighbourArea area);

        IList<CoreDataSet> GetCoreDataListForAllCategoryAreasOfCategoryAreaType(Grouping grouping,
            TimePeriod timePeriod, int categoryTypeId, string areaCode);

        Limits GetCoreDataLimitsByIndicatorId(int indicatorId);
        Limits GetCoreDataLimitsByIndicatorIdAndAreaTypeId(int indicatorId, int areaTypeId);
        Limits GetCoreDataLimitsByIndicatorIdAndAreaTypeIdAndParentAreaCode(int indicatorId, int areaTypeId, string parentAreaCode);
        int GetCoreDataCountAtDataPoint(Grouping grouping);
        IList<CoreDataSet> GetCoreDataForAllAreasOfType(Grouping grouping, TimePeriod period);
        int GetCoreDataCountWhereBothCI95AreMinusOne();
        int GetCoreDataCountWhereBothCI998AreMinusOne();
        IList<CoreDataSet> GetCoreDataForIndicatorId(int indicatorId);

        IList<double> GetOrderedCoreDataValidValuesForAllAreasOfType(Grouping grouping, TimePeriod period,
            IEnumerable<string> ignoredAreaCodes);

        bool GetIndicatorMetadataAlwaysShowSexWithIndicatorName(int indicatorId);
        bool GetIndicatorMetadataAlwaysShowAgeWithIndicatorName(int indicatorId);

        IList<IndicatorMetadata> GetIndicatorMetadata(IList<Grouping> groupings,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties, int profileId);

        IList<IndicatorMetadata> GetIndicatorMetadata(IList<Grouping> groupings,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties);

        IndicatorMetadata GetIndicatorMetadata(Grouping grouping,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties);

        IndicatorMetadata GetIndicatorMetadata(int indicatorId,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties);

        IndicatorMetadata GetIndicatorMetadata(int indicatorId,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties,
            int profileId);

        IList<IndicatorMetadata> GetIndicatorMetadata(IList<int> indicatorIds,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties);

        IList<IndicatorMetadata> GetIndicatorMetadata(IList<int> indicatorIds,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties,
            int profileId);

        IList<IndicatorMetadataTextValue> GetIndicatorMetadataTextValues(int indicatorId);

        IList<IndicatorMetadata> GetGroupSpecificIndicatorMetadataTextValues(IList<Grouping> groupings,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties);

        IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextProperties();
        IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextPropertiesIncludingInternalMetadata();

        IList<int> GetAllIndicatorIds();

        IList<CoreDataSet> GetDataIncludingInequalities(int indicatorId, TimePeriod period,
            IList<int> excludedCategoryTypeIds, params string[] areaCodes);

        IList<int> GetAllCategoryTypeIds(string areaCode, int indicatorId, int sexId, int ageId);
        int GetCoreDataMaxYear(int indicatorId);
        IList<GroupingSubheading> GetGroupingSubheadings(int areaTypeId, int groupId);

        void ClearSession();
    }

    public class GroupDataReader : BaseReader, IGroupDataReader
    {
        private const string PropertyValue = "Value";
        private const string PropertyIndicatorId = "IndicatorId";
        private const string PropertyAreaTypeId = "AreaTypeId";

        private const string CategoryIdIsUndefined = " and d.CategoryId = -1";

        private const string TimePeriodClause =
            " and d.Year = :year and d.YearRange = :yearRange and d.Quarter = :quarter and d.Month = :month";

        private const string GetCoreDataQueryString =
             "from CoreDataSet d where d.IndicatorId = :indicatorId" + TimePeriodClause +
             " and d.SexId = :sexId and d.AgeId = :ageId and d.AreaCode {0}" + CategoryIdIsUndefined;

        private const string GetCoreDataListForChildrenOfAreaQueryString =
            "select d from CoreDataSet d, AreaHierarchy h, Area a" +
            " where d.AreaCode = h.ChildAreaCode and d.AreaCode = a.Code and d.IndicatorId = :indicatorId" +
            TimePeriodClause + " and d.SexId = :sexId and d.AgeId = :ageId and h.ParentAreaCode = :parentAreaCode" +
            " and a.AreaTypeId in (:areaTypeIds) and a.IsCurrent = 1" + CategoryIdIsUndefined;

        private const string GetCoreDataListForChildrenOfCategoryAreaQueryString =
            "select d from CoreDataSet d, CategorisedArea c, Area a" +
            " where d.AreaCode = c.AreaCode and a.Code = c.AreaCode" + CategoryIdIsUndefined +
            " and d.IndicatorId = :indicatorId" + TimePeriodClause +
            " and d.SexId = :sexId and d.AgeId = :ageId and a.IsCurrent = 1" +
            " and c.CategoryTypeId = :categoryTypeId and c.CategoryId = :categoryId" +
            " and c.ChildAreaTypeId = :childAreaTypeId and c.ParentAreaTypeId = :parentAreaTypeId";

        private const string GetCoreDataListForChildrenOfNearestNeighbourAreaQueryString =
            "select d from CoreDataSet d, AreaCodeNeighbourMapping n" +
            " where d.AreaCode = n.NeighbourAreaCode and n.AreaCode = :neighbourAreaCode" + CategoryIdIsUndefined +
            " and d.IndicatorId = :indicatorId" + TimePeriodClause +
            " and d.SexId = :sexId and d.AgeId = :ageId and n.NeighbourTypeId = :neighbourTypeId";

        private const string GetCoreDataForAllAreasOfTypeQueryString =
            "select d from CoreDataSet d, Area a" +
            " where d.AreaCode = a.Code and d.IndicatorId = :indicatorId" +
            TimePeriodClause + " and d.SexId = :sexId and d.AgeId = :ageId and a.AreaTypeId in (:areaTypeIds) and a.IsCurrent = 1" + CategoryIdIsUndefined;

        private const string GetOrderedCoreDataValidValuesForAllAreasOfTypeQueryString =
            "select d.Value from CoreDataSet d, Area a" +
            " where d.AreaCode = a.Code and d.IndicatorId = :indicatorId" +
            TimePeriodClause + " and d.SexId = :sexId and d.AgeId = :ageId and a.AreaTypeId in (:areaTypeIds) and a.IsCurrent = 1" +
            " and d.Value != -1" + CategoryIdIsUndefined;

        private const string GetCoreDataByIndicatorIdAndAreaCodeQueryString =
            "from CoreDataSet d where d.IndicatorId = :indicatorId and d.AreaCode = :areaCode" + CategoryIdIsUndefined;

        private const string OrderByIndicatorIdAndNewness =
            "order by g.IndicatorId, g.DataPointYear desc, g.DataPointQuarter desc, g.DataPointMonth desc";

        private const string GetCoreDataQueryStringInequalities =
            "from CoreDataSet d where d.IndicatorId = :indicatorId" + TimePeriodClause +
            " and d.AreaCode {0}";

        /// <summary>
        /// For Mock object creation.
        /// </summary>
        public GroupDataReader() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sessionFactory">The session factory</param>
        internal GroupDataReader(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public void ClearSession()
        {
            CurrentSession.Clear();
        }

        public IList<int> GetIndicatorIdsByGroupIdAndAreaTypeId(int groupId, int areaTypeId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "select g from Grouping g where g.GroupId = :groupId and g.AreaTypeId = :areaTypeId order by g.Sequence");
            q.SetParameter(ParameterGroupId, groupId);
            q.SetParameter("areaTypeId", areaTypeId);
            IList<Grouping> groupings = q.List<Grouping>();
            return (from g in groupings select g.IndicatorId).Distinct().ToList();
        }

        public GroupingMetadata GetGroupingMetadata(int groupId)
        {
            return CurrentSession.CreateCriteria<GroupingMetadata>()
                .Add(Restrictions.Eq("Id", groupId))
                .UniqueResult<GroupingMetadata>();
        }

        public virtual IList<GroupingMetadata> GetGroupingMetadataList(IList<int> groupIds)
        {
            // Guard clause to avoid case where query will fail to execute
            if (groupIds.Any() == false)
            {
                return new List<GroupingMetadata>();
            }

            IQuery q = CurrentSession.CreateQuery("from GroupingMetadata g where g.Id in (:groupId) order by g.Sequence");
            q.SetParameterList(ParameterGroupId, groupIds.ToList());
            return q.List<GroupingMetadata>();
        }

        public IList<int> GetGroupIdsOfProfile(int profileId)
        {
            IQuery q = CurrentSession.CreateQuery("select g.id from GroupingMetadata g where g.ProfileId = :profileId order by g.Sequence");
            q.SetCacheable(true);
            q.SetParameter("profileId", profileId);
            return q.List<int>();
        }

        public IList<Grouping> GetGroupingsByIndicatorId(int indicatorId)
        {
            IQuery q = CurrentSession.CreateQuery("select g from Grouping g where g.IndicatorId = :indicatorId");
            q.SetParameter(ParameterIndicatorId, indicatorId);
            return q.List<Grouping>();
        }

        public Grouping GetGroupingsByGroupIdAndIndicatorId(int groupId, int indicatorId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "from Grouping g where g.GroupId = :groupId and g.IndicatorId = :indicatorId order by g.DataPointYear");
            q.SetParameter(ParameterGroupId, groupId);
            q.SetParameter(ParameterIndicatorId, indicatorId);
            return q.List<Grouping>().FirstOrDefault();
        }

        public IList<Grouping> GetGroupingListByGroupIdIndicatorIdAreaType(int groupId, int indicatorId, int areaTypeId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "from Grouping g where g.GroupId = :groupId and g.IndicatorId = :indicatorId and g.AreaTypeId = :areaTypeId order by g.DataPointYear");
            q.SetParameter(ParameterGroupId, groupId);
            q.SetParameter(ParameterIndicatorId, indicatorId);
            q.SetParameter(ParameterAreaType, areaTypeId);
            return q.List<Grouping>();
        }

        public IList<Grouping> GetGroupingListByIndicatorIdAreaType(int indicatorId, int areaTypeId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "from Grouping g where g.IndicatorId = :indicatorId and g.AreaTypeId = :areaTypeId order by g.DataPointYear");
            q.SetParameter(ParameterIndicatorId, indicatorId);
            q.SetParameter(ParameterAreaType, areaTypeId);
            return q.List<Grouping>();
        }

        public IList<Grouping> GetGroupingsByGroupIdsAndIndicatorId(List<int> groupIds, int indicatorId)
        {
            return CurrentSession.CreateCriteria<Grouping>()
                .Add(Restrictions.Eq("IndicatorId", indicatorId))
                .Add(Restrictions.In("GroupId", groupIds))
                .List<Grouping>();
        }

        public IList<Grouping> GetGroupings(int groupId, int indicatorId, int areaTypeId, int sexId, int ageId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "from Grouping g where g.GroupId = :groupId and g.AgeId = :ageId and g.SexId = :sexId and g.IndicatorId = :indicatorId and g.AreaTypeId = :areaTypeId  order by g.Sequence");
            q.SetParameter(ParameterGroupId, groupId);
            q.SetParameter("ageId", ageId);
            q.SetParameter("areaTypeId", areaTypeId);
            q.SetParameter(ParameterIndicatorId, indicatorId);
            q.SetParameter(ParameterSexId, sexId);
            return q.List<Grouping>();
        }

        public IList<Grouping> GetGroupings(int groupId, int indicatorId, int sexId, int ageId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "from Grouping g where g.GroupId = :groupId and g.AgeId = :ageId and g.SexId = :sexId and g.IndicatorId = :indicatorId  order by g.Sequence");
            q.SetParameter(ParameterGroupId, groupId);
            q.SetParameter("ageId", ageId);
            q.SetParameter(ParameterIndicatorId, indicatorId);
            q.SetParameter(ParameterSexId, sexId);
            return q.List<Grouping>();
        }

        public IList<Grouping> GetGroupingsByGroupIdIndicatorIdSexId(int groupId, int areaTypeId, int indicatorId, int sexId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "from Grouping g where g.GroupId = :groupId and g.AreaTypeId = :areaTypeId and g.SexId = :sexId and g.IndicatorId = :indicatorId  order by g.Sequence");
            q.SetParameter(ParameterGroupId, groupId);
            q.SetParameter("areaTypeId", areaTypeId);
            q.SetParameter(ParameterIndicatorId, indicatorId);
            q.SetParameter(ParameterSexId, sexId);
            return q.List<Grouping>();
        }

        public IList<Grouping> GetGroupingsByGroupIdIndicatorIdAgeId(int groupId, int areaTypeId, int indicatorId, int ageId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "from Grouping g where g.GroupId = :groupId and g.AreaTypeId = :areaTypeId and g.AgeId = :ageId and g.IndicatorId = :indicatorId  order by g.Sequence");
            q.SetParameter(ParameterGroupId, groupId);
            q.SetParameter("areaTypeId", areaTypeId);
            q.SetParameter("ageId", ageId);
            q.SetParameter(ParameterIndicatorId, indicatorId);
            return q.List<Grouping>();
        }

        public IList<Grouping> GetGroupingsByGroupIdAndAreaTypeId(int groupId, int areaTypeId)
        {
            IQuery q = CurrentSession.CreateQuery("from Grouping g where g.GroupId = :groupId and g.AreaTypeId = :areaTypeId " +
                OrderByIndicatorIdAndNewness);
            q.SetParameter(ParameterGroupId, groupId);
            q.SetParameter("areaTypeId", areaTypeId);
            return q.List<Grouping>();
        }

        public IList<Grouping> GetGroupingsByGroupIdAndAreaTypeIdOrderedBySequence(int groupId, int areaTypeId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "from Grouping g where g.GroupId = :groupId and g.AreaTypeId = :areaTypeId order by g.Sequence");
            q.SetParameter(ParameterGroupId, groupId);
            q.SetParameter("areaTypeId", areaTypeId);
            return q.List<Grouping>();
        }

        public IList<Grouping> GetGroupingsByIndicatorIds(IList<int> indicatorIds, IList<int> groupingIds)
        {
            IQuery q = CurrentSession.CreateQuery(
                "from Grouping g where g.GroupId in (:groupId) and g.IndicatorId in (:indicatorIds) " +
                OrderByIndicatorIdAndNewness);
            q.SetParameterList("indicatorIds", indicatorIds.ToList());
            q.SetParameterList(ParameterGroupId, groupingIds.ToList());
            return q.List<Grouping>();
        }

        public virtual IList<Grouping> GetGroupingsByGroupIdsAndIndicatorIdsAndAreaType(IList<int> groupIds,
            IList<int> indicatorIds, int areaTypeId)
        {
            return CurrentSession.CreateCriteria<Grouping>()
                .Add(Restrictions.In("GroupId", groupIds.ToArray()))
                .Add(Restrictions.In(PropertyIndicatorId, indicatorIds.ToArray()))
                .Add(Restrictions.Eq(PropertyAreaTypeId, areaTypeId))
                .List<Grouping>();
        }

        public virtual int GetCommonestPolarityForIndicator(int indicatorId)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetMostCommonPolarityIdForIndicator");
            q.SetParameter("indicatorId", indicatorId);
            return q.UniqueResult<int>();
        }

        public virtual IList<Grouping> GetGroupingByAreaTypeIdAndIndicatorIdAndSexIdAndAgeId(
            int areaTypeId, int indicatorId, int sexId, int ageId)
        {
            return CurrentSession.QueryOver<Grouping>()
                .Where(
                    x =>
                        x.IndicatorId == indicatorId &&
                        x.AreaTypeId == areaTypeId &&
                        x.SexId == sexId &&
                        x.AgeId == ageId)
                .List();
        }

        public virtual IList<Grouping> GetGroupingsByGroupIdsAndIndicatorIds(IList<int> groupIds,
            IList<int> indicatorIds)
        {
            List<Grouping> allGroupings = new List<Grouping>();

            // Query the database a little at a time, for large numbers of items the query will fail otherwise
            var splitter = new LongListSplitter<int>(indicatorIds);
            while (splitter.AnyLeft())
            {
                var nextData = CurrentSession.CreateCriteria<Grouping>()
                    .Add(Restrictions.In("GroupId", groupIds.ToArray()))
                    .Add(Restrictions.In(PropertyIndicatorId, splitter.NextItems().ToArray()))
                    .List<Grouping>();
                allGroupings.AddRange(nextData);
            }

            return allGroupings;
        }

        public IList<GroupingData> GetAvailableDataByIndicatorIdAndAreaTypeId(int? indicatorId, int? areaTypeId)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetAvailableGroupingDataByIndicatorIdAndAreaTypeId");
            q.SetParameter("indicatorId", indicatorId);
            q.SetParameter("areaTypeId", areaTypeId);
            q.SetResultTransformer(Transformers.AliasToBean<GroupingData>());
            return q.List<GroupingData>();
        }
        /// <summary>
        /// NOTE: should use AgeID - need to change in all calling services
        /// </summary>
        public IList<Grouping> GetGroupingsWithoutAgeId(int groupId, int indicatorId, int areaTypeId, int sexId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "from Grouping g where g.GroupId = :groupId and g.IndicatorId = :indicatorId and g.AreaTypeId = :areaTypeId and g.SexId = :sexId");
            q.SetParameter(ParameterGroupId, groupId);
            q.SetParameter(ParameterIndicatorId, indicatorId);
            q.SetParameter("areaTypeId", areaTypeId);
            q.SetParameter(ParameterSexId, sexId);
            return q.List<Grouping>();
        }

        public virtual IList<int> GetDistinctGroupingAreaTypeIdsForAllProfiles()
        {
            IQuery q = CurrentSession.CreateQuery("select distinct g.AreaTypeId from Grouping g");
            q.SetCacheable(true);
            return q.List<int>();
        }

        public virtual IList<int> GetDistinctGroupingAreaTypeIds(IList<int> groupIds)
        {
            // Guard clause to avoid case where query will fail to execute
            if (groupIds.Any() == false)
            {
                return new List<int>();
            }

            IQuery q = CurrentSession.CreateQuery("select distinct g.AreaTypeId from Grouping g where g.GroupId in (:groupId)");
            q.SetParameterList(ParameterGroupId, groupIds.ToList());
            return q.List<int>();
        }

        public IList<Grouping> GetGroupingsByGroupId(int groupId)
        {
            IQuery q = CurrentSession.CreateQuery("from Grouping g where g.GroupId = :groupId order by g.Sequence");
            q.SetParameter(ParameterGroupId, groupId);
            return q.List<Grouping>();
        }

        public IList<Grouping> GetGroupingsByGroupIds(IList<int> ids)
        {
            IQuery q = CurrentSession.CreateQuery("from Grouping g where g.GroupId in (:groupId) order by g.Sequence");
            q.SetParameterList(ParameterGroupId, ids.ToList());
            return q.List<Grouping>();
        }

        /// <summary>
        /// Key is child area code, value is category ID
        /// </summary>
        public Dictionary<string, int> GetCategoriesWithinParentArea(int categoryTypeId,
            string parentAreaCode, int childAreaTypeId, int parentAreaType)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetCategoriesWithinParentArea");
            q.SetParameter("parentAreaType", parentAreaType);
            q.SetParameter("parentAreaCode", parentAreaCode);
            q.SetParameter("childAreaTypeId", childAreaTypeId);
            q.SetParameter("categoryTypeId", categoryTypeId);
            var results = q.List();

            return results.Cast<object[]>().ToDictionary(
                resultArray => (string)resultArray[0],
                resultArray => Convert.ToInt32(resultArray[1])
                );
        }

        public IList<int> GetAllAgeIdsForIndicator(int indicatorId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "select distinct c.AgeId from CoreDataSet c where c.IndicatorId = :indicatorId");
            q.SetCacheable(true);
            q.SetParameter(ParameterIndicatorId, indicatorId);
            return q.List<int>();
        }

        /// <summary>
        /// This should always be zero. FUS should have converted double -1 to nulls.
        /// </summary>
        public int GetCoreDataCountWhereBothCI95AreMinusOne()
        {
            return CurrentSession.QueryOver<CoreDataSet>()
                .Where(x => x.LowerCI95 == ValueData.NullValue && x.UpperCI95 == ValueData.NullValue)
                .RowCount();
        }

        /// <summary>
        /// This should always be zero. FUS should have converted double -1 to nulls.
        /// </summary>
        public int GetCoreDataCountWhereBothCI998AreMinusOne()
        {
            return CurrentSession.QueryOver<CoreDataSet>()
                .Where(x => x.LowerCI99_8 == ValueData.NullValue && x.UpperCI99_8 == ValueData.NullValue)
                .RowCount();
        }

        /// <summary>
        /// Gets the list of core data set based on the indicator id
        /// </summary>
        /// <param name="indicatorId">The indicator id</param>
        /// <returns></returns>
        public virtual IList<CoreDataSet> GetCoreDataForIndicatorId(int indicatorId)
        {
            return CurrentSession.CreateCriteria<CoreDataSet>()
                .Add(Restrictions.Eq("IndicatorId", indicatorId))
                .List<CoreDataSet>();
        }

        /// <summary>
        /// Selects data without discriminating by ageId. To only be used when multiple age bands are
        /// required together, e.g. for population pyramids.
        /// </summary>
        public virtual IList<CoreDataSet> GetCoreDataForAllAges(int indicatorId, TimePeriod period,
            string areaCode, int sexId)
        {
            var criteria = CurrentSession.CreateCriteria<CoreDataSet>()
                .Add(Restrictions.Eq("IndicatorId", indicatorId))
                .Add(Restrictions.Eq("SexId", sexId))
                .Add(Restrictions.Eq("AreaCode", areaCode))
                .Add(Restrictions.Eq("CategoryTypeId", CategoryTypeIds.Undefined));

            AddTimePeriodRestrictions(period, criteria);

            return criteria.List<CoreDataSet>();
        }

        public virtual IList<CoreDataSet> GetCoreDataForAllSexes(int indicatorId, TimePeriod period,
            string areaCode, int ageId)
        {
            var criteria = CurrentSession.CreateCriteria<CoreDataSet>()
                .Add(Restrictions.Eq("IndicatorId", indicatorId))
                .Add(Restrictions.Eq("AgeId", ageId))
                .Add(Restrictions.Eq("AreaCode", areaCode))
                .Add(Restrictions.Eq("CategoryTypeId", CategoryTypeIds.Undefined));

            AddTimePeriodRestrictions(period, criteria);

            return criteria.List<CoreDataSet>();
        }

        private static void AddTimePeriodRestrictions(TimePeriod period, ICriteria criteria)
        {
            criteria
                .Add(Restrictions.Eq("Year", period.Year))
                .Add(Restrictions.Eq("YearRange", period.YearRange))
                .Add(Restrictions.Eq("Quarter", period.Quarter))
                .Add(Restrictions.Eq("Month", period.Month));
        }

        public IList<CoreDataSet> GetCoreData(int indicatorId, string areaCode)
        {
            IQuery q = CurrentSession.CreateQuery(GetCoreDataByIndicatorIdAndAreaCodeQueryString);
            q.SetParameter("areaCode", areaCode);
            q.SetParameter(ParameterIndicatorId, indicatorId);
            return q.List<CoreDataSet>();
        }

        public virtual IList<CoreDataSet> GetCoreData(Grouping grouping, TimePeriod period, params string[] areaCodes)
        {
            if (areaCodes.Length == 0)
            {
                return new List<CoreDataSet>();
            }

            var allData = new List<CoreDataSet>();

            // Query the database 1000 areas at a time, for large numbers of areas the query will fail otherwise
            var splitter = new LongListSplitter<string>(areaCodes);
            while (splitter.AnyLeft())
            {
                var nextData = GetCoreDataForAreaCodes(grouping, period, splitter.NextItems());
                allData.AddRange(nextData);
            }
            return allData;
        }

        private IEnumerable<CoreDataSet> GetCoreDataForAreaCodes(Grouping grouping, TimePeriod period, IEnumerable<string> areaCodes)
        {
            IQuery q = CurrentSession.CreateQuery(string.Format(GetCoreDataQueryString, "in (:areaCodes)"));
            q.SetParameterList("areaCodes", areaCodes);

            SetTimePeriodParameters(q, period);

            SetGroupingParameters(q, grouping);

            var nextData = q.List<CoreDataSet>();
            return nextData;
        }

        public virtual CoreDataSet GetCoreDataForCategoryArea(Grouping grouping,
            TimePeriod timePeriod, ICategoryArea categoryArea)
        {
            return CurrentSession.CreateCriteria<CoreDataSet>()
                .Add(Restrictions.Eq("IndicatorId", grouping.IndicatorId))
                .Add(Restrictions.Eq("Year", timePeriod.Year))
                .Add(Restrictions.Eq("YearRange", timePeriod.YearRange))
                .Add(Restrictions.Eq("Quarter", timePeriod.Quarter))
                .Add(Restrictions.Eq("Month", timePeriod.Month))
                .Add(Restrictions.Eq("AreaCode", categoryArea.ParentAreaCode))
                .Add(Restrictions.Eq("SexId", grouping.SexId))
                .Add(Restrictions.Eq("AgeId", grouping.AgeId))
                .Add(Restrictions.Eq("CategoryTypeId", categoryArea.CategoryTypeId))
                .Add(Restrictions.Eq("CategoryId", categoryArea.CategoryId))
                .UniqueResult<CoreDataSet>();
        }

        public CoreDataSet GetCoreDataForCategory(Grouping grouping, TimePeriod timePeriod,
            string areaCode, int categoryTypeId, int categoryId)
        {
            return CurrentSession.CreateCriteria<CoreDataSet>()
                .Add(Restrictions.Eq("IndicatorId", grouping.IndicatorId))
                .Add(Restrictions.Eq("Year", timePeriod.Year))
                .Add(Restrictions.Eq("YearRange", timePeriod.YearRange))
                .Add(Restrictions.Eq("Quarter", timePeriod.Quarter))
                .Add(Restrictions.Eq("Month", timePeriod.Month))
                .Add(Restrictions.Eq("SexId", grouping.SexId))
                .Add(Restrictions.Eq("AgeId", grouping.AgeId))
                .Add(Restrictions.Eq("AreaCode", areaCode))
                .Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                .Add(Restrictions.Eq("CategoryId", categoryId))
                .UniqueResult<CoreDataSet>();
        }

        public IList<CoreDataSet> GetAllCategoryDataWithinParentArea(string parentAreaCode,
                int indicatorId, int sexId, int ageId, TimePeriod timePeriod)
        {
            return CurrentSession.CreateCriteria<CoreDataSet>()
           .Add(Restrictions.Eq("IndicatorId", indicatorId))
           .Add(Restrictions.Eq("Year", timePeriod.Year))
           .Add(Restrictions.Eq("YearRange", timePeriod.YearRange))
           .Add(Restrictions.Eq("Quarter", timePeriod.Quarter))
           .Add(Restrictions.Eq("Month", timePeriod.Month))
           .Add(Restrictions.Eq("AreaCode", parentAreaCode))
           .Add(Restrictions.Eq("SexId", sexId))
           .Add(Restrictions.Eq("AgeId", ageId))
           .Add(Restrictions.Not(Restrictions.Eq("CategoryTypeId", -1)))
           .Add(Restrictions.Not(Restrictions.Eq("CategoryId", -1)))
           .AddOrder(new Order("CategoryTypeId", true))
           .AddOrder(new Order("CategoryId", true))
           .List<CoreDataSet>();
        }

        public IList<int> GetAllCategoryTypeIds(string areaCode, int indicatorId, int sexId, int ageId)
        {
            var list = new List<int>();
            string sql =
                "SELECT Distinct CategoryTypeID FROM CoreDataSet WHERE IndicatorID =" + indicatorId + " AND SexID = " + sexId + " AND " +
                "AgeID = " + ageId + " AND AreaCode = '" + areaCode + "' AND CategoryTypeID != -1";

            var q = CurrentSession.CreateSQLQuery(sql);
            var result = q.List<Int16>();

            foreach (var catTypeId in result)
            {

                list.Add(Convert.ToInt16(catTypeId));
            }

            return list;
        }

        public int GetCoreDataMaxYear(int indicatorId)
        {
            var q = CurrentSession.CreateCriteria<CoreDataSet>()
                .Add(Restrictions.Eq("IndicatorId", indicatorId))
                .SetProjection(Projections.Max("Year"));
            return q.UniqueResult<int>();
        }

        public virtual IList<CoreDataSet> GetCoreDataListForChildrenOfArea(Grouping grouping, TimePeriod period, string parentAreaCode)
        {
            if (parentAreaCode.Equals(AreaCodes.England,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return GetCoreDataForAllAreasOfType(grouping, period);
            }

            IQuery q = CurrentSession.CreateQuery(GetCoreDataListForChildrenOfAreaQueryString);
            q.SetParameter("parentAreaCode", parentAreaCode);
            SetTimePeriodParameters(q, period);
            SetGroupingParameters(q, grouping);
            SetAreaTypeIdsParameter(q, grouping);
            return q.List<CoreDataSet>();
        }

        public virtual IList<CoreDataSet> GetCoreDataListForChildrenOfCategoryArea(Grouping grouping,
            TimePeriod period, CategoryArea categoryArea)
        {
            IQuery q = CurrentSession.CreateQuery(GetCoreDataListForChildrenOfCategoryAreaQueryString);
            q.SetParameter("categoryTypeId", categoryArea.CategoryTypeId);
            q.SetParameter("categoryId", categoryArea.CategoryId);
            q.SetParameter("childAreaTypeId", grouping.AreaTypeId);
            q.SetParameter("parentAreaTypeId", categoryArea.ParentAreaTypeId);
            SetTimePeriodParameters(q, period);
            SetGroupingParameters(q, grouping);
            return q.List<CoreDataSet>();
        }

        public virtual IList<CoreDataSet> GetCoreDataListForChildrenOfNearestNeighbourArea(Grouping grouping,
            TimePeriod period, NearestNeighbourArea area)
        {
            IQuery q = CurrentSession.CreateQuery(GetCoreDataListForChildrenOfNearestNeighbourAreaQueryString);
            q.SetParameter("neighbourTypeId", area.NeighbourTypeId);
            q.SetParameter("neighbourAreaCode", area.AreaCodeOfAreaWithNeighbours);
            SetTimePeriodParameters(q, period);
            SetGroupingParameters(q, grouping);
            return q.List<CoreDataSet>();
        }

        public virtual IList<CoreDataSet> GetCoreDataListForAllCategoryAreasOfCategoryAreaType(Grouping grouping,
            TimePeriod timePeriod, int categoryTypeId, string areaCode)
        {
            return CurrentSession.CreateCriteria<CoreDataSet>()
                .Add(Restrictions.Eq("IndicatorId", grouping.IndicatorId))
                .Add(Restrictions.Eq("Year", timePeriod.Year))
                .Add(Restrictions.Eq("YearRange", timePeriod.YearRange))
                .Add(Restrictions.Eq("Quarter", timePeriod.Quarter))
                .Add(Restrictions.Eq("Month", timePeriod.Month))
                .Add(Restrictions.Eq("AreaCode", areaCode))
                .Add(Restrictions.Eq("SexId", grouping.SexId))
                .Add(Restrictions.Eq("AgeId", grouping.AgeId))
                .Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                .AddOrder(new Order("CategoryId", true))
                .List<CoreDataSet>();
        }


        public Limits GetCoreDataLimitsByIndicatorId(int indicatorId)
        {
            var limits = CurrentSession.CreateCriteria<CoreDataSet>()
                .Add(Restrictions.Eq(PropertyIndicatorId, indicatorId))
                .Add(Restrictions.IsNotNull(PropertyValue))
                .Add(Restrictions.Not(Restrictions.Eq(PropertyValue, -1.0)))
                .SetProjection(Projections.Min(PropertyValue), Projections.Max(PropertyValue))
                .UniqueResult<object[]>();

            if (limits[0] == null)
            {
                // No data rows to find limits on
                return null;
            }

            return new Limits
            {
                Min = (double)limits[0],
                Max = (double)limits[1]
            };
        }

        public Limits GetCoreDataLimitsByIndicatorIdAndAreaTypeId(int indicatorId, int areaTypeId)
        {
            const string query = "select min(d.Value),max(d.Value) from CoreDataSet d, Area a" +
             " where d.AreaCode = a.Code and d.IndicatorId = :indicatorId and a.AreaTypeId in (:areaTypeIds) and d.Value != -1 and a.IsCurrent = 1";

            IQuery q = CurrentSession.CreateQuery(query);
            q.SetParameterList("areaTypeIds", GetComponentAreaTypeIds(areaTypeId));
            q.SetParameter("indicatorId", indicatorId);
            var limits = q.UniqueResult<object[]>();

            if (limits[0] == null)
            {
                // No data rows to find limits on
                return null;
            }

            return new Limits
            {
                Min = (double)limits[0],
                Max = (double)limits[1]
            };
        }

        public Limits GetCoreDataLimitsByIndicatorIdAndAreaTypeIdAndParentAreaCode(int indicatorId, int areaTypeId, string parentAreaCode)
        {
            const string query = "select min(d.Value),max(d.Value) from CoreDataSet d, AreaHierarchy h, Area a" +
              " where d.AreaCode = h.ChildAreaCode and d.AreaCode = a.Code and h.ParentAreaCode = :parentAreaCode" +
              " and d.IndicatorId = :indicatorId and a.AreaTypeId in (:areaTypeIds) and d.Value != -1 and a.IsCurrent = 1";

            IQuery q = CurrentSession.CreateQuery(query);
            q.SetParameterList("areaTypeIds", GetComponentAreaTypeIds(areaTypeId));
            q.SetParameter("indicatorId", indicatorId);
            q.SetParameter("parentAreaCode", parentAreaCode);
            var limits = q.UniqueResult<object[]>();

            if (limits[0] == null)
            {
                // No data rows to find limits on
                return null;
            }

            return new Limits
            {
                Min = (double)limits[0],
                Max = (double)limits[1]
            };
        }

        public int GetCoreDataCountAtDataPoint(Grouping grouping)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetCoreDataSetCount");
            SetGroupingParameters(q, grouping);
            SetTimePeriodParameters(q, TimePeriod.GetDataPoint(grouping));
            SetAreaTypeIdsParameter(q, grouping);
            return q.UniqueResult<int>();
        }

        public virtual IList<CoreDataSet> GetCoreDataForAllAreasOfType(Grouping grouping, TimePeriod period)
        {
            IQuery q = CurrentSession.CreateQuery(GetCoreDataForAllAreasOfTypeQueryString);
            SetTimePeriodParameters(q, period);
            SetGroupingParameters(q, grouping);
            SetAreaTypeIdsParameter(q, grouping);
            return q.List<CoreDataSet>();
        }

        private void SetAreaTypeIdsParameter(IQuery q, Grouping grouping)
        {
            q.SetParameterList("areaTypeIds", GetComponentAreaTypeIds(grouping.AreaTypeId));
        }

        public IList<double> GetOrderedCoreDataValidValuesForAllAreasOfType(Grouping grouping, TimePeriod period,
            IEnumerable<string> ignoredAreaCodes)
        {
            //TODO replace string concaternation of HQL with code selectors 
            var queryString = GetOrderedCoreDataValidValuesForAllAreasOfTypeQueryString;

            bool areAnyIgnoredAreas = ignoredAreaCodes.Any();
            if (areAnyIgnoredAreas)
            {
                queryString += " and a.Code not in (:ignoredAreaCodes)";
            }

            queryString += " order by d.Value";

            IQuery q = CurrentSession.CreateQuery(queryString);
            SetTimePeriodParameters(q, period);
            SetGroupingParameters(q, grouping);
            q.SetParameterList("areaTypeIds", GetComponentAreaTypeIds(grouping.AreaTypeId));

            if (areAnyIgnoredAreas)
            {
                q.SetParameterList("ignoredAreaCodes", ignoredAreaCodes.ToList());
            }

            return q.List<double>();
        }

        private IList<int> GetComponentAreaTypeIds(int areaTypeId)
        {
            return new AreaTypeIdSplitter(new AreaTypeComponentRepository())
                .GetComponentAreaTypeIds(areaTypeId);
        }


        private static void SetGroupingParameters(IQuery q, Grouping grouping)
        {
            q.SetParameter("indicatorId", grouping.IndicatorId);
            q.SetParameter(ParameterSexId, grouping.SexId);
            q.SetParameter("ageId", grouping.AgeId);
        }

        private static void SetTimePeriodParameters(IQuery q, TimePeriod period)
        {
            q.SetParameter("year", period.Year);
            q.SetParameter("yearRange", period.YearRange);
            q.SetParameter("quarter", period.Quarter);
            q.SetParameter("month", period.Month);
        }

        public IList<int> GetAllIndicatorIds()
        {
            var q = CurrentSession.CreateSQLQuery("select distinct(IndicatorID) from Grouping");
            return q.List<int>();
        }

        public bool GetIndicatorMetadataAlwaysShowSexWithIndicatorName(int indicatorId)
        {
            return CurrentSession.Query<IndicatorMetadata>()
                .Where(x => x.IndicatorId == indicatorId)
                .Select(x => x.AlwaysShowSexWithIndicatorName)
                .Single<bool>();
        }

        public bool GetIndicatorMetadataAlwaysShowAgeWithIndicatorName(int indicatorId)
        {
            return CurrentSession.Query<IndicatorMetadata>()
                .Where(x => x.IndicatorId == indicatorId)
                .Select(x => x.AlwaysShowAgeWithIndicatorName)
                .Single<bool>();
        }

        public IList<IndicatorMetadata> GetIndicatorMetadata(IList<Grouping> groupings,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties, int profileId)
        {
            var indicatorIds = groupings.Select(x => x.IndicatorId).Distinct().ToList();
            return GetIndicatorMetadata(indicatorIds, indicatorMetadataTextProperties, profileId);
        }

        public IList<IndicatorMetadata> GetIndicatorMetadata(IList<Grouping> groupings,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            var indicatorIds = groupings.Select(x => x.IndicatorId).Distinct().ToList();
            return GetIndicatorMetadata(indicatorIds, indicatorMetadataTextProperties);
        }

        public IndicatorMetadata GetIndicatorMetadata(Grouping grouping,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            GroupingMetadata groupingMetadata = GetGroupingMetadata(grouping.GroupId);
            int profileId = groupingMetadata.ProfileId;

            return GetIndicatorMetadata(grouping.IndicatorId, indicatorMetadataTextProperties, profileId);
        }

        public IndicatorMetadata GetIndicatorMetadata(int indicatorId,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            IQuery q = CurrentSession.CreateQuery("from IndicatorMetadata i where i.IndicatorId = :indicatorId");
            q.SetParameter("indicatorId", indicatorId);
            IndicatorMetadata metadata = q.UniqueResult<IndicatorMetadata>();
            AddIndicatorMetadataText(indicatorMetadataTextProperties, -1, metadata);
            return metadata;
        }

        public IndicatorMetadata GetIndicatorMetadata(int indicatorId,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties,
            int profileId)
        {
            IQuery q = CurrentSession.CreateQuery("from IndicatorMetadata i where i.IndicatorId = :indicatorId");
            q.SetParameter("indicatorId", indicatorId);
            IndicatorMetadata metadata = q.UniqueResult<IndicatorMetadata>();

            AddIndicatorMetadataText(indicatorMetadataTextProperties, profileId, metadata);

            return metadata;
        }

        public IList<IndicatorMetadata> GetIndicatorMetadata(IList<int> indicatorIds,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            if (indicatorIds.Count == 0)
            {
                return new List<IndicatorMetadata>();
            }

            var allMetadata = new List<IndicatorMetadata>();

            // Query the database 1000 areas at a time, for large numbers of items the query will fail otherwise
            var splitter = new LongListSplitter<int>(indicatorIds);
            while (splitter.AnyLeft())
            {
                IQuery q = CurrentSession.CreateQuery("from IndicatorMetadata i where i.IndicatorId in (:indicatorIds)");
                q.SetParameterList("indicatorIds", splitter.NextItems());
                IList<IndicatorMetadata> metadatas = q.List<IndicatorMetadata>();

                if (metadatas.Any())
                {
                    AddIndicatorMetadataText(indicatorMetadataTextProperties, -1, metadatas.ToArray());
                    allMetadata.AddRange(metadatas);
                }
            }

            return allMetadata;
        }

        public IList<IndicatorMetadata> GetIndicatorMetadata(IList<int> indicatorIds,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties, int profileId)
        {
            if (indicatorIds.Count == 0)
            {
                return new List<IndicatorMetadata>();
            }

            var allMetadata = new List<IndicatorMetadata>();

            // Query the database 1000 areas at a time, for large numbers of items the query will fail otherwise
            var splitter = new LongListSplitter<int>(indicatorIds);
            while (splitter.AnyLeft())
            {
                IQuery q = CurrentSession.CreateQuery("from IndicatorMetadata i where i.IndicatorId in (:indicatorIds)");
                q.SetParameterList("indicatorIds", splitter.NextItems());
                IList<IndicatorMetadata> metadatas = q.List<IndicatorMetadata>();

                if (metadatas.Any())
                {
                    AddIndicatorMetadataText(indicatorMetadataTextProperties, profileId, metadatas.ToArray());
                    allMetadata.AddRange(metadatas);
                }
            }

            return allMetadata;
        }

        /// <summary>
        /// Gets the list of indicator meta data text values based on the indicator id
        /// </summary>
        /// <param name="indicatorId">The indicator id</param>
        /// <returns></returns>
        public IList<IndicatorMetadataTextValue> GetIndicatorMetadataTextValues(int indicatorId)
        {
            return CurrentSession.CreateCriteria<IndicatorMetadataTextValue>()
                .Add(Restrictions.Eq("IndicatorId", indicatorId))
                .List<IndicatorMetadataTextValue>();
        }

        public IList<IndicatorMetadata> GetGroupSpecificIndicatorMetadataTextValues(IList<Grouping> groupings,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            List<IndicatorMetadata> metadataList = new List<IndicatorMetadata>();

            if (groupings.Any())
            {
                var groupIds = groupings.Select(x => x.GroupId).Distinct().ToArray();

                // Get metadata text values
                IQuery q = CurrentSession.GetNamedQuery("GetIndicatorMetadataTextValuesByGroupIds");
                q.SetParameterList("groupIds", groupIds);
                IList overridingMetadataTextValueList = q.List();

                foreach (IList<object> overridingMetadataTextValue in overridingMetadataTextValueList)
                {
                    IndicatorMetadata metadata = new IndicatorMetadata();
                    metadata.IndicatorId = (int)overridingMetadataTextValue[0];
                    metadataList.Add(metadata);
                    metadata.Descriptive = GetTextValues(indicatorMetadataTextProperties, overridingMetadataTextValue);
                }
            }

            return metadataList;
        }

        private void AddIndicatorMetadataText(IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties, int profileId = ProfileIds.Undefined,
            params IndicatorMetadata[] metadata)
        {
            // Order by ID to be consistent with text value results
            var metadataOrdered = metadata.OrderBy(x => x.IndicatorId).ToList();

            IEnumerable<int> indicatorIds = (from m in metadataOrdered select m.IndicatorId);
            IQuery q = CurrentSession.GetNamedQuery("GetIndicatorMetadataTextValuesByIndicatorIds");
            q.SetParameterList("indicatorIds", indicatorIds.ToArray());
            IList baseTextMetadata = q.List();

            if (baseTextMetadata.Count < metadata.Count())
            {
                // A indicator is missing corresponding text properties
                List<int> foundIds = (from object result in baseTextMetadata select (int)((IList<object>)result)[0]).ToList();
                int missingId = indicatorIds.Where(x => foundIds.Contains(x) == false).First();
                throw new FingertipsException("Some indicators are missing text: " + missingId);
            }

            if (baseTextMetadata.Count > metadata.Count())
            {
                throw new FingertipsException("There are more base indicator text metadata rows than there are indicators. " +
                "Check the [IndicatorMetadataTextValues] table for an indicator id with two rows where GroupID is null.");
            }

            for (int metadataIndex = 0; metadataIndex < metadataOrdered.Count; metadataIndex++)
            {
                metadataOrdered[metadataIndex].Descriptive = GetTextValues(
                    indicatorMetadataTextProperties, (IList<object>)baseTextMetadata[metadataIndex], profileId);
            }
        }

        private static IDictionary<string, string> GetTextValues(IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties,
            IList<object> overridingMetadataTextValue, int profileId = ProfileIds.Undefined)
        {
            // Add 2 to skip IndicatorID and GroupID
            int valueIndex = 2;

            IDictionary<string, string> textValues = new Dictionary<string, string>();
            foreach (var indicatorMetadataTextProperty in indicatorMetadataTextProperties)
            {
                // Skip ID
                if (overridingMetadataTextValue[valueIndex] is int)
                {
                    valueIndex++;
                }

                var text = (string)overridingMetadataTextValue[valueIndex];
                if (string.IsNullOrEmpty(text) == false)
                {
                    textValues.Add(indicatorMetadataTextProperty.ColumnName, text.Trim());
                }
                else
                {
                    if (profileId == ProfileIds.IndicatorsForReview)
                    {
                        textValues.Add(indicatorMetadataTextProperty.ColumnName, string.Empty);
                    }
                }

                valueIndex++;
            }
            return textValues;
        }

        public IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextProperties()
        {
            return CurrentSession.CreateCriteria<IndicatorMetadataTextProperty>()
                .Add(Restrictions.Eq("IsInternalMetadata", 0))
                .List<IndicatorMetadataTextProperty>();
        }

        public IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextPropertiesIncludingInternalMetadata()
        {
            var indicatorMetadataTextProperties = GetIndicatorMetadataTextProperties();

            var internalMetadataTextProperties = CurrentSession.CreateCriteria<IndicatorMetadataTextProperty>()
                .Add(!Restrictions.Eq("IsInternalMetadata", 0))
                .List<IndicatorMetadataTextProperty>();

            foreach (var internalMetadataTextProperty in internalMetadataTextProperties)
            {
                indicatorMetadataTextProperties.Add(internalMetadataTextProperty);
            }

            return indicatorMetadataTextProperties;
        }

        //Inequalities: Category Type and Category are NOT -1 and standard: -1
        public virtual IList<CoreDataSet> GetDataIncludingInequalities(int indicatorId, TimePeriod period,
            IList<int> excludedCategoryTypeIds, params string[] areaCodes)
        {
            if (areaCodes.Length == 0)
            {
                return new List<CoreDataSet>();
            }

            var allData = new List<CoreDataSet>();

            // Query the database a reduced number of areas at a time, for large numbers of areas the query will fail otherwise
            //Inequalities: Category Type and Category are NOT -1 and standard: -1
            var splitter = new LongListSplitter<string>(areaCodes);
            while (splitter.AnyLeft())
            {
                var nextData = GetCoreDataForAreaCodesInequalities(indicatorId,
                    period, excludedCategoryTypeIds, splitter.NextItems());
                allData.AddRange(nextData);
            }
            return allData;
        }

        public IList<GroupingSubheading> GetGroupingSubheadings(int areaTypeId, int groupId)
        {
            return CurrentSession.CreateCriteria<GroupingSubheading>()
                .Add(Restrictions.Eq("AreaTypeId", areaTypeId))
                .Add(Restrictions.Eq("GroupId", groupId))
                .List<GroupingSubheading>();
        }

        private IEnumerable<CoreDataSet> GetCoreDataForAreaCodesInequalities(int indicatorId, TimePeriod period,
            IList<int> excludedCategoryTypeIds, IEnumerable<string> areaCodes)
        {
            StringBuilder sb = new StringBuilder("in (:areaCodes)");
            if (excludedCategoryTypeIds.Any())
            {
                sb.Append(" and d.CategoryTypeId not in (:excludedCategoryTypeIds)");
            }
            sb.Append(" order by d.SexId, d.AgeId, d.AreaCode, d.CategoryTypeId, d.CategoryId");

            IQuery q = CurrentSession.CreateQuery(string.Format(GetCoreDataQueryStringInequalities, sb));

            q.SetParameterList("areaCodes", areaCodes);
            if (excludedCategoryTypeIds.Any())
            {
                q.SetParameterList("excludedCategoryTypeIds", excludedCategoryTypeIds);
            }

            SetTimePeriodParameters(q, period);

            q.SetParameter("indicatorId", indicatorId);

            var nextData = q.List<CoreDataSet>();
            return nextData;
        }

    }
}
