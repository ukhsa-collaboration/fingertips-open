using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataSorting
{
    public class CoreDataSetFilter : PholioFilter
    {
        private List<CoreDataSet> data;

        /// <summary>
        /// Required for mock object creation.
        /// </summary>
        protected CoreDataSetFilter()
        {
        }

        public CoreDataSetFilter(IEnumerable<CoreDataSet> data)
        {
            this.data = data == null ?
                new List<CoreDataSet>() :
                data.ToList();
        }

        /// <summary>
        /// Returns the first CoreDataSet object that matches the specified area code.
        /// </summary>
        public CoreDataSet SelectWithAreaCode(string areaCode)
        {
            return data.FirstOrDefault(
                x => x.AreaCode.Equals(areaCode, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Keeps data that matches the specified area codes.
        /// </summary>
        public IEnumerable<CoreDataSet> SelectWithAreaCodes(IEnumerable<string> areaCodesToKeep)
        {
            if (IsAreaCodeListOk(areaCodesToKeep))
            {
                var lowerCodes = GetCodesInLowerCase(areaCodesToKeep);

                return data.Where(x => lowerCodes.Contains(x.AreaCode.ToLower()));
            }

            return data;
        }

        public virtual IEnumerable<CoreDataSet> SelectWhereCountAndDenominatorAreValid()
        {
            return data.Where(x => x.IsDenominatorValid && x.IsCountValid);
        }

        public virtual IEnumerable<CoreDataSet> SelectWhereCountIsValid()
        {
            return data.Where(x => x.IsCountValid);
        }

        public virtual IEnumerable<CoreDataSet> SelectWhereValueIsValid()
        {
            return data.Where(x => x.IsValueValid);
        }

        public virtual IEnumerable<double> SelectValidValues()
        {
            return SelectWhereValueIsValid().Select(x => x.Value);
        }

        public virtual IEnumerable<int> SelectDistinctSexIds()
        {
            return data.Select(x => x.SexId).Distinct();
        }

        public virtual IEnumerable<int> SelectDistinctAgeIds()
        {
            return data.Select(x => x.AgeId).Distinct();
        }

        /// <summary>
        /// Removes data that matches the specified area codes.
        /// </summary>
        public IEnumerable<CoreDataSet> RemoveWithAreaCode(IEnumerable<string> areaCodesToRemove)
        {
            if (IsAreaCodeListOk(areaCodesToRemove))
            {
                var lowerCodes = GetCodesInLowerCase(areaCodesToRemove);

                return data.Where(x => lowerCodes.Contains(x.AreaCode.ToLower()) == false);
            }

            return data;
        }

        /// <summary>
        /// Removes data that matches the specified age IDs.
        /// </summary>
        public IEnumerable<CoreDataSet> RemoveWithAgeId(IEnumerable<int> ageIdsToRemove)
        {
            return data.Where(x => ageIdsToRemove.Contains(x.AgeId) == false);
        }
    }
}
