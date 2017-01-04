using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.PholioObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataAccess
{
    public interface IGroupDataReader
    {
        IList<int> GetIndicatorIdsByGroupIdAndAreaTypeId(int groupId, int areaTypeId);
        IList<GroupingMetadata> GetGroupMetadataList(IList<int> groupIds);
        IList<int> GetGroupingIds(int profileId);
        Grouping GetGroupingsByGroupIdAndIndicatorId(int groupId, int indicatorId);
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
            string parentAreaCode, int childAreaTypeId);

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

        IList<CoreDataSet> GetCoreDataForCategoryTypeId(Grouping grouping,
            TimePeriod timePeriod, int categoryTypeId);

        CoreDataSet GetCoreDataForCategory(Grouping grouping,
            TimePeriod timePeriod, string areaCode, int categoryTypeId, int categoryId);

        IList<CoreDataSet> GetAllCategoryDataWithinParentArea(string parentAreaCode,
            int indicatorId, int sexId, int ageId, TimePeriod timePeriod);

        IList<CoreDataSet> GetCoreDataListForChildrenOfArea(Grouping grouping, TimePeriod period, string parentAreaCode);

        IList<CoreDataSet> GetCoreDataListForChildrenOfCategoryArea(Grouping grouping,
            TimePeriod period, CategoryArea categoryArea);

        IList<CoreDataSet> GetCoreDataListForAllCategoryAreasOfCategoryAreaType(Grouping grouping,
            TimePeriod timePeriod, int categoryTypeId, string areaCode);

        Limits GetCoreDataLimitsByIndicatorId(int indicatorId);
        Limits GetCoreDataLimitsByIndicatorIdAndAreaTypeId(int indicatorId, int areaTypeId);
        Limits GetCoreDataLimitsByIndicatorIdAndAreaTypeIdAndParentAreaCode(int indicatorId, int areaTypeId, string parentAreaCode);
        int GetCoreDataCountAtDataPoint(Grouping grouping);
        IList<CoreDataSet> GetCoreDataForAllAreasOfType(Grouping grouping, TimePeriod period);

        IList<double> GetOrderedCoreDataValidValuesForAllAreasOfType(Grouping grouping, TimePeriod period,
            IEnumerable<string> ignoredAreaCodes);

        IList<IndicatorMetadata> GetIndicatorMetadata(IList<Grouping> groupings,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties);

        IndicatorMetadata GetIndicatorMetadata(Grouping grouping,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties);

        IndicatorMetadata GetIndicatorMetadata(int indicatorId,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties);

        IList<IndicatorMetadata> GetIndicatorMetadata(IList<int> indicatorIds,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties);

        IList<IndicatorMetadata> GetGroupSpecificIndicatorMetadataTextValues(IList<Grouping> groupings,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties);

        IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextProperties();

        IList<int> GetAllIndicators();

    }

    public class GroupDataReader : BaseReader, IGroupDataReader
    {
        private const string PropertyValue = "Value";
        private const string PropertyIndicatorId = "IndicatorId";
        private const string PropertyAreaTypeId = "AreaTypeId";

        private const string CategoryIdIsUndefined =
            " and d.CategoryId = -1";

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
            " and d.IndicatorId = :indicatorId" +
            TimePeriodClause + " and d.SexId = :sexId and d.AgeId = :ageId and a.IsCurrent = 1" +
            " and c.CategoryTypeId = :categoryTypeId and c.CategoryId = :categoryId" +
            " and c.ChildAreaTypeId = :childAreaTypeId and c.ParentAreaTypeId = :parentAreaTypeId";

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

        public IList<int> GetIndicatorIdsByGroupIdAndAreaTypeId(int groupId, int areaTypeId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "select g from Grouping g where g.GroupId = :groupId and g.AreaTypeId = :areaTypeId order by g.Sequence");
            q.SetParameter(ParameterGroupId, groupId);
            q.SetParameter("areaTypeId", areaTypeId);
            IList<Grouping> groupings = q.List<Grouping>();
            return (from g in groupings select g.IndicatorId).Distinct().ToList();
        }

        public virtual IList<GroupingMetadata> GetGroupMetadataList(IList<int> groupIds)
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

        public IList<int> GetGroupingIds(int profileId)
        {
            IQuery q = CurrentSession.CreateQuery("select  g.id from GroupingMetadata g where g.ProfileId = :profileId");
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
            IQuery q = CurrentSession.CreateQuery("from Grouping g where g.GroupId = :groupId and g.IndicatorId = :indicatorId order by g.DataPointYear");
            q.SetParameter(ParameterGroupId, groupId);
            q.SetParameter(ParameterIndicatorId, indicatorId);
            return q.List<Grouping>().FirstOrDefault();
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

        public virtual IList<Grouping> GetGroupingsByGroupIdsAndIndicatorIds(IList<int> groupIds, IList<int> indicatorIds)
        {
            return CurrentSession.CreateCriteria<Grouping>()
                .Add(Restrictions.In("GroupId", groupIds.ToArray()))
                .Add(Restrictions.In(PropertyIndicatorId, indicatorIds.ToArray()))
                .List<Grouping>();
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

        public IList<Grouping> GetGroupingsByGroupId(int id)
        {
            IQuery q = CurrentSession.CreateQuery("from Grouping g where g.GroupId = :groupId order by g.Sequence");
            q.SetParameter(ParameterGroupId, id);
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
            string parentAreaCode, int childAreaTypeId)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetCategoriesWithinParentArea");
            q.SetParameter("parentAreaCode", parentAreaCode);
            q.SetParameter("childAreaTypeId", childAreaTypeId);
            q.SetParameter("categoryTypeId", categoryTypeId);
            var results = q.List();

            return results.Cast<object[]>().ToDictionary(
                resultArray => (string)resultArray[0],
                resultArray => (int)resultArray[1]);
        }

        public IList<int> GetAllAgeIdsForIndicator(int indicatorId)
        {
            IQuery q = CurrentSession.CreateQuery(
                "select distinct c.AgeId from CoreDataSet c where c.IndicatorId = :indicatorId");
            q.SetParameter(ParameterIndicatorId, indicatorId);
            return q.List<int>();
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
            var splitter = new AreaCodeListSplitter(areaCodes);
            while (splitter.AnyLeft())
            {
                var nextData = GetCoreDataForAreaCodes(grouping, period, splitter.NextCodes());
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

        public virtual IList<CoreDataSet> GetCoreDataForCategoryTypeId(Grouping grouping,
            TimePeriod timePeriod, int categoryTypeId)
        {
            return CurrentSession.CreateCriteria<CoreDataSet>()
                .Add(Restrictions.Eq("IndicatorId", grouping.IndicatorId))
                .Add(Restrictions.Eq("Year", timePeriod.Year))
                .Add(Restrictions.Eq("YearRange", timePeriod.YearRange))
                .Add(Restrictions.Eq("Quarter", timePeriod.Quarter))
                .Add(Restrictions.Eq("Month", timePeriod.Month))
                .Add(Restrictions.Eq("SexId", grouping.SexId))
                .Add(Restrictions.Eq("AgeId", grouping.AgeId))
                .Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                .AddOrder(new Order("AreaCode", true))
                .AddOrder(new Order("CategoryId", true))
                .List<CoreDataSet>();
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

        public virtual IList<CoreDataSet> GetCoreDataListForChildrenOfArea(Grouping grouping, TimePeriod period, string parentAreaCode)
        {
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
            q.SetParameterList("areaTypeIds", new AreaTypeIdSplitter(areaTypeId).Ids);
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
            q.SetParameterList("areaTypeIds", new AreaTypeIdSplitter(areaTypeId).Ids);
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

        private static void SetAreaTypeIdsParameter(IQuery q, Grouping grouping)
        {
            q.SetParameterList("areaTypeIds", new AreaTypeIdSplitter(grouping.AreaTypeId).Ids);
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
            q.SetParameterList("areaTypeIds", new AreaTypeIdSplitter(new List<int> { grouping.AreaTypeId }).Ids);

            if (areAnyIgnoredAreas)
            {
                q.SetParameterList("ignoredAreaCodes", ignoredAreaCodes.ToList());
            }

            return q.List<double>();
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

        public IList<int> GetAllIndicators()
        {
            var q = CurrentSession.CreateSQLQuery("select distinct(IndicatorID) from Grouping");
            return q.List<int>();
        }

        public IList<IndicatorMetadata> GetIndicatorMetadata(IList<Grouping> groupings,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            if (groupings.Count == 0)
            {
                return new List<IndicatorMetadata>();
            }

            int[] indicatorIds = (from g in groupings select g.IndicatorId).Distinct().ToArray();

            IQuery q = CurrentSession.CreateQuery("from IndicatorMetadata i where i.IndicatorId in (:indicatorIds)");
            q.SetParameterList("indicatorIds", indicatorIds);
            IList<IndicatorMetadata> metadata = q.List<IndicatorMetadata>();
            AddIndicatorMetadataText(indicatorMetadataTextProperties, metadata.ToArray());
            return metadata;
        }

        public IndicatorMetadata GetIndicatorMetadata(Grouping grouping,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            return GetIndicatorMetadata(grouping.IndicatorId, indicatorMetadataTextProperties);
        }

        public IndicatorMetadata GetIndicatorMetadata(int indicatorId,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            IQuery q = CurrentSession.CreateQuery("from IndicatorMetadata i where i.IndicatorId = :indicatorId");
            q.SetParameter("indicatorId", indicatorId);
            IndicatorMetadata metadata = q.UniqueResult<IndicatorMetadata>();
            AddIndicatorMetadataText(indicatorMetadataTextProperties, metadata);
            return metadata;
        }

        public IList<IndicatorMetadata> GetIndicatorMetadata(IList<int> indicatorIds,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            IQuery q = CurrentSession.CreateQuery("from IndicatorMetadata i where i.IndicatorId in (:indicatorIds)");
            q.SetParameterList("indicatorIds", indicatorIds.ToList());
            IList<IndicatorMetadata> metadata = q.List<IndicatorMetadata>();
            AddIndicatorMetadataText(indicatorMetadataTextProperties, metadata.ToArray());
            return metadata;
        }

        public IList<IndicatorMetadata> GetGroupSpecificIndicatorMetadataTextValues(IList<Grouping> groupings,
            IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties)
        {
            List<IndicatorMetadata> metadataList = new List<IndicatorMetadata>();

            if (groupings.Count > 0)
            {

                List<int> ids = new List<int>();
                ids.AddRange(groupings.Select(x => x.GroupId).Distinct());

                IQuery q = CurrentSession.GetNamedQuery("GetIndicatorMetadataTextValuesByGroupIds");
                q.SetParameterList("groupIds", ids.ToArray());
                IList results = q.List();

                foreach (IList<object> result in results)
                {
                    IndicatorMetadata metadata = new IndicatorMetadata();
                    metadata.IndicatorId = (int)result[0];
                    metadataList.Add(metadata);
                    metadata.Descriptive = GetTextValues(indicatorMetadataTextProperties, result);
                }
            }

            return metadataList;
        }

        private void AddIndicatorMetadataText(IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties,
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
                    indicatorMetadataTextProperties, (IList<object>)baseTextMetadata[metadataIndex]);
            }
        }

        private static IDictionary<string, string> GetTextValues(IList<IndicatorMetadataTextProperty> indicatorMetadataTextProperties,
            IList<object> values)
        {
            IDictionary<string, string> textValues = new Dictionary<string, string>();
            for (int i = 0; i < indicatorMetadataTextProperties.Count; i++)
            {
                // Add 2 to skip IndicatorID and GroupID
                int valueIndex = i + 2;

                string text = (string)values[valueIndex];

                if (string.IsNullOrEmpty(text) == false)
                {
                    textValues.Add(indicatorMetadataTextProperties[i].ColumnName, text);
                }
            }
            return textValues;
        }

        public IList<IndicatorMetadataTextProperty> GetIndicatorMetadataTextProperties()
        {
            return CurrentSession.CreateQuery("from IndicatorMetadataTextProperty").List<IndicatorMetadataTextProperty>();
        }

    }
}
