using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public class PracticeDataAccess : BaseDataAccess
    {
        private const string SqlGetCcgPopulation =
@"SELECT t1.AgeID, SUM(t1.Value) AS 'Total', COUNT(*) AS 'Count'
 FROM  CoreDataSet AS t1 
 INNER JOIN L_Areas AS t2 ON t1.AreaCode = t2.AreaCode
 INNER JOIN L_AreaMapping AS t3 ON t1.AreaCode = t3.ChildLevelGeographyCode
 WHERE (t1.IndicatorID = {0}) AND (t1.SexID = {1}) AND t1.Value IS NOT NULL
 AND t3.ParentLevelGeographyCode = '{2}' AND t2.AreaTypeId = {3} AND (t1.Year = {4})
 AND t2.IsCurrent = 1 AND t1.CategoryId = -1
 GROUP BY t1.AgeID";

        private const string SqlGetPracticeAggregateDataValue =
@"SELECT SUM(t1.Value) / Count(t1.Value) AS 'Avg'
 FROM CoreDataSet AS t1 
 INNER JOIN L_Areas AS t2 ON t1.AreaCode = t2.AreaCode
 INNER JOIN L_AreaMapping AS t3 ON t1.AreaCode = t3.ChildLevelGeographyCode
 WHERE t1.IndicatorID = {0} AND t1.SexID = {1} AND t1.Value IS NOT NULL AND t3.ParentLevelGeographyCode = '{2}'
 AND t1.year = {3} AND t1.yearRange = {4} AND t1.quarter = {5} AND t1.month = {6}
 AND t2.AreaTypeId = {7} AND t1.AgeID = {8} AND t2.IsCurrent = 1 AND t1.CategoryId = -1
 GROUP BY t1.AgeID";

        private const string SqlGetPracticeAggregateDataValueForCategorisedAreas =
            @"SELECT SUM(t1.Value) / Count(t1.Value) AS 'Avg'
 FROM CoreDataSet AS t1
 INNER JOIN L_Areas AS t2 ON t1.AreaCode = t2.AreaCode
 INNER JOIN L_CategorisedAreas AS t3 ON t1.AreaCode = t3.AreaCode
 WHERE t1.IndicatorID = {0}
	AND t1.SexID = {1}
	AND t1.Value IS NOT NULL
	AND t1.Year = {2}
	AND t1.YearRange = {3}
	AND t1.Quarter = {4}
	AND t1.Month = {5}
	AND t2.AreaTypeID = {6}
	AND t2.IsCurrent = 1
	AND t3.categoryid = {7}
	AND t3.categorytypeid = {8}
	AND t1.AgeID = {9}";

        private const string SqlGetPracticeCodeToValidValueMap =
@"SELECT t1.AreaCode, t1.Value
 FROM CoreDataSet AS t1
 INNER JOIN L_Areas AS t2 ON t1.AreaCode = t2.AreaCode
 WHERE t1.IndicatorID = {0} AND t1.SexID = {1} AND t1.Value IS NOT NULL AND t1.Value <> -1
 AND t1.year = {2} AND t1.yearRange = {3} AND t1.quarter = {4} AND t1.month = {5}
 AND t2.AreaTypeId = {6} AND t2.IsCurrent = 1 AND t1.CategoryId = -1";

        private const string SqlGetPracticeCodeToBaseDataMapFromParentValueDataMap =
@"SELECT t1.AreaCode, t1.Value, t1.LowerCI95, t1.UpperCI95, t1.Count, t1.Denominator
 FROM CoreDataSet AS t1 
 INNER JOIN L_Areas AS t2 ON t1.AreaCode = t2.AreaCode
 INNER JOIN L_AreaMapping AS t3 ON t1.AreaCode = t3.ChildLevelGeographyCode
 WHERE t1.IndicatorID = {0} AND t1.SexID = {1} AND t3.ParentLevelGeographyCode = '{2}' AND t1.Value IS NOT NULL
 AND t1.year = {3} AND t1.yearRange = {4} AND t1.quarter = {5} AND t1.month = {6} 
 AND t2.AreaTypeId = {7} AND t1.AgeID = {8} AND t2.IsCurrent = 1 AND t1.CategoryId = -1";

        private const string SqlGetPracticeCodeToBaseDataMap =
@"SELECT DISTINCT t1.AreaCode, t1.Value, t1.LowerCI95, t1.UpperCI95, t1.Count, t1.Denominator
 FROM CoreDataSet AS t1 
 INNER JOIN L_Areas AS t2 ON t1.AreaCode = t2.AreaCode
 WHERE t1.IndicatorID = {0} AND t1.SexID = {1} AND t1.Value IS NOT NULL
 AND t1.year = {2} AND t1.yearRange = {3} AND t1.quarter = {4} AND t1.month = {5}
 AND t2.AreaTypeId = {6} AND t1.AgeID = {7} AND t2.IsCurrent = 1 AND t1.CategoryId = -1";

        private const string SqlGetPracticeParents = @"SELECT ParentLevelGeographyCode FROM L_AreaMapping WHERE ChildLevelGeographyCode = '{0}'";

        public const string ShapeAreaCodePrefix = "S_";

        public PracticeDataAccess()
        {
            ConnectionString = ConnectionStrings.PholioConnectionString;
        }

        public QuinaryPopulation GetCcgQuinaryPopulation(int indicatorId, TimePeriod period, string areaCode,
            int sexId)
        {
            string sql = string.Format(SqlGetCcgPopulation, indicatorId, sexId, areaCode, AreaTypeIds.GpPractice, period.Year);

            QuinaryPopulation population = new QuinaryPopulation();

            DataTable table = ExecuteSelectSqlString(sql);

            foreach (DataRow row in table.Rows)
            {
                population.Values.Add(new QuinaryPopulationValue
                {
                    AgeId = Convert.ToInt32(row.ItemArray[0]),
                    Value = (double)row.ItemArray[1]
                });

                if (population.ChildAreaCount == 0)
                {
                    population.ChildAreaCount = (int)row.ItemArray[2];
                }
            }

            return population;
        }

        /// <summary>
        /// WARNING: Do not use this for CCGs. Is used to generate Shape values.
        /// </summary>
        public virtual double GetPracticeAggregateDataValue(Grouping grouping, TimePeriod period, string areaCode)
        {
            // Age ID currently ignored
            SqlCommand cmd = new SqlCommand(string.Format(SqlGetPracticeAggregateDataValue,
                grouping.IndicatorId, grouping.SexId, areaCode, period.Year, period.YearRange, 
                period.Quarter, period.Month, AreaTypeIds.GpPractice, grouping.AgeId),
                GetConnection());

            object o = ReadObject(cmd);
            if (o == null || o is DBNull)
            {
                return ValueData.NullValue;
            }

            return (double)o;
        }

        public virtual double GetGpDeprivationDecileDataValue(Grouping grouping, TimePeriod period, CategoryArea categoryArea)
        {
            // Age ID currently ignored
            SqlCommand cmd = new SqlCommand(string.Format(SqlGetPracticeAggregateDataValueForCategorisedAreas,
                grouping.IndicatorId, grouping.SexId, period.Year, period.YearRange, period.Quarter, period.Month,
                AreaTypeIds.GpPractice, categoryArea.CategoryId, categoryArea.CategoryTypeId, grouping.AgeId),
                GetConnection());

            object o = ReadObject(cmd);
            if (o == null || o is DBNull)
            {
                return ValueData.NullValue;
            }

            return (double)o;
        }

        public Dictionary<string, float> GetPracticeCodeToValidValueMap(int indicatorId, TimePeriod period, int sexId)
        {
            string sql = string.Format(SqlGetPracticeCodeToValidValueMap,
                indicatorId, sexId, period.Year, period.YearRange, period.Quarter, period.Month, AreaTypeIds.GpPractice);

            DataTable table = ExecuteSelectSqlString(sql);

            return table.Rows.Cast<DataRow>().ToDictionary(
                row => (string)row.ItemArray[0],
                row => Convert.ToSingle((double)row.ItemArray[1])
                );
        }

        public Dictionary<string, ValueData> GetPracticeCodeToValueDataMap(Grouping grouping, TimePeriod period, string parentAreaCode)
        {
            DataTable table = ExecuteGetPracticeCodeToBaseDataMapFromParentValueDataMap(grouping, parentAreaCode, period);
            return RowsToValueData(table);
        }

        public Dictionary<string, CoreDataSet> GetPracticeCodeToBaseDataMap(Grouping grouping, TimePeriod period, string parentAreaCode)
        {
            DataTable table = ExecuteGetPracticeCodeToBaseDataMapFromParentValueDataMap(grouping, parentAreaCode, period);
            return RowsToBaseData(table);
        }

        public Dictionary<string, CoreDataSet> GetPracticeCodeToBaseDataMap(Grouping grouping, TimePeriod period)
        {
            string sql = string.Format(SqlGetPracticeCodeToBaseDataMap,
                grouping.IndicatorId, grouping.SexId, period.Year, period.YearRange, period.Quarter, period.Month,
                AreaTypeIds.GpPractice, grouping.AgeId);

            DataTable table = ExecuteSelectSqlString(sql);
            return RowsToBaseData(table);
        }

        private DataTable ExecuteGetPracticeCodeToBaseDataMapFromParentValueDataMap(Grouping grouping, string parentAreaCode, TimePeriod period)
        {
            string sql = string.Format(SqlGetPracticeCodeToBaseDataMapFromParentValueDataMap,
                                       grouping.IndicatorId, grouping.SexId, parentAreaCode,
                                       period.Year, period.YearRange, period.Quarter, period.Month,
                                       AreaTypeIds.GpPractice, grouping.AgeId);

            return ExecuteSelectSqlString(sql);
        }

        private static Dictionary<string, ValueData> RowsToValueData(DataTable table)
        {
            return table.Rows.Cast<DataRow>().ToDictionary(
                row => (string)row.ItemArray[0],
                row => new ValueData { Value = (double)row.ItemArray[1], Count = (double)row.ItemArray[4]}
                );
        }

        private static Dictionary<string, CoreDataSet> RowsToBaseData(DataTable table)
        {
            var dataMap = new Dictionary<string, CoreDataSet>();
            foreach (DataRow row in table.Rows)
            {
                var data = new CoreDataSet
                               {
                                   Value = (double)row.ItemArray[1], 
                               };

                dataMap.Add((string)row.ItemArray[0], data);

                const int indexLowerCi = 2;
                const int indexUperCi = 3;
                const int indexCount = 4;
                const int indexDenominator = 5;

                if (row.ItemArray[indexLowerCi] != DBNull.Value 
                    && row.ItemArray[indexCount] != DBNull.Value 
                    && row.ItemArray[indexDenominator] != DBNull.Value)
                {
                    data.LowerCI95 = (double)row.ItemArray[indexLowerCi];
                    data.UpperCI95 = (double)row.ItemArray[indexUperCi];
                    data.Count = (double) row.ItemArray[indexCount];
                    data.Denominator = (double)row.ItemArray[indexDenominator];
                }
                else
                {
                    data.LowerCI95 = ValueData.NullValue;
                    data.UpperCI95 = ValueData.NullValue;
                    data.Count = ValueData.NullValue;
                    data.Denominator = ValueData.NullValue;
                }
            }

            return dataMap;
        }

        public int? GetShape(string areaCode)
        {
            return GetParentNumber(ShapeAreaCodePrefix, areaCode);
        }

        private int? GetParentNumber(string prefix, string areaCode)
        {
            string sql = string.Format(SqlGetPracticeParents, areaCode);
            IList<string> areaCodes = GetStringList(ExecuteSelectSqlString(sql));

            foreach (var code in areaCodes)
            {
                if (code.StartsWith(prefix))
                {
                    return int.Parse(code.Replace(prefix, string.Empty));
                }
            }
            return null;
        }
    }
}
