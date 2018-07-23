using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Helpers;

namespace PholioVisualisation.ServicesWeb.Controllers
{
    /// <summary>
    /// Services related to content
    /// </summary>
    [RoutePrefix("api")]
    public class ContentController : BaseController
    {
        private readonly IContentItemRepository _contentItemRepository;
        private readonly IRequestContentParserHelper _parserHelper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contentItemRepository">Content item repository</param>
        /// <param name="parserHelper">Request content parser helper</param>
        public ContentController(IContentItemRepository contentItemRepository, IRequestContentParserHelper parserHelper)
        {
            _contentItemRepository = contentItemRepository;
            _parserHelper = parserHelper;
        }
        
        /// <summary>
        /// Gets a content string associated with a specific profile
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="key">Content key unique within the profile</param>
        [HttpGet]
        [Route("content")]
        public string GetContent(int profile_id, string key)
        {
            try
            {
                ContentItem contentItem = _contentItemRepository.GetContentForProfile(profile_id, key);
                return contentItem != null ? contentItem.Content : string.Empty;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }


        /// <summary>
        /// Publish content items
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("contents")]
        public async Task<HttpResponseMessage> PublishContentItems()
        {
            bool success = false;
            string errorMessage = string.Empty;

            try
            {
                // Parse content items
                IList<ContentItem> contentItems = await _parserHelper.ParseContentItems(Request);

                _contentItemRepository.SaveContentItems(contentItems);

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
            return HttpHelper.GetHttpResponseMessage(Request, success, errorMessage);
        }
    }
}
