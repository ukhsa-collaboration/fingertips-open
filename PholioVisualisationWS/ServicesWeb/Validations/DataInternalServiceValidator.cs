using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.ServicesWeb.Validations
{
    /// <summary>
    /// Interface for dta internal services validator
    /// </summary>
    public interface IDataInternalServicesValidator
    {
        /// <summary>
        /// ValidateLatestDataFileForGroup
        /// </summary>
        IList<Exception> ValidateLatestDataFileForGroup();
        /// <summary>
        /// ValidateLatestDataFileForIndicator
        /// </summary>
        IList<Exception> ValidateLatestDataFileForIndicator();
        /// <summary>
        /// ValidateLatestWithInequalitiesDataFileForIndicator
        /// </summary>
        IList<Exception> ValidateLatestWithInequalitiesDataFileForIndicator();
        /// <summary>
        /// ValidateAllPeriodsWithInequalitiesDataFileForIndicator
        /// </summary>
        IList<Exception> ValidateAllPeriodsWithInequalitiesDataFileForIndicator();
        /// <summary>
        /// ValidateAllPeriodDataFileByIndicator
        /// </summary>
        IList<Exception> ValidateAllPeriodDataFileByIndicator();
        /// <summary>
        /// ValidateLatestPopulationDataFile
        /// </summary>
        IList<Exception> ValidateLatestPopulationDataFile();
    }

    /// <summary>
    /// Validator for Data internal service data
    /// </summary>
    public class DataInternalServiceValidator : RequestedParameters, IDataInternalServicesValidator
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        public DataInternalServiceValidator(int childAreaTypeId, int parentAreaTypeId, string parentAreaCode, string sexId, string ageId, string indicatorIds = null, string areasCode = null, string categoryAreaCode = null, int? profileId = null,
            int? groupId = null, string inequalities = null) : base(childAreaTypeId, parentAreaTypeId, parentAreaCode, sexId, ageId, indicatorIds, areasCode, categoryAreaCode, profileId, groupId, inequalities)
        {
        }

        /// <summary>
        /// Validate Latest Data File For Group
        /// </summary>
        /// <returns>IList with the exceptions found</returns>
        public IList<Exception> ValidateLatestDataFileForGroup()
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
        /// Validate Latest Data File For Indicator
        /// </summary>
        /// <returns>IList with the exceptions found</returns>
        public IList<Exception> ValidateLatestDataFileForIndicator()
        {
            var exceptionsList = new List<Exception>
            {
                ValidateIndicatorIds(),
                ValidateChildAreaTypeId(),
                ValidateParentAreaTypeId(),
                ValidateProfileId(),
                ValidateParentAreaCode(),
                ValidateSex(),
                ValidateAge()
        };

            return GetListWithoutNullValues(exceptionsList);
        }

        /// <summary>
        /// Validate Latest With Inequalities Data File For Indicator
        /// </summary>
        /// <returns>IList with the exceptions found</returns>
        public IList<Exception> ValidateLatestWithInequalitiesDataFileForIndicator()
        {
            var exceptionsList = new List<Exception>
            {
                ValidateIndicatorIds(),
                ValidateChildAreaTypeId(),
                ValidateParentAreaTypeId(),
                ValidateInequalities(),
                ValidateProfileId(),
                ValidateParentAreaCode()
            };

            return GetListWithoutNullValues(exceptionsList);
        }

        /// <summary>
        /// Validate All Periods With Inequalities Data File For Indicator
        /// </summary>
        /// <returns>IList with the exceptions found</returns>
        public IList<Exception> ValidateAllPeriodsWithInequalitiesDataFileForIndicator()
        {
            var exceptionsList = new List<Exception>
            {
                ValidateIndicatorIds(),
                ValidateChildAreaTypeId(),
                ValidateParentAreaTypeId(),
                ValidateInequalities(),
                ValidateProfileId(),
                ValidateParentAreaCode()
            };

            return GetListWithoutNullValues(exceptionsList);
        }

        /// <summary>
        /// Validate All Period Data File By Indicator
        /// </summary>
        /// <returns>IList with the exceptions found</returns>
        public IList<Exception> ValidateAllPeriodDataFileByIndicator()
        {
            var exceptionsList = new List<Exception>
            {
                ValidateIndicatorIds(),
                ValidateChildAreaTypeId(),
                ValidateParentAreaTypeId(),
                ValidateAreaCode(),
                ValidateProfileId(),
                ValidateParentAreaCode(),
                ValidateSex(),
                ValidateAge()
            };

            return GetListWithoutNullValues(exceptionsList);
        }

        /// <summary>
        /// Validate Latest Population Data File
        /// </summary>
        /// <returns>IList with the exceptions found</returns>
        public IList<Exception> ValidateLatestPopulationDataFile()
        {
            var exceptionsList = new List<Exception>
            {
                ValidateChildAreaTypeId(),
                ValidateParentAreaTypeId(),
                ValidateAreaCode(),
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