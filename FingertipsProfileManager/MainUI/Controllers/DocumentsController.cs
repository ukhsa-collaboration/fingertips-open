using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private readonly ProfilesWriter _writer = ReaderFactory.GetProfilesWriter();

        public ActionResult Index()
        {
            var profileId = Request.Params["selectedProfile"];
            var model = new DocumentsGridModel
            {
                SortBy = "Sequence",
                SortAscending = true,
                CurrentPageIndex = 1,
                PageSize = 100
            };

            var user = UserDetails.CurrentUser();
            var userProfiles = user.GetProfilesUserHasPermissionsTo().ToList();

            AssignProfileList(model, userProfiles);
            AssignProfileId(model, profileId, userProfiles);

            GetDocumentItems(model);
            return View(model);
        }

        private static void AssignProfileId(DocumentsGridModel model, string profileId, IList<ProfileDetails> userProfiles)
        {
            // If request params don't have any selected profile, use the first profile to populate the model
            model.ProfileId = profileId == null
                ? userProfiles[0].Id
                : int.Parse(profileId);
        }

        [HttpPost]
        public ActionResult Upload(string selectedProfileId)
        {
            if (Request != null)
            {
                int profileId = -1;
                if (!string.IsNullOrEmpty(selectedProfileId))
                {
                    profileId = Convert.ToInt32(selectedProfileId);
                }
                var maxFileSizeInBytes = 10000000/*10MB*/;
                HttpPostedFileBase file = Request.Files["fileToBeUploaded"];
                if (file != null)
                {
                    byte[] uploadedFile = new byte[file.InputStream.Length];
                    file.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                    if (uploadedFile.Length > maxFileSizeInBytes)
                    {
                        throw new FpmException("Max file upload size is 10 MB");
                    }

                    var fileName = Path.GetFileName(file.FileName);

                    // check is filename unique.
                    var docs = _reader.GetDocuments(fileName);
                    if (docs.Count > 0)
                    {
                        // now check is this name used with current profile.
                        var docFromDatabase = docs.FirstOrDefault(x => x.ProfileId == profileId);
                        if (docFromDatabase != null)
                        {
                            docFromDatabase.ProfileId = profileId;
                            docFromDatabase.FileName = fileName;                            
                            docFromDatabase.FileData = uploadedFile;
                            docFromDatabase.UploadedBy = new CurrentUser().Name;
                            docFromDatabase.UploadedOn = DateTime.Now;

                            _writer.UpdateDocument(docFromDatabase);
                        }
                    }
                    else
                    {
                        // else overwrite current file for selected profile.
                        var doc = new Document
                        {
                            ProfileId = profileId,
                            FileName = fileName,
                            FileData = uploadedFile,
                            UploadedBy = new CurrentUser().Name,
                            UploadedOn = DateTime.Now
                        };
                        _writer.NewDocument(doc);
                    }
                }
            }

            return RedirectToAction("Index", new { selectedProfile = selectedProfileId });
        }

        public ActionResult IsFileNameUnique(string filename, string selectedProfileId)
        {            
            if (string.IsNullOrEmpty(selectedProfileId))
            {
                return new HttpStatusCodeResult(400, "bad request");
            }
            
            var profileId = Convert.ToInt32(selectedProfileId);
            var fileNameHelper = new FileNameHelper();
            
            return new JsonResult
            {
                Data = fileNameHelper.IsUnique(filename, profileId),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private void AssignProfileList(DocumentsGridModel model, IList<ProfileDetails> profilesDetails)
        {
            IEnumerable<SelectListItem> userProfilesForModel = profilesDetails.Select(c => new SelectListItem
            {
                Text = c.Name.ToString(CultureInfo.InvariantCulture),
                Value = c.Id.ToString(CultureInfo.InvariantCulture)
            });

            model.ProfileList = userProfilesForModel;
        }

        private void GetDocumentItems(DocumentsGridModel model)
        {
            model.DocumentItems = _reader.GetDocuments(model.ProfileId);
        }
    }
}