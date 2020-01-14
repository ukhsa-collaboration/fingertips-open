using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.ServicesWeb.Validations
{
    /// <summary>
    /// Interface for dta internal services validator
    /// </summary>
    public interface IDataServicesValidator
    {
        /// <summary>
        /// ValidateDataFileForIndicatorList
        /// </summary>
        IList<Exception> ValidateAllDataFileForIndicatorList();

        /// <summary>
        /// ValidateAllDataFileForGroup
        /// </summary>
        IList<Exception> ValidateAllDataFileForGroup();

        /// <summary>
        /// ValidateAllDataFileForProfile
        /// </summary>
        IList<Exception> ValidateAllDataFileForProfile();
    }

    /// <summary>
    /// Validator for Data internal service data
    /// </summary>
    public class DataServiceValidator : RequestedParameters, IDataServicesValidator
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        public DataServiceValidator(int childAreaTypeId, int parentAreaTypeId, string parentAreaCode, string sexIds, string ageIds, string indicatorIds = null, string areasCode = null, string categoryAreaCode = null, int? profileId = null,
            int? groupId = null, string inequalities = null) : base(childAreaTypeId, parentAreaTypeId, parentAreaCode, sexIds, ageIds, indicatorIds, areasCode, categoryAreaCode, profileId, groupId, inequalities)
        {
        }

        /// <summary>
        /// Validate All Data File For Indicator List
        /// </summary>
        /// <returns>List of exceptions errors</returns>
        public IList<Exception> ValidateAllDataFileForIndicatorList()
        {
            var exceptionsList = new List<Exception>
            {
                ValidateIndicatorIds(),
                ValidateChildAreaTypeId(),
                ValidateParentAreaTypeId(),
                ValidateProfileId(),
                ValidateParentAreaCode()
            };

            return GetListWithoutNullValues(exceptionsList);
        }

        /// <summary>
        /// Validate All Data File For Group
        /// </summary>
        /// <returns>List of exceptions errors</returns>
        public IList<Exception> ValidateAllDataFileForGroup()
        {
            var exceptionsList = new List<Exception>
            {
                ValidateChildAreaTypeId(),
                ValidateParentAreaTypeId(),
                ValidateGroupId(),
                ValidateParentAreaCode()
            };

            return GetListWithoutNullValues(exceptionsList);
        }

        /// <summary>
        /// Validate All Data File For Profile
        /// </summary>
        /// <returns>List of exceptions errors</returns>
        public IList<Exception> ValidateAllDataFileForProfile()
        {
            var exceptionsList = new List<Exception>
            {
                ValidateChildAreaTypeId(),
                ValidateParentAreaTypeId(),
                ValidateProfileId(),
                ValidateParentAreaCode()
            };

            return GetListWithoutNullValues(exceptionsList);
        }

        private static IList<Exception> GetListWithoutNullValues(IEnumerable<Exception> exceptionList)
        {
            return exceptionList.Where(x => x != null).ToList();
        }

    }
}