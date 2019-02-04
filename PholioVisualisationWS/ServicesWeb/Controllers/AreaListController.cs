using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PholioVisualisation.ServicesWeb.Helpers;
using PholioVisualisation.UserData;
using PholioVisualisation.UserData.Repositories;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    /// <summary>
    /// This controller contains actions to manage the area list
    /// </summary>
    [RoutePrefix("api")]
    public class AreaListController : BaseController
    {
        private readonly IAreaListRepository _areaListRepository;
        private IPublicIdGenerator _publicIdGenerator;

        /// <summary>
        /// Constructor to initialise the area list repository and public id generator
        /// </summary>
        /// <param name="areaListRepository">The area list repository</param>
        /// <param name="publicIdGenerator">The public id generator</param>
        public AreaListController(IAreaListRepository areaListRepository, IPublicIdGenerator publicIdGenerator)
        {
            _areaListRepository = areaListRepository;
            _publicIdGenerator = publicIdGenerator;
        }

        /// <summary>
        /// Get all the area lists created by the user
        /// </summary>
        /// <param name="user_id">The user id</param>
        /// <returns>The enumerable list of area lists</returns>
        [HttpGet]
        [Route("arealists")]
        public IEnumerable<AreaList> GetAreaLists(string user_id)
        {
            try
            {
                var areaList = _areaListRepository.GetAll(user_id);
                return areaList;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get the area list based on its id
        /// </summary>
        /// <param name="area_list_id">The area list id</param>
        /// <returns>The area list</returns>
        [HttpGet]
        [Route("arealist")]
        public AreaList GetAreaList(int area_list_id)
        {
            try
            {
                var areaList = _areaListRepository.Get(area_list_id);
                return areaList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Get the area list based on the public id and user id
        /// </summary>
        /// <param name="public_id">The public id</param>
        /// <param name="user_id">The user id</param>
        /// <returns>The area list</returns>
        [HttpGet]
        [Route("arealist/by_public_id")]
        public AreaList GetAreaListByPublicId(string public_id, string user_id)
        {
            try
            {
                var areaList = _areaListRepository.GetAreaListByPublicId(public_id);

                if (areaList.UserId != user_id)
                {
                    throw new Exception(string.Format("The area list {0} ({1}) is not owned by the user {2}", areaList.ListName, areaList.Id, user_id));
                }

                return areaList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Get the list of area list area codes based on the area list id
        /// </summary>
        /// <param name="area_list_id">The area list id</param>
        /// <returns>The enumberable list of area list area codes</returns>
        [HttpGet]
        [Route("arealist/areacodes")]
        public IEnumerable<AreaListAreaCode> GetAreaListAreaCodes(int area_list_id)
        {
            try
            {
                var areaListAreaCodes = _areaListRepository.GetAreaListAreaCodes(area_list_id);
                return areaListAreaCodes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Save an area list
        /// </summary>
        /// <returns>The http response message</returns>
        [HttpPost]
        [Route("arealist/save")]
        public HttpResponseMessage SaveAreaList()
        {
            HttpResponseMessage responseMessage;

            try
            {
                var formCollection = Request.Content.ReadAsFormDataAsync().Result;

                var areaTypeId = Convert.ToInt32(formCollection["areaTypeId"]);
                var areaListName = formCollection["areaListName"];
                var userId = formCollection["userId"];
                var areaCodes = formCollection["areaCodeList"].Split(',');

                int areaListId = Save(areaTypeId, areaListName, userId, areaCodes);

                responseMessage = areaListId > 0
                    ? GetHttpResponseMessage(true, string.Empty)
                    : GetHttpResponseMessage(false, "AreaList.Save: Database insert failed.");
            }
            catch (Exception ex)
            {
                Log(ex);
                responseMessage = GetHttpResponseMessage(false, ex.Message);
            }

            return responseMessage;
        }

        /// <summary>
        /// Copy an area list
        /// </summary>
        /// <returns>The http response message</returns>
        [HttpPost]
        [Route("arealist/copy")]
        public HttpResponseMessage CopyAreaList()
        {
            HttpResponseMessage responseMessage;

            try
            {
                var formCollection = Request.Content.ReadAsFormDataAsync().Result;
                var areaListId = Convert.ToInt32(formCollection["areaListId"]);
                var areaListName = formCollection["areaListName"];
                var userId = formCollection["userId"];

                var areaListFromDb = _areaListRepository.Get(areaListId);

                if (!_areaListRepository.DoesUserOwnList(areaListFromDb.PublicId, userId))
                {
                    return GetHttpResponseMessage(false, "AreaList.Copy: User does not own the list.");
                }


                var areaListAreaCodes = _areaListRepository.GetAreaListAreaCodes(areaListId).ToList();
                List<string> areaCodes = new List<string>();
                foreach (var areaListAreaCode in areaListAreaCodes)
                {
                    areaCodes.Add(areaListAreaCode.AreaCode);
                }

                areaListId = Save(areaListFromDb.AreaTypeId, areaListName, userId, areaCodes);

                responseMessage = areaListId > 0
                    ? GetHttpResponseMessage(true, string.Empty)
                    : GetHttpResponseMessage(false, "AreaList.Copy: Database insert failed.");
            }
            catch (Exception ex)
            {
                Log(ex);
                responseMessage = GetHttpResponseMessage(false, ex.Message);
            }

            return responseMessage;
        }

        /// <summary>
        /// Update an area list
        /// </summary>
        /// <returns>The http response message</returns>
        [HttpPost]
        [Route("arealist/update")]
        public HttpResponseMessage UpdateAreaList()
        {
            HttpResponseMessage responseMessage;

            try
            {
                var formCollection = Request.Content.ReadAsFormDataAsync().Result;

                var areaListId = Convert.ToInt32(formCollection["areaListId"]);
                var areaListName = formCollection["areaListName"];
                var areaCodes = formCollection["areaCodeList"].Split(',');
                var userId = formCollection["userId"];
                var publicId = formCollection["publicId"];

                if (!_areaListRepository.DoesUserOwnList(publicId, userId))
                {
                    return GetHttpResponseMessage(false, "AreaList.Update: User does not own the list.");
                }

                _areaListRepository.Update(areaListId, areaListName, areaCodes);

                responseMessage = GetHttpResponseMessage(true, string.Empty);
            }
            catch (Exception ex)
            {
                Log(ex);
                responseMessage = GetHttpResponseMessage(false, ex.Message);
            }

            return responseMessage;
        }

        /// <summary>
        /// Delete an area list
        /// </summary>
        /// <returns>The http response message</returns>
        [HttpPost]
        [Route("arealist/delete")]
        public HttpResponseMessage DeleteAreaList()
        {
            HttpResponseMessage responseMessage;

            try
            {
                var formCollection = Request.Content.ReadAsFormDataAsync().Result;
                var areaListId = Convert.ToInt32(formCollection["areaListId"]);
                var userId = formCollection["userId"];
                var publicId = formCollection["publicId"];

                if (!_areaListRepository.DoesUserOwnList(publicId, userId))
                {
                    return GetHttpResponseMessage(false, "AreaList.Delete: User does not own the list.");
                }

                _areaListRepository.Delete(areaListId);

                responseMessage = GetHttpResponseMessage(true, string.Empty);
            }
            catch (Exception ex)
            {
                Log(ex);
                responseMessage = GetHttpResponseMessage(false, ex.Message);
            }

            return responseMessage;
        }

        private int Save(int areaTypeId, string areaListName, string userId, IList<string> areaCodes)
        {
            var publicId = _publicIdGenerator.GetAreaListPublicId();

            AreaList areaList = new AreaList
            {
                AreaTypeId = areaTypeId,
                ListName = areaListName,
                PublicId = publicId,
                UserId = userId,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            };

            return _areaListRepository.Save(areaList, areaCodes);
        }

        /// <summary>
        /// Build and return the http response message
        /// </summary>
        /// <param name="success">The boolean value stating success or failure</param>
        /// <param name="errorMessage">The error message</param>
        /// <returns>The http response message</returns>
        private HttpResponseMessage GetHttpResponseMessage(bool success, string errorMessage)
        {
            HttpResponseMessage result = Request.CreateResponse(success
                ? HttpStatusCode.OK
                : HttpStatusCode.InternalServerError, errorMessage);
            return result;
        }
    }
}
