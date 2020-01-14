using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Helpers;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    /// <summary>
    /// Update indicators data on live
    /// </summary>
    [System.Web.Http.RoutePrefix("api")]
    public class PholioController : BaseController
    {
        private readonly IProfileReader _profileReader;
        private readonly IGroupDataReader _groupDataReader;
        private readonly ICoreDataSetValidator _coreDataSetValidator;
        private readonly IIndicatorMetadataRepository _indicatorMetadataRepository;
        private readonly IRequestContentParserHelper _parserHelper;

        /// <summary>
        /// Constructor to initialise the profile reader and group data reader
        /// </summary>
        /// <param name="profileReader">The profile reader</param>
        /// <param name="groupDataReader">The group data reader</param>
        /// <param name="coreDataSetValidator">The core data set validator</param>
        /// /// <param name="indicatorMetadataRepository">The indicator meta data repository</param>
        /// <param name="parserHelper">The request content parser helper</param>
        public PholioController(IProfileReader profileReader, IGroupDataReader groupDataReader,
            ICoreDataSetValidator coreDataSetValidator, IIndicatorMetadataRepository indicatorMetadataRepository,
            IRequestContentParserHelper parserHelper)
        {
            _profileReader = profileReader;
            _coreDataSetValidator = coreDataSetValidator;
            _groupDataReader = groupDataReader;
            _indicatorMetadataRepository = indicatorMetadataRepository;
            _parserHelper = parserHelper;
        }

        /// <summary>
        /// Get the list of meta data text values for an indicator
        /// </summary>
        /// <param name="indicator_id">The indicator id</param>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("metadata")]
        public IList<IndicatorMetadataTextValue> GetMetadata(int indicator_id)
        {
            IList<IndicatorMetadataTextValue> indicatorMetadataTextValues = new List<IndicatorMetadataTextValue>();

            try
            {
                indicatorMetadataTextValues = _groupDataReader.GetIndicatorMetadataTextValues(indicator_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }

            return indicatorMetadataTextValues;
        }

        /// <summary>
        /// Replace indicator metadata text values on live
        /// </summary>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("metadata")]
        public async Task<HttpResponseMessage> ReplaceIndicatorMetaDataTextValues()
        {
            bool success = false;
            string errorMessage = string.Empty;

            try
            {
                // Parse meta data
                var indicatorMetadataTextValues = await _parserHelper.ParseMetadata(Request);

                // Remove metadata text for profiles that do not exist
                var profileIds = _profileReader.GetAllProfileIds();
                indicatorMetadataTextValues = indicatorMetadataTextValues
                    .Where(x => x.ProfileId == null || profileIds.Contains(x.ProfileId.Value))
                    .ToList();

                // Check indicator ID exists
                var indicatorId = indicatorMetadataTextValues.First().IndicatorId;
                CheckIndicatorIdExists(indicatorId);

                // Create an instance of indicator metadata text value repository
                FpmIndicatorMetadataTextValueRepository repository =
                    new FpmIndicatorMetadataTextValueRepository();
                repository.ReplaceIndicatorMetadataTextValues(indicatorMetadataTextValues);

                // Set the success to true
                success = true;
            }
            catch (Exception ex)
            {
                // Error identified, set the success to false and log the error
                Log(ex);
                errorMessage = string.Format("{0} {1} {2}", ex.Message, ex.InnerException, ex.StackTrace);
            }

            // Return the http response message
            return GetHttpResponseMessage(success, errorMessage);
        }

        /// <summary>
        /// Get the list of groupings for an indicator
        /// </summary>
        /// <param name="profile_id">The profile id</param>
        /// <param name="indicator_id">The indicator id</param>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("groupings")]
        public IList<Grouping> GetGroupings(int profile_id, int indicator_id)
        {
            IList<Grouping> groupings = new List<Grouping>();

            try
            {
                IList<int> groupIds = _profileReader.GetGroupIdsForProfile(profile_id);

                groupings = _groupDataReader.GetGroupingsByGroupIdsAndIndicatorId(groupIds.ToList(), indicator_id);
            }
            catch (Exception ex)
            {
                Log(ex);
            }

            return groupings;
        }

        /// <summary>
        /// Replace groupings
        /// </summary>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("groupings")]
        public async Task<HttpResponseMessage> ReplaceGroupings()
        {
            bool success;
            string errorMessage;

            try
            {
                // Parse groupings
                var groupings = await _parserHelper.ParseGroupings(Request);

                // Check indicator exists
                var indicatorId = groupings.First().IndicatorId;
                CheckIndicatorIdExists(indicatorId);

                // Validate groupings
                GroupingValidator validator = new GroupingValidator(groupings);
                validator.ValidateAges();
                validator.ValidateAreaTypes();     
                errorMessage = validator.GetErrorMessage();
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    throw new FingertipsException(errorMessage);
                }

                // Create an instance of fpm grouping repository
                FpmGroupingRepository repository = new FpmGroupingRepository();
                repository.ReplaceGroupings(groupings);

                // Set the success to true
                success = true;
            }
            catch (Exception ex)
            {
                // Error identified, set the success to false and log the error
                success = false;
                Log(ex);
                errorMessage = string.Format("{0} {1} {2}", ex.Message, ex.InnerException, ex.StackTrace);
            }

            // Return the http response message
            return GetHttpResponseMessage(success, errorMessage);
        }

        /// <summary>
        /// Get the list of core data sets for an indicator
        /// </summary>
        /// <param name="indicator_id">The indicator id</param>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("coredata")]
        public IList<CoreDataSet> GetCoreDataSets(int indicator_id)
        {
            IList<CoreDataSet> coreDataSets = new List<CoreDataSet>();

            try
            {
                coreDataSets = _groupDataReader.GetCoreDataForIndicatorId(indicator_id);
            }
            catch (Exception ex)
            {
                Log(ex);
            }

            return coreDataSets;
        }

        /// <summary>
        /// Replace core data set for an indicator
        /// </summary>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("coredata")]
        public HttpResponseMessage ReplaceCoreData()
        {
            bool success;
            string errorMessage;

            try
            {
                // Parse core data sets
                var coreDataSets = _parserHelper.ParseCoreDataSets(Request);

                // Check indicator exists
                var indicatorId = coreDataSets.First().IndicatorId;
                CheckIndicatorIdExists(indicatorId);

                // Validate core data
                _coreDataSetValidator.Validate(coreDataSets);
                errorMessage = _coreDataSetValidator.GetErrorMessage();
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    throw new FingertipsException(errorMessage);
                }

                // Replace core data sets for indicator
                var repository = new CoreDataSetRepository();
                repository.ReplaceCoreDataSetForAnIndicator(coreDataSets);

                // Set the success to true
                success = true;
            }
            catch (Exception ex)
            {
                // Error identified, set the success to false and log the error
                success = false;
                Log(ex);
                errorMessage = string.Format("{0} {1} {2}", ex.Message, ex.InnerException, ex.StackTrace);
            }

            // Set and return the http response message
            return GetHttpResponseMessage(success, errorMessage);
        }

        /// <summary>
        /// Delete all groupings for a profile
        /// </summary>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("delete-all-groupings-for-profile")]
        public async Task<HttpResponseMessage> DeleteAllGroupingsForProfile()
        {
            bool success;
            string errorMessage = string.Empty;

            try
            {
                // Parse profile
                var profileId = await _parserHelper.ParseProfileId(Request);

                // Create an instance of fpm grouping repository
                FpmGroupingRepository repository = new FpmGroupingRepository();

                // Delete all groupings for the profile
                repository.DeleteAllGroupingsForProfile(profileId);

                // Set the success to true
                success = true;
            }
            catch (Exception ex)
            {
                // Error identified, set the success to false and log the error
                success = false;
                Log(ex);
                errorMessage = string.Format("{0} {1} {2}", ex.Message, ex.InnerException, ex.StackTrace);
            }

            // Return the http response message
            return GetHttpResponseMessage(success, errorMessage);
        }

        private HttpResponseMessage GetHttpResponseMessage(bool success, string errorMessage)
        {
            HttpResponseMessage result = Request.CreateResponse(success
                ? HttpStatusCode.OK
                : HttpStatusCode.InternalServerError, errorMessage);
            return result;
        }

        private void CheckIndicatorIdExists(int indicatorId)
        {
            if (_indicatorMetadataRepository.DoesIndicatorIdExist(indicatorId) == false)
            {
                throw new FingertipsException("Indicator is not defined: " + indicatorId);
            }
        }
    }
}
