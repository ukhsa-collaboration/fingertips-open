using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public interface ITrendDataReader
    {
        /// <summary>
        ///     Get the minimum for Value, LowerCI and UpperCI
        /// </summary>
        /// <returns>Minimum value</returns>
        double? GetMin(Grouping grouping, IEnumerable<string> areaCodes);

        /// <summary>
        ///     Get the maximum for Value, LowerCI and UpperCI
        /// </summary>
        /// <returns>Maximum value</returns>
        double? GetMax(Grouping grouping, IEnumerable<string> areaCodes);

        /// <summary>
        /// Gets all core data between the base line and data point time points of a grouping.
        /// </summary>
        /// <returns>List of CoreDataSet</returns>
        IList<CoreDataSet> GetTrendData(Grouping grouping, string areaCode);

        IDictionary<string, IList<CoreDataSet>> GetTrendDataForMultipleAreas(Grouping grouping, params string[] areaCodes);

        IList<CoreDataSet> GetTrendDataForSpecificCategory(Grouping grouping, string areaCode,
            int categoryTypeId, int categoryId);
    }

    public class TrendDataReader : BaseReader, ITrendDataReader
    {
        private const string GetLimitQuery =
            "select {0}(d.{1}) from CoreDataSet d where d.IndicatorId = :indicatorId and d.Year between :startYear and :endYear" +
            " and d.SexId = :sexId and d.AgeId = :ageId and d.AreaCode in (:areaCodes) and d.{1} != -1 ";

        private static readonly string[] DataColumns = { "Value", "LowerCI", "UpperCI" };

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="sessionFactory">The session factory</param>
        internal TrendDataReader(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        /// <summary>
        ///     Get the minimum for Value, LowerCI and UpperCI
        /// </summary>
        /// <returns>Minimum value</returns>
        public double? GetMin(Grouping grouping, IEnumerable<string> areaCodes)
        {
            return GetLimit(grouping, "min", areaCodes).Min();
        }

        /// <summary>
        ///     Get the maximum for Value, LowerCI and UpperCI
        /// </summary>
        /// <returns>Maximum value</returns>
        public double? GetMax(Grouping grouping, IEnumerable<string> areaCodes)
        {
            return GetLimit(grouping, "max", areaCodes).Max();
        }

        public IDictionary<string, IList<CoreDataSet>> GetTrendDataForMultipleAreas(Grouping grouping, params string[] areaCodes)
        {
            return GetTrendDataForSpecificCategoryForMultiplesAreas(grouping,
                CategoryTypeIds.Undefined, CategoryIds.Undefined, areaCodes);
        }

        /// <summary>
        /// Gets all core data between the base line and data point time points of a grouping.
        /// </summary>
        /// <returns>List of CoreDataSet</returns>
        public IDictionary<string, IList<CoreDataSet>> GetTrendDataForSpecificCategoryForMultiplesAreas(Grouping grouping, 
            int categoryTypeId, int categoryId, params string[] areaCodes)
        {
            var allData = new List<CoreDataSet>();

            // Query the database 1000 areas at a time, for large numbers of areas the query will fail otherwise
            var splitter = new LongListSplitter<string>(areaCodes);
            while (splitter.AnyLeft())
            {
                var criteria = CurrentSession.CreateCriteria<CoreDataSet>();
                criteria.Add(Restrictions.In("AreaCode", splitter.NextCodes().ToList()));
                AddGroupingRestrictions(grouping, criteria);
                AddTimePeriodRestriction(grouping, criteria);
                criteria.Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                    .Add(Restrictions.Eq("CategoryId", categoryId));
                criteria.AddOrder(Order.Asc("AreaCode"));

                AddTimeOrdering(criteria);

                var listFiltered = RestrictDataToTimePeriodsLimits(grouping, criteria.List<CoreDataSet>());

                allData.AddRange(listFiltered);
            }

            // Transform data list for all areas to dictionary of areacode to data list
            var d = new Dictionary<string, IList<CoreDataSet>>();
            var currentAreaCode = "";
            List<CoreDataSet> currentList = null;
            foreach (var coreDataSet in allData)
            {
                var areaCode = coreDataSet.AreaCode;
                if (areaCode != currentAreaCode)
                {
                    currentAreaCode = areaCode;
                    currentList = new List<CoreDataSet>();
                    d[currentAreaCode] = currentList;
                }

                currentList.Add(coreDataSet);
            }

            return d;
        }

        /// <summary>
        /// Gets all core data between the base line and data point time points of a grouping.
        /// </summary>
        /// <returns>List of CoreDataSet</returns>
        public IList<CoreDataSet> GetTrendData(Grouping grouping, string areaCode)
        {
            IList<CoreDataSet> data = GetTrendDataForSpecificCategory(grouping, areaCode,
                CategoryTypeIds.Undefined , CategoryIds.Undefined);
            return RestrictDataToTimePeriodsLimits(grouping, data);
        }

        public IList<CoreDataSet> GetTrendDataForSpecificCategory(Grouping grouping, string areaCode,
            int categoryTypeId, int categoryId)
        {
            var criteria = CurrentSession.CreateCriteria<CoreDataSet>();
            criteria.Add(Restrictions.Eq("AreaCode", areaCode));
            AddGroupingRestrictions(grouping, criteria);
            AddTimePeriodRestriction(grouping, criteria);
            criteria.Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                .Add(Restrictions.Eq("CategoryId", categoryId));

            AddTimeOrdering(criteria);

            return RestrictDataToTimePeriodsLimits(grouping, criteria.List<CoreDataSet>());
        }

        private static IList<CoreDataSet> RestrictDataToTimePeriodsLimits(Grouping grouping, IList<CoreDataSet> data)
        {
            return new CoreDataTimeFilter(data).Filter(
                TimePeriod.GetBaseline(grouping),
                TimePeriod.GetDataPoint(grouping)).ToList();
        }

        private IEnumerable<double?> GetLimit(Grouping grouping, string function, IEnumerable<string> areaCodes)
        {
            var results = new List<double?>();
            var areaCodesList = areaCodes.ToList();

            if (areaCodesList.Any() == false)
            {
                return results;
            }

            foreach (string column in DataColumns)
            {
                IQuery q = GetQueryLimitedByTimePeriodInterval(grouping,
                    string.Format(GetLimitQuery, function, column) + "{0}");
                q.SetParameterList("areaCodes", areaCodesList);
                AddGroupingParameters(grouping, q);
                var result = q.UniqueResult<double?>();
                if (result != null)
                {
                    results.Add(result);
                }
            }

            return results.Where(x => x.HasValue);
        }

        private void AddGroupingParameters(Grouping grouping, IQuery q)
        {
            q.SetParameter("indicatorId", grouping.IndicatorId);
            q.SetParameter("startYear", grouping.BaselineYear);
            q.SetParameter("endYear", grouping.DataPointYear);
            q.SetParameter("sexId", grouping.SexId);
            q.SetParameter("ageId", grouping.AgeId);
        }

        private IQuery GetQueryLimitedByTimePeriodInterval(Grouping grouping, string query)
        {
            IQuery q;
            if (grouping.IsDataQuarterly())
            {
                q = CurrentSession.CreateQuery(string.Format(query, " and d.Quarter > 0"));
            }
            else if (grouping.IsDataMonthly())
            {
                q = CurrentSession.CreateQuery(string.Format(query, " and d.Month > 0"));
            }
            else
            {
                q =
                    CurrentSession.CreateQuery(string.Format(query,
                        " and d.YearRange = :yearRange and d.Quarter = :quarter"));
                q.SetParameter("yearRange", grouping.YearRange);
                q.SetParameter("quarter", grouping.BaselineQuarter);
            }
            return q;
        }

        private static void AddTimeOrdering(ICriteria criteria)
        {
            criteria.AddOrder(Order.Asc("Year"));
            criteria.AddOrder(Order.Asc("Quarter"));
            criteria.AddOrder(Order.Asc("Month"));
        }

        private static void AddGroupingRestrictions(Grouping grouping, ICriteria criteria)
        {
            criteria.Add(Restrictions.Eq("IndicatorId", grouping.IndicatorId))
                .Add(Restrictions.Eq("SexId", grouping.SexId))
                .Add(Restrictions.Eq("AgeId", grouping.AgeId));
        }

        private static void AddTimePeriodRestriction(Grouping grouping, ICriteria criteria)
        {
            if (grouping.IsDataQuarterly())
            {
                criteria.Add(Restrictions.Gt("Quarter", 0));
            }
            else if (grouping.IsDataMonthly())
            {
                criteria.Add(Restrictions.Gt("Month", 0));
            }
            else
            {
                criteria.Add(Restrictions.Eq("Quarter", -1));
                criteria.Add(Restrictions.Eq("Month", -1));
                criteria.Add(Restrictions.Eq("YearRange", grouping.YearRange));
            }
        }
    }
}