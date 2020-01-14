using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.Analysis.TrendMarkers
{
    public class TrendRequest
    {
        public int ValueTypeId { get; set; }
        public double UnitValue { get; set; }

        /// <summary>
        /// All data including invalid data
        /// </summary>
        public IEnumerable<CoreDataSet> Data { get; set; }

        /// <summary>
        /// Longest unbroken sequence of valid data if enough are available
        /// </summary>
        public IEnumerable<CoreDataSet> ValidData
        {
            get { return GetValidDataList(); }
        }

        public int YearRange { get; set; }

        public Grouping Grouping { get; set; }

        public ValidationResult IsValid()
        {
            var valid = ValueTypeId == ValueTypeIds.Proportion
                    || ValueTypeId == ValueTypeIds.CrudeRate;

            if (!valid)
            {
                return new ValidationResult()
                {
                    IsValid = false,
                    ValidationMessage = "The recent trend cannot be calculated for this value type"
                };
            }

            if (YearRange != 1)
            {
                return new ValidationResult()
                {
                    IsValid = false,
                    ValidationMessage = "The recent trend cannot be calculated for this year range"
                };
            }

            if (Data == null)
            {
                return new ValidationResult()
                {
                    IsValid = false,
                    ValidationMessage = "No data points found"
                };
            }

            if (Data.Count() < TrendMarkerCalculator.PointsToUse)
            {
                return new ValidationResult()
                {
                    IsValid = false,
                    ValidationMessage = "Not enough data points to calculate recent trend"
                };
            }

            // Define valid data
            if (ValidData.Count() < TrendMarkerCalculator.PointsToUse)
            {
                return new ValidationResult()
                {
                    IsValid = false,
                    ValidationMessage = "Not enough data points with valid values to calculate recent trend"
                };
            }

            // Check for possible divide by zero errors if the count and denominator are same
            if (IsPossibleDivideByZeroErrors())
            {
                return new ValidationResult()
                {
                    IsValid = false,
                    ValidationMessage = "The recent trend cannot be calculated"
                };
            }

            return new ValidationResult()
            {
                IsValid = true,
                ValidationMessage = string.Empty
            };
        }

        /// <summary>
        /// Method to check whether the count and denominator are equal.
        /// If they are equal, then SPC for proportion calculation will
        /// result in divide by zero error leading to wrong trend result.
        /// </summary>
        /// <returns>Boolean</returns>
        private bool IsPossibleDivideByZeroErrors()
        {
            foreach (var data in ValidData)
            {
                if (data.Count.Value.CompareTo(data.Denominator) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the data (starting with the most recent) until an invalid data point is reached.
        /// </summary>
        private IList<CoreDataSet> GetValidDataList()
        {
            var orderedData = new CoreDataSetSorter(Data.ToList()).SortByDescendingYear();
            var validData = new List<CoreDataSet>();
            foreach (var data in orderedData)
            {
                if (IsDataValid(data))
                {
                    validData.Add(data);
                }
                else
                {
                    // Sequence of valid data should be unbroken by missing points
                    break;
                }
            }
            return validData;
        }

        /// <summary>
        /// Only Count and denominator are required for the calculation. Check Value too because 
        /// a missing value suggests it has been suppressed.
        /// </summary>
        private static bool IsDataValid(CoreDataSet data)
        {
            return data.IsValueValid && data.IsCountValid && data.IsDenominatorValid;
        }
    }
}