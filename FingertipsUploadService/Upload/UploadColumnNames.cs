using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FingertipsUploadService.Upload
{
    /// <summary>
    /// Column names that are used in the upload template file
    /// </summary>
    public class UploadColumnNames
    {
        public static string IndicatorId = "IndicatorId".ToLower();
        public static string Year = "Year".ToLower();
        public static string YearRange = "YearRange".ToLower();
        public static string Quarter = "Quarter".ToLower();
        public static string Month = "Month".ToLower();
        public static string AgeId = "AgeId".ToLower();
        public static string SexId = "SexId".ToLower();
        public static string AreaCode = "AreaCode".ToLower();
        public static string Count = "Count".ToLower();
        public static string Value = "Value".ToLower();
        public static string LowerCI95 = "LowerCI95".ToLower();
        public static string UpperCI95 = "UpperCI95".ToLower();
        public static string LowerCI99_8 = "LowerCI99_8".ToLower();
        public static string UpperCI99_8 = "UpperCI99_8".ToLower();
        public static string Denominator = "Denominator".ToLower();
        public static string Denominator2 = "Denominator_2".ToLower();
        public static string ValueNoteId = "ValueNoteId".ToLower();
        public static string CategoryTypeId = "CategoryTypeId".ToLower();
        public static string CategoryId = "CategoryId".ToLower();

        /// <summary>
        /// Gets all the column names in the upload template file.
        /// </summary>
        public static IList<string> GetColumnNames()
        {
            return new List<string>
            {
                IndicatorId,
                Year,
                YearRange,
                Quarter,
                Month,
                AgeId,
                SexId,
                AreaCode,
                Count,
                Value,
                LowerCI95,
                UpperCI95,
                LowerCI99_8,
                UpperCI99_8,
                Denominator,
                Denominator2,
                ValueNoteId,
                CategoryTypeId,
                CategoryId
            };
        }



        public static IList<string> GetNumericColumnNames()
        {
            return GetColumnNames().Where(x => x != AreaCode).ToList();
        }

        /// <summary>
        /// Get deprecated column names and the new names they map to
        /// </summary>
        public static IDictionary<string, string> GetDeprecatedColumnNames()
        {
            return new Dictionary<string, string> {
                { "lowerci", LowerCI95},
                { "upperci", UpperCI95}
            };
        }

        public static IList<string> GetNamesOfOptionalColumns()
        {
            return new List<string>
            {
                CategoryTypeId,
                CategoryId,
                Denominator2,
                LowerCI99_8,
                UpperCI99_8
            };
        }

        /// <summary>
        /// Get all the column names of a data table
        /// </summary>
        public static IList<string> GetColumnNames(DataTable dataTable)
        {
            return dataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
        }

        public static void ChangeAllColumnNamesToLowerCase(DataTable dataTable)
        {
            foreach (var column in dataTable.Columns.Cast<DataColumn>())
            {
                column.ColumnName = column.ColumnName.ToLower();
            }
        }

        /// <summary>
        /// Replaces the names of deprecated columns with the corresponding new names
        /// </summary>
        public static void ChangeDeprecatedColumnNames(DataTable dataTable)
        {
            var deprecatedColumnNamesLookUp = GetDeprecatedColumnNames();
            var deprecatedColumnNames = deprecatedColumnNamesLookUp.Keys;

            foreach (var column in dataTable.Columns.Cast<DataColumn>())
            {
                var columnName = column.ColumnName.ToLower();
                if (deprecatedColumnNames.Contains(columnName))
                {
                    column.ColumnName = deprecatedColumnNamesLookUp[columnName];
                }
            }
        }
    }
}
