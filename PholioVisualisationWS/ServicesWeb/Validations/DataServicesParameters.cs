using System;
using System.Collections.Generic;

namespace PholioVisualisation.ServicesWeb.Validations
{
    public interface IDataParameters
    {
        string[] GetCategoryCodeArray();
        IList<int> GetListIndicatorIds();
        int GetChildAreaTypeId();
        int GetParentAreaTypeId();
        string GetParentAreaCode();
        int? GetProfileId();
        int? GetGroupId();
        string GetAreasCode();
        string GetInequalities();
        IList<int> GetSexIds();
        IList<int> GetAgeIds();
    }

    public interface IDataServicesParameters : IDataParameters
    {
        DataServiceUse GetDataServiceCalled();
    }

    /// <summary>
    /// Enumerator with the services types available
    /// </summary>
    public enum DataServiceUse
    {
        /// <summary>
        /// Service to get all data from a indicator list
        /// </summary>
        AllDataFileForIndicatorList,
        /// <summary>
        /// Service to get all data from a group
        /// </summary>
        AllDataFileForGroup,
        /// <summary>
        /// Service to get all data from a profile
        /// </summary>
        AllDataFileForProfile
    };

    /// <summary>
    /// Parameters for data internal services with validations
    /// </summary>
    public class DataServicesParameters : DataServiceValidator, IDataServicesParameters
    {
        /// <summary>
        /// Endpoint call
        /// </summary>
        public DataServiceUse DataServiceCalled;

        /// <inheritdoc />
        /// <summary>
        /// Requested parameters for data internal services
        /// </summary>
        public DataServicesParameters(DataServiceUse dataServiceCalled, int childAreaTypeId, int parentAreaTypeId, string parentAreaCode, 
            string sexIds, string ageIds, string indicatorIds = null, 
            string areasCode = null, string categoryAreaCode = null,
            int? profileId = null, int? groupId = null, string inequalities = null) : 
            base(childAreaTypeId, parentAreaTypeId, parentAreaCode, sexIds, ageIds,
                indicatorIds, areasCode, categoryAreaCode, profileId, groupId, inequalities)
        {
            DataServiceCalled = dataServiceCalled;
        }

        /// <summary>
        /// Validation control for DataInternalServicesParameters
        /// </summary>
        protected sealed override void Validate()
        {
            ValidationsExceptionsFound = new List<Exception>();
            switch (DataServiceCalled)
            {
                case DataServiceUse.AllDataFileForIndicatorList:
                    ValidationsExceptionsFound = ValidateAllDataFileForIndicatorList();
                    break;
                case DataServiceUse.AllDataFileForGroup:
                    ValidationsExceptionsFound = ValidateAllDataFileForGroup();
                    break;
                case DataServiceUse.AllDataFileForProfile:
                    ValidationsExceptionsFound = ValidateAllDataFileForProfile();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("That service is not existing at the moment");
            }
        }

        public DataServiceUse GetDataServiceCalled()
        {
            return DataServiceCalled;
        }
    }
}