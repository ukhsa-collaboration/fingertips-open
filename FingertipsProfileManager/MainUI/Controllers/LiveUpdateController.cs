using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Fpm.MainUI.ViewModels.ProfilesAndIndicators;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Newtonsoft.Json;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("live-updates")]
    public class LiveUpdateController : Controller
    {
        private readonly string _targetApiUrl = AppConfig.GetLiveSiteWsUrl();
        private readonly string _sourceApiUrl = AppConfig.GetPholioWs();
        private readonly string _liveUpdateKey = AppConfig.GetLiveUpdateKey();
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();

        [Route("profile")]
        public ActionResult ProfileLiveUpdate(int profileId = ProfileIds.Undefined)
        {
            if (profileId == ProfileIds.Undefined)
            {
                throw new FpmException("No profile ID was specified");
            }

            IndicatorMetadataProvider provider = new IndicatorMetadataProvider(_reader);

            // Get the profile name based on the profile id, to be displayed on the view
            string profileName = provider.GetProfileName(profileId);

            // Get the list of indicator meta data text values based for the profile
            IList<IndicatorMetadataTextValue> indicatorMetadataTextValues =
                provider.GetAllIndicatorsForProfile(profileId);

            ViewBag.TargetUrl = _targetApiUrl;
            ViewBag.SourceUrl = _sourceApiUrl;

            // Return the view
            return View(new ProfileIndicatorMetadataTextValues
            {
                ProfileId = profileId,
                ProfileName = profileName,
                IndicatorMetadataTextValues = indicatorMetadataTextValues
            });
        }

        [HttpPost]
        [Route("replace-metadata")]
        public async Task<ActionResult> ReplaceIndicatorMetaDataLive(int indicator_id)
        {
            string apiUrl;
            string httpGetRequestResult;

            try
            {
                // Get the api url for the http get request
                apiUrl = GetApiUrlForHttpGetRequest(LiveUpdateTypes.MetaDataTextValues, indicator_id);

                // If the api url is blank then throw an exception
                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = string.Format(
                            "Unable to form the http get request api url for replacing indicator metadata text values for the indicator id: {0}",
                            indicator_id)
                    });
                }

                // Get the http get request result
                httpGetRequestResult = await GetRequestResult(apiUrl);

                // If the http get request result is empty then throw and exception
                if (string.IsNullOrWhiteSpace(httpGetRequestResult))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = string.Format(
                            "No indicator metadata text value records found for the indicator id: {0}", indicator_id)
                    });
                }

                // Get the api url for the http post request
                apiUrl = GetApiUrlForHttpPostRequest(LiveUpdateTypes.MetaDataTextValues);

                // If the api url is blank then throw an exception
                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = string.Format(
                            "Unable to form the http post request api url for replacing indicator metadata text values for the indicator id: {0}",
                            indicator_id)
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = string.Format("{0} {1} {2}", ex.Message, ex.InnerException, ex.StackTrace)
                });
            }

            // Return the http post request result
            return await PostRequestResult(apiUrl, httpGetRequestResult, LiveUpdateTypes.MetaDataTextValues);
        }

        [HttpPost]
        [Route("replace-groupings")]
        public async Task<ActionResult> ReplaceGroupingsLive(int profile_id, int indicator_id)
        {
            string apiUrl;
            string httpGetRequestResult;

            try
            {
                // Get the api url for the http get request
                apiUrl = GetApiUrlForHttpGetRequest(LiveUpdateTypes.Groupings, indicator_id, profile_id);

                // If the api url is blank then throw an exception
                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = string.Format(
                            "Unable to form the http get request api url for replacing groupings for the profile id: {0} and indicator id: {1}",
                            profile_id, indicator_id)
                    });
                }

                // Get the http get request result
                httpGetRequestResult = await GetRequestResult(apiUrl);

                // If the http get request result is empty then throw and exception
                if (string.IsNullOrWhiteSpace(httpGetRequestResult))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = string.Format(
                            "No grouping records found for the profile id: {0} and indicator id: {1}", 
                            profile_id, indicator_id)
                    });
                }

                // Get the api url for the http post request
                apiUrl = GetApiUrlForHttpPostRequest(LiveUpdateTypes.Groupings);

                // If the api url is blank then throw an exception
                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = string.Format(
                            "Unable to form the http post request api url for replacing groupings for the profile id: {0} and indicator id: {1}",
                            profile_id, indicator_id)
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = string.Format("{0} {1} {2}", ex.Message, ex.InnerException, ex.StackTrace)
                });
            }

            // Return the http post request result
            return await PostRequestResult(apiUrl, httpGetRequestResult, LiveUpdateTypes.Groupings);
        }

        [HttpPost]
        [Route("replace-coredata")]
        public async Task<ActionResult> ReplaceIndicatorCoreDataSetLive(int indicator_id)
        {
            string apiUrl;
            string httpGetRequestResult;

            try
            {
                // Get the api url for the http get request
                apiUrl = GetApiUrlForHttpGetRequest(LiveUpdateTypes.CoreDataSet, indicator_id);

                // If the api url is blank then throw an exception
                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = string.Format(
                            "Unable to form the http get request api url for replacing core data for the indicator id: {0}",
                            indicator_id)
                    });
                }

                // Get the http get request result
                httpGetRequestResult = await GetRequestResult(apiUrl);

                // If the http get request result is empty then throw and exception
                if (string.IsNullOrWhiteSpace(httpGetRequestResult))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = string.Format("No core data set records found for the indicator id: {0}", indicator_id)
                    });
                }

                // Get the api url for the http post request
                apiUrl = GetApiUrlForHttpPostRequest(LiveUpdateTypes.CoreDataSet);

                // If the api url is blank then throw an exception
                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = string.Format(
                            "Unable to form the http post request api url for replacing core data for the indicator id: {0}",
                            indicator_id)
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = string.Format("{0} {1} {2}", ex.Message, ex.InnerException, ex.StackTrace)
                });
            }

            // Return the http post request result
            return await PostRequestResult(apiUrl, httpGetRequestResult, LiveUpdateTypes.CoreDataSet);
        }

        [HttpPost]
        [Route("delete-all-groupings-for-profile")]
        public async Task<ActionResult> DeleteAllGroupingsForProfileLive(int profile_id)
        {
            string apiUrl;
            string httpGetRequestResult;

            try
            {
                // Get the api url for the http post request
                apiUrl = GetApiUrlForHttpPostRequest(LiveUpdateTypes.DeleteAllGroupingsForProfile);

                httpGetRequestResult = profile_id.ToString();

                // If the api url is blank then throw an exception
                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    return Json(new
                    {
                        Success = false,
                        Message = string.Format(
                            "Unable to form the http post request api url for deleting the groupings for the profile id: {0}",
                            profile_id)
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = string.Format("{0} {1} {2}", ex.Message, ex.InnerException, ex.StackTrace)
                });
            }

            // Return the http post request result
            return await PostRequestResult(apiUrl, httpGetRequestResult, LiveUpdateTypes.DeleteAllGroupingsForProfile);
        }

        [HttpGet]

        private async Task<string> GetRequestResult(string apiUrl)
        {
            string result = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                    if (result == "[]")
                    {
                        result = string.Empty;
                    }
                }
            }

            return result;
        }

        private async Task<JsonResult> PostRequestResult(string apiUrl, string getRequestResult, LiveUpdateTypes liveUpdateTypes)
        {
            JsonResult result;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_targetApiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.Add(new StringContent(JsonConvert.SerializeObject(_liveUpdateKey)), "LiveUpdateKey");

                        switch (liveUpdateTypes)
                        {
                            case LiveUpdateTypes.MetaDataTextValues:
                                formData.Add(new StringContent(getRequestResult), "indicator-metadata-textvalues");
                                break;

                            case LiveUpdateTypes.Groupings:
                                formData.Add(new StringContent(getRequestResult), "groupings");
                                break;

                            case LiveUpdateTypes.CoreDataSet:
                                formData.Add(new StringContent(getRequestResult), "core-dataset");
                                break;

                            case LiveUpdateTypes.DeleteAllGroupingsForProfile:
                                formData.Add(new StringContent(JsonConvert.SerializeObject(getRequestResult)), "delete-all-groupings-for-profile");
                                break;
                        }

                        HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

                        result = response.IsSuccessStatusCode
                            ? Json(new {Success = true, Message = "OK"})
                            : Json(new {Success = false, Message = response.Content.ReadAsStringAsync().Result});
                    }
                }
            }
            catch (Exception ex)
            {
                result = Json(new
                {
                    Success = false,
                    Message = string.Format("{0} {1} {2}", ex.Message, ex.InnerException, ex.StackTrace)
                });
            }

            return result;
        }

        private string GetApiUrlForHttpGetRequest(LiveUpdateTypes liveUpdateTypes, int indicatorId, int? profileId = ProfileIds.Undefined)
        {
            string apiUrl = string.Empty;

            switch (liveUpdateTypes)
            {
                case LiveUpdateTypes.MetaDataTextValues:
                    apiUrl = string.Format("{0}api/metadata?indicator_id={1}", _sourceApiUrl, indicatorId);
                    break;

                case LiveUpdateTypes.Groupings:
                    apiUrl = string.Format("{0}api/groupings?profile_id={1}&indicator_id={2}", _sourceApiUrl, profileId, indicatorId);
                    break;

                case LiveUpdateTypes.CoreDataSet:
                    apiUrl = string.Format("{0}api/coredata?indicator_id={1}", _sourceApiUrl, indicatorId);
                    break;
            }

            return apiUrl;
        }

        private string GetApiUrlForHttpPostRequest(LiveUpdateTypes liveUpdateTypes)
        {
            string apiUrl = string.Empty;

            switch (liveUpdateTypes)
            {
                case LiveUpdateTypes.MetaDataTextValues:
                    apiUrl = string.Format("{0}api/metadata", _targetApiUrl);
                    break;

                case LiveUpdateTypes.Groupings:
                    apiUrl = string.Format("{0}api/groupings", _targetApiUrl);
                    break;

                case LiveUpdateTypes.CoreDataSet:
                    apiUrl = string.Format("{0}api/coredata", _targetApiUrl);
                    break;

                case LiveUpdateTypes.DeleteAllGroupingsForProfile:
                    apiUrl = string.Format("{0}api/delete-all-groupings-for-profile", _targetApiUrl);
                    break;
            }

            return apiUrl;
        }

        public enum LiveUpdateTypes
        {
            MetaDataTextValues = 0,
            Groupings = 1,
            CoreDataSet = 2,
            DeleteAllGroupingsForProfile = 3
        }
    }
}