using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

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

            GetAllProfiles(model);

            if (profileId != ProfileIds.Undefined)
            {
                model.ProfileId = profileId;
                ViewBag.ProfileId = profileId;
            }

            GetContentItems(model);
            return View(model);
        }

        [Route("edit-content-item")]
        public ActionResult EditContentItem(string contentId, int profileId)
        {
            var contentItem = _reader.GetContentItem(Convert.ToInt32(contentId));
            contentItem.ProfileName = _reader.GetProfiles().FirstOrDefault(x => x.Id == profileId).Name;
            ViewBag.ProfileId = profileId;
            return View("EditContentItem", contentItem);
        }

        [Route("create-content-item")]
        public ActionResult CreateContentItem(int selectedProfile)
        {
            var content = new ContentItem ();
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
                ViewBag.updateError = "Update Failure";
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

            _writer.UpdateContentItem(newContentItem);

            AuditContentChange(newContentItem, contentItem.Content);

            return RedirectToAction("ContentIndex", new { profileId = contentItem.ProfileId });
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

        private void GetContentItems(ContentGridModel profiles)
        {
            profiles.ContentItems = _reader.GetProfileContentItems(profiles.ProfileId);
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

        private void GetAllProfiles(ContentGridModel model)
        {
            var profiles = _reader.GetProfiles().OrderBy(x => x.Name).ToList();
            var allProfiles = profiles.Select(c => new SelectListItem
            {
                Text = c.Name.ToString(CultureInfo.InvariantCulture),
                Value = c.Id.ToString(CultureInfo.InvariantCulture)
            });

            model.ProfileList = allProfiles;
            var firstOrDefault = model.ProfileList.FirstOrDefault();
            if (firstOrDefault != null)
            {
                model.ProfileId = Convert.ToInt32(firstOrDefault.Value);
            }
        }
    }
}
