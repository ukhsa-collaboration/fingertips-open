using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("content")]
    public class ContentController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private readonly ProfilesWriter _writer = ReaderFactory.GetProfilesWriter();

        [Route("content-index")]
        public ActionResult ContentIndex(int profileId = ProfileIds.Undefined)
        {
            var model = new ContentGridModel
            {
                SortBy = "Sequence",
                SortAscending = true,
                CurrentPageIndex = 1,
                PageSize = 100
            };

            model.ProfileList = ProfileMenuHelper.GetProfileListForCurrentUser();
            model.ProfileId = profileId;
            ViewBag.ProfileId = profileId;
            model.ContentItems = _reader.GetProfileContentItems(profileId);
            return View(model);
        }

        [Route("edit-content-item")]
        public ActionResult EditContentItem(string contentId, int profileId)
        {
            var contentItem = _reader.GetContentItem(Convert.ToInt32(contentId));
            ViewBag.ProfileId = profileId;
            ViewBag.ProfileName = _reader.GetProfileDetailsByProfileId(profileId).Name;
            return View("EditContentItem", contentItem);
        }

        [Route("create-content-item")]
        public ActionResult CreateContentItem(int selectedProfile)
        {
            var content = new ContentItem();
            content.ProfileId = selectedProfile;
            ViewBag.ProfileId = selectedProfile;
            return View(content);
        }

        [Route("insert-content-item")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult InsertContentItem(ContentItem contentItem)
        {
            if (!TryUpdateModel(contentItem))
            {
                ViewBag.UpdateError = "Update Failure";
                return View("CreateContentItem", contentItem);
            }

            var keyExist = _reader.GetContentItem(contentItem.ContentKey, contentItem.ProfileId);
            if (keyExist != null)
            {
                ViewBag.UpdateError = GetDuplicateContentKeyMessage(contentItem);
                ViewBag.ProfileId = contentItem.ProfileId;
                return View("CreateContentItem", contentItem);
            }

            var content = contentItem.IsPlainText ? formatPlainText(contentItem.Content) : contentItem.Content;
            var newContentItem = _writer.NewContentItem(contentItem.ProfileId,
                contentItem.ContentKey, contentItem.Description, contentItem.IsPlainText, content);
            AuditContentChange(newContentItem, "CREATED");

            return RedirectToAction("ContentIndex", new { profileId = contentItem.ProfileId });
        }

        [Route("content-audit-list")]
        public ActionResult GetContentAuditList(IEnumerable<int> contentIds)
        {
            var contentAuditList = _reader.GetContentAuditList(contentIds.ToList());
            return PartialView("_ContentAudit", contentAuditList);
        }

        [Route("update-content-item")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateContentItem(ContentItem contentItem)
        {
            var newContentItem = new ContentItem();
            AutoMapper.Mapper.Map(contentItem, newContentItem);

            // Convert to plain text
            if (contentItem.IsPlainText)
            {
                newContentItem.Content = formatPlainText(contentItem.Content);
            }

            var existingContent = _reader.GetContentItem(contentItem.ContentKey, contentItem.ProfileId);
            if (existingContent != null && existingContent.Id != newContentItem.Id)
            {
                ViewBag.UpdateError = GetDuplicateContentKeyMessage(contentItem);
                ViewBag.ProfileName = contentItem.ProfileId;
                ViewBag.ProfileName = _reader.GetProfileDetailsByProfileId(contentItem.ProfileId).Name;

                return View("EditContentItem", contentItem);
            }

            _writer.UpdateContentItem(newContentItem);

            AuditContentChange(newContentItem, contentItem.Content);

            return RedirectToAction("ContentIndex", new { profileId = contentItem.ProfileId });
        }

        private static string GetDuplicateContentKeyMessage(ContentItem contentItem)
        {
            return string.Format("That content key '{0}' has already been used for this profile", contentItem.ContentKey);
        }

        [Route("delete-content-item")]
        public ActionResult DeleteContentItem(int contentItemId)
        {
            var contentItem = _writer.GetContentItem(contentItemId);

            if (contentItem == null)
            {
                throw new FpmException("Content item could not be deleted with id " + contentItemId);
            }

            _writer.DeleteContentItem(contentItem.ContentKey, contentItem.ProfileId);

            // Audit content item deletion
            var oldContent = contentItem.Content;
            contentItem.Content = "DELETED";
            AuditContentChange(contentItem, oldContent);

            return Json(new
            {
                Id = contentItemId,
                Message = "The content item was successfully deleted."
            });
        }

        [HttpPost]
        [Route("publish-content-item")]
        public async Task<ActionResult> PublishContentItem(IList<int> contentIds)
        {
            // Read the live update key from the configuration
            string liveUpdateKey = AppConfig.GetLiveUpdateKey();

            // Read the api url from the configuration
            string apiUrl = AppConfig.GetLiveSiteWsUrl();
            apiUrl = apiUrl + "api/contents";

            // Get the content item(s) to be published
            IList<ContentItem> contentItems = _reader.GetContentItems(contentIds);

            // Setup the http client object and post asynchronously to the web api method
            // to publish the document on live server
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Serialise and add the live update key and content item(s) to the multi part form data content
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new StringContent(JsonConvert.SerializeObject(liveUpdateKey)), "LiveUpdateKey");

                    formData.Add(new StringContent(JsonConvert.SerializeObject(contentItems)), "content-items");

                    // Post asynchronously the request to the web api method
                    HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

                    // If the publishing of the document succeeded then return a json result
                    if (response.IsSuccessStatusCode)
                    {
                        return new JsonResult
                        {
                            Data = "Success",
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                }
            }

            throw new FpmException("Publishing live failed for the content(s)");
        }

        [Route("view-in-own-page")]
        public ActionResult ViewContentItemInOwnPage(int contentItemId)
        {
            var contentItem = _reader.GetContentItem(contentItemId);

            if (contentItem == null)
            {
                throw new FpmException("Content item could not be deleted with id " + contentItemId);
            }

            return View(contentItem);
        }

        private string formatPlainText(string content)
        {
            //Persist any line breaks if plain text
            var replacedLineBreaksContent = Regex.Replace(content, "(<br />)", "\r\n");
            var removedHTMLContent = Regex.Replace(replacedLineBreaksContent, "<.*?>", string.Empty);
            return Regex.Replace(removedHTMLContent, "(\r\n|\n)", "<br />");
        }

        private void AuditContentChange(ContentItem contentItem, string oldContent)
        {
            _writer.NewContentAudit(new ContentAudit
            {
                ContentKey = contentItem.ContentKey,
                ToContent = contentItem.Content,
                FromContent = oldContent,
                ContentId = contentItem.Id,
                UserName = UserDetails.CurrentUser().Name,
                Timestamp = DateTime.Now
            });
        }
    }
}
