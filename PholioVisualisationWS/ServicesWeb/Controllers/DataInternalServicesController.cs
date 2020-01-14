using PholioVisualisation.PholioObjects;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PholioVisualisation.ServicesWeb.Managers;
using PholioVisualisation.ServicesWeb.Validations;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    /// <summary>
    /// Endpoints related exporting CSV data from visualizations and tables
    /// </summary>
    [RoutePrefix("api")]
    public class DataInternalServicesController : DataBaseController
    {
        /// <summary>
        /// Simple service for testing whether the API is available
        /// </summary>
        [HttpGet]
        [Route("test")]
        public string Test()
        {
            return "Success";
        }


        /// <summary>
        /// Get the most recent data for all the indicators without inequalities in a profile group in CSV format
        /// </summary>
        /// <remarks>This service returns data in CSV not JSON format so the response will not be viewable on this page</remarks>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="group_id">Profile group ID</param>
        /// <param name="area_codes">Area code list selected</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        /// <param name="category_area_code">Ignore this parameter</param>
        [HttpGet]
        [Route("latest_data_without_inequalities/csv/by_group_id")]
        public HttpResponseMessage GetLatestDataFileForGroup(int child_area_type_id, int parent_area_type_id, int group_id, string area_codes, 
            string parent_area_code = AreaCodes.England, string category_area_code = null)
        {
            var receivedParameters = new DataInternalServicesParameters(DataInternalServiceUse.LatestDataFileForGroup, child_area_type_id, parent_area_type_id,
                "", "", parent_area_code, null, area_codes, category_area_code, null, group_id);

            if (!receivedParameters.IsValid())
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = receivedParameters.GetExceptionStringContentMessages()
                };
            }

            try
            {
                var dataInternalServiceManager = new DataInternalServicesManager(receivedParameters, GroupDataReader, ProfileReader, AreasReader);
                var exportParameters = dataInternalServiceManager.ExportParameters;
                var onDemandParameters = dataInternalServiceManager.OnDemandParameters;

                return GetOnDemandIndicatorDataResponse(AreasReader, exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Get the most recent data for specific indicators without inequalities in CSV format
        /// </summary>
        /// <remarks>This service returns data in CSV not JSON format so the response will not be viewable on this page</remarks>
        /// <param name="indicator_ids">Comma separated list of indicator IDs [Maximum 20]</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="sex_ids">Comma separated list of sex IDs</param>
        /// <param name="age_ids">Comma separated list of age IDs</param>
        /// <param name="area_codes">Area code list selected [Maximum 80]</param>
        /// <param name="profile_id">Profile ID [optional]</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        /// <param name="category_area_code">Category area code [optional]</param>
        [HttpGet]
        [Route("latest_data_without_inequalities/csv/by_indicator_id")]
        public HttpResponseMessage GetLatestDataFileForIndicator(string indicator_ids, int child_area_type_id, int parent_area_type_id, string sex_ids, 
            string age_ids, string area_codes = null, int profile_id = ProfileIds.Undefined, string parent_area_code = AreaCodes.England, 
            string category_area_code = null)
        {
            var receivedParameters = new DataInternalServicesParameters(DataInternalServiceUse.LatestDataFileForIndicator, child_area_type_id,
                parent_area_type_id, sex_ids, age_ids, parent_area_code, indicator_ids, area_codes, category_area_code,
                profile_id);

            if (!receivedParameters.IsValid())
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = receivedParameters.GetExceptionStringContentMessages()
                };
            }

            try
            {
                var dataInternalServiceManager = new DataInternalServicesManager(receivedParameters, GroupDataReader, ProfileReader, AreasReader);
                var exportParameters = dataInternalServiceManager.ExportParameters;
                var onDemandParameters = dataInternalServiceManager.OnDemandParameters;

                return GetOnDemandIndicatorDataResponse(AreasReader, exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Get the most recent data with inequalities by indicator in CSV format
        /// </summary>
        /// <remarks>This service returns data in CSV not JSON format so the response will not be viewable on this page</remarks>
        /// <param name="indicator_ids">Comma separated list of indicator IDs [Maximum 20]</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="area_codes">Area code list selected [Maximum 80]</param>
        /// <param name="inequalities">Inequalities to be filtered by</param>
        /// <param name="profile_id">Profile ID [optional]</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        /// <param name="category_area_code">Category area code [optional]</param>
        [HttpGet]
        [Route("latest_data_with_inequalities/csv/by_indicator_id")]
        public HttpResponseMessage GetLatestWithInequalitiesDataFileForIndicator(string indicator_ids, int child_area_type_id, 
            int parent_area_type_id, string inequalities, string area_codes = null, int profile_id = ProfileIds.Undefined,
            string parent_area_code = AreaCodes.England, string category_area_code = null)
        {
            var receivedParameters = new DataInternalServicesParameters(DataInternalServiceUse.LatestWithInequalitiesDataFileForIndicator, child_area_type_id, 
                parent_area_type_id, "", "", parent_area_code, indicator_ids, area_codes, category_area_code,
                profile_id, null, inequalities);

            if (!receivedParameters.IsValid())
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = receivedParameters.GetExceptionStringContentMessages()
                };
            }

            try
            {
                var dataInternalServiceManager = new DataInternalServicesManager(receivedParameters, GroupDataReader, ProfileReader, AreasReader);
                var exportParameters = dataInternalServiceManager.ExportParameters;
                var onDemandParameters = dataInternalServiceManager.OnDemandParameters;

                return GetOnDemandIndicatorDataResponse(AreasReader, exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Get data for all time periods with inequalities by indicator in CSV format
        /// </summary>
        /// <remarks>This service returns data in CSV not JSON format so the response will not be viewable on this page</remarks>
        /// <param name="indicator_ids">Comma separated list of indicator IDs [Maximum 20]</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="area_codes">Area code list selected [Maximum 80]</param>
        /// <param name="inequalities">Inequalities to be filtered by</param>
        /// <param name="profile_id">Profile ID [optional]</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        /// <param name="category_area_code">Category area code [optional]</param>
        [HttpGet]
        [Route("all_data_with_inequalities/csv/by_indicator_id")]
        public HttpResponseMessage GetAllPeriodsWithInequalitiesDataFileForIndicator(string indicator_ids, int child_area_type_id, 
            int parent_area_type_id, string inequalities, string area_codes = null, int profile_id = ProfileIds.Undefined, 
            string parent_area_code = AreaCodes.England, string category_area_code = null)
        {
            var receivedParameters = new DataInternalServicesParameters(DataInternalServiceUse.AllPeriodsWithInequalitiesDataFileForIndicator, 
                child_area_type_id, parent_area_type_id, "", "", parent_area_code, indicator_ids, area_codes,
                category_area_code, profile_id, null, inequalities);

            if (!receivedParameters.IsValid())
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = receivedParameters.GetExceptionStringContentMessages()
                };
            }

            try
            {
                var dataInternalServiceManager = new DataInternalServicesManager(receivedParameters, GroupDataReader, ProfileReader, AreasReader);
                var exportParameters = dataInternalServiceManager.ExportParameters;
                var onDemandParameters = dataInternalServiceManager.OnDemandParameters;

                return GetOnDemandIndicatorDataResponse(AreasReader, exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An internal error occurred, please contact the administrator")
                };
                return responseMessage;
            }
        }

        /// <summary>
        /// Get data for all time periods for specific indicators without inequalities in CSV format
        /// </summary>
        /// <remarks>This service returns data in CSV not JSON format so the response will not be viewable on this page</remarks>
        /// <param name="indicator_ids">Comma separated list of indicator IDs [Maximum 20]</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="sex_ids">Comma separated list of sex IDs</param>
        /// <param name="age_ids">Comma separated list of age IDs</param>
        /// <param name="area_codes">Area code list selected [Maximum 80]</param>
        /// <param name="profile_id">Profile ID [optional]</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        /// <param name="category_area_code">Ignore this parameter</param>
        [HttpGet]
        [Route("all_data_without_inequalities/csv/by_indicator_id")]
        public HttpResponseMessage GetAllPeriodDataFileByIndicator(string indicator_ids, int child_area_type_id, int parent_area_type_id, 
            string sex_ids, string age_ids, string area_codes, int profile_id = ProfileIds.Undefined,
            string parent_area_code = AreaCodes.England, string category_area_code = null)
        {
            var receivedParameters = new DataInternalServicesParameters(DataInternalServiceUse.AllPeriodDataFileByIndicator,
                child_area_type_id, parent_area_type_id, sex_ids, age_ids, parent_area_code, indicator_ids, area_codes,
                category_area_code, profile_id);

            if (!receivedParameters.IsValid())
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = receivedParameters.GetExceptionStringContentMessages()
                };
            }

            try
            {
                var dataInternalServiceManager = new DataInternalServicesManager(receivedParameters, GroupDataReader, ProfileReader, AreasReader);
                var exportParameters = dataInternalServiceManager.ExportParameters;
                var onDemandParameters = dataInternalServiceManager.OnDemandParameters;

                return GetOnDemandIndicatorDataResponse(AreasReader, exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        /// <summary>
        /// Get the most recent population data in CSV format
        /// </summary>
        /// <remarks>This service returns data in CSV not JSON format so the response will not be viewable on this page</remarks>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="area_codes">Area code list selected</param>
        /// <param name="parent_area_code">The parent area code [default is England]</param>
        /// <param name="category_area_code">Category area code [optional]</param>
        [HttpGet]
        [Route("latest/population/csv")]
        public HttpResponseMessage GetLatestPopulationDataFile(int child_area_type_id, int parent_area_type_id, string area_codes, 
            string parent_area_code = AreaCodes.England, string category_area_code = null)
        {
            const int groupId = GroupIds.Population;

            var receivedParameters = new DataInternalServicesParameters(DataInternalServiceUse.LatestPopulationDataFile, 
                child_area_type_id, parent_area_type_id, "", "", parent_area_code, null, area_codes, category_area_code, null, groupId);

            if (!receivedParameters.IsValid())
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = receivedParameters.GetExceptionStringContentMessages()
                };
            }

            try
            {
                var dataInternalServiceManager = new DataInternalServicesManager(receivedParameters, GroupDataReader, ProfileReader, AreasReader);
                var exportParameters = dataInternalServiceManager.ExportParameters;
                var onDemandParameters = dataInternalServiceManager.OnDemandParameters;
                onDemandParameters.AreQuinaryPopulations = true;

                return GetOnDemandIndicatorDataResponse(AreasReader, exportParameters, onDemandParameters);
            }
            catch (Exception ex)
            {
                Log(ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}