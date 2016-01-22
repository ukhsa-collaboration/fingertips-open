using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Controllers
{
    public class ContentController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private readonly ProfilesWriter _writer = ReaderFactory.GetProfilesWriter();

        public ActionResult ContentIndex()
        {
            var model = new ContentGridModel
            {
                SortBy = "Sequence",
                SortAscending = true,
                CurrentPageIndex = 1,
                PageSize = 100
            };

            GetAllProfiles(model);

            var profileId = Request.Params["profileId"];
            if (profileId != null)
            {
                model.ProfileId = int.Parse(profileId);
            }

            GetContentItems(model);
            return View(model);
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

        private void GetContentItems(ContentGridModel profiles)
        {
            profiles.ContentItems = _reader.GetProfileContentItems(profiles.ProfileId);
        }

        public ActionResult EditContent(string contentId, int profileId)
        {
            var contentItem = _reader.GetContentItem(Convert.ToInt32(contentId));
            contentItem.ProfileName = _reader.GetProfiles().FirstOrDefault(x => x.Id == profileId).Name;

            if (HttpContext.Request.UrlReferrer != null)
            {
                contentItem.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();
            }

            return View("EditContent", contentItem);
        }

        public ActionResult CreateContent(string selectedProfile)
        {
            var model = new ContentGridModel();

            GetAllProfiles(model);

            model.ProfileId = Convert.ToInt32(selectedProfile);

            var content = new ContentItem { ProfileList = model.ProfileList };
            if (HttpContext.Request.UrlReferrer != null)
            {
                content.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();
            }

            return View("CreateContent", content);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult InsertContent(ContentItem fc, string description, string selectedProfile,
            string content, string contentKey, bool plainTextContent)
        {
            if (!TryUpdateModel(fc))
            {
                ViewBag.updateError = "Update Failure";
                return View("CreateContent", fc);
            }

            var contentItem = _writer.NewContentItem(Convert.ToInt32(selectedProfile), contentKey, description, plainTextContent, plainTextContent ? formatPlainText(content) : content);
            AuditContentChange(contentItem, "CREATED");

            return RedirectToAction("ContentIndex", new { profileId = selectedProfile });
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

        public ActionResult GetContentAuditList(IEnumerable<int> contentIds)
        {
            var contentAuditList = _reader.GetContentAuditList(contentIds.ToList());
            return PartialView("_ContentAudit", contentAuditList);
        }

        private string formatPlainText(string content)
        {
            //Persist any line breaks if plain text
            var replacedLineBreaksContent = Regex.Replace(content, "(<br />)", "\r\n");
            var removedHTMLContent = Regex.Replace(replacedLineBreaksContent, "<.*?>", string.Empty);
            return Regex.Replace(removedHTMLContent, "(\r\n|\n)", "<br />");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateContent(string description, string content, int id, string contentKey,
            string returnUrl, string oldContent, bool plainTextContent)
        {
            var contentItem = new ContentItem();
            contentItem.Id = id;
            contentItem.Description = description;
            contentItem.ContentKey = contentKey;
            contentItem.PlainTextContent = plainTextContent;
            contentItem.Content = plainTextContent ? formatPlainText(content) : content;

            _writer.UpdateContentItem(contentItem);

            AuditContentChange(contentItem, oldContent);

            return Redirect(returnUrl);
        }

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

        public ActionResult ReloadContentItems(string selectedProfile)
        {
            var model = new ContentGridModel();

            GetAllProfiles(model);

            model.ProfileId = Convert.ToInt32(selectedProfile);
            GetContentItems(model);

            return View("ContentIndex", model);
        }
    }
}
