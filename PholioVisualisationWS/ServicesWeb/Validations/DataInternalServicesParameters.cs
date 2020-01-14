using System;
using System.Collections.Generic;
using PholioVisualisation.ServicesWeb.Helpers;

namespace PholioVisualisation.ServicesWeb.Validations
{
    public interface IDataInternalServicesParameters : IDataParameters
    {
        IList<string> GetChildAreasCodeList();
        DataInternalServiceUse GetDataInternalServiceCalled();
    }


    /// <summary>
    /// Enumerator with the services types available
    /// </summary>
    public enum DataInternalServiceUse
    {
        /// <summary>
        /// Service to get the latest data by group id
        /// </summary>
        LatestDataFileForGroup,
        /// <summary>
        /// Service to get the latest data by indicator id
        /// </summary>
        LatestDataFileForIndicator,
        /// <summary>
        /// Service to get the latest data with inequalities filter by indicator id
        /// </summary>
        LatestWithInequalitiesDataFileForIndicator,
        /// <summary>
        /// Service to get all periods data with inequalities filter by indicator id
        /// </summary>
        AllPeriodsWithInequalitiesDataFileForIndicator,
        /// <summary>
        /// Service to get all periods data with by indicator id
        /// </summary>
        AllPeriodDataFileByIndicator,
        /// <summary>
        /// Service to get the latest population data
        /// </summary>
        LatestPopulationDataFile
    };

    /// <summary>
    /// Parameters for data internal services with validations
    /// </summary>
    public class DataInternalServicesParameters : DataInternalServiceValidator, IDataInternalServicesParameters
    {
        /// <summary>
        /// Endpoint call
        /// </summary>
        public DataInternalServiceUse DataInternalServiceCalled;

        /// <inheritdoc />
        /// <summary>
        /// Requested parameters for data internal services
        /// </summary>
        public DataInternalServicesParameters(DataInternalServiceUse dataInternalServiceCalled, int childAreaTypeId, int parentAreaTypeId, string sexId, string ageId, string parentAreaCode, string indicatorIds = null, string areasCode = null, string categoryAreaCode = null,
            int? profileId = null, int? groupId = null, string inequalities = null) : base(childAreaTypeId, parentAreaTypeId, parentAreaCode, sexId, ageId, indicatorIds, areasCode, categoryAreaCode, profileId, groupId, inequalities)
        {
            DataInternalServiceCalled = dataInternalServiceCalled;
        }

        /// <summary>
        /// Validation control for DataInternalServicesParameters
        /// </summary>
        protected sealed override void Validate()
        {
            ValidationsExceptionsFound = new List<Exception>();
            switch (DataInternalServiceCalled)
            {
                case DataInternalServiceUse.LatestDataFileForGroup:
                    ValidationsExceptionsFound = ValidateLatestDataFileForGroup();
                    break;
                case DataInternalServiceUse.LatestDataFileForIndicator:
                    ValidationsExceptionsFound = ValidateLatestDataFileForIndicator();
                    break;
                case DataInternalServiceUse.LatestWithInequalitiesDataFileForIndicator:
                    ValidationsExceptionsFound = ValidateLatestWithInequalitiesDataFileForIndicator();
                    break;
                case DataInternalServiceUse.AllPeriodsWithInequalitiesDataFileForIndicator:
                    ValidationsExceptionsFound = ValidateAllPeriodsWithInequalitiesDataFileForIndicator();
                    break;
                case DataInternalServiceUse.AllPeriodDataFileByIndicator:
                    ValidationsExceptionsFound = ValidateAllPeriodDataFileByIndicator();
                    break;
                case DataInternalServiceUse.LatestPopulationDataFile:
                    ValidationsExceptionsFound = ValidateLatestPopulationDataFile();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("That service is not existing at the moment");
            }
        }

        public IList<string> GetChildAreasCodeList()
        {
            return ServiceHelper.StringListStringParser(AreasCode);
        }

        public DataInternalServiceUse GetDataInternalServiceCalled()
        {
            return DataInternalServiceCalled;
        }
    }
}