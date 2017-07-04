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
    [RoutePrefix("documents")]
    public class DocumentsController : Controller
    {
        public const int MaxFileSizeInBytes = 50000000/*50MB*/;

        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private readonly ProfilesWriter _writer = ReaderFactory.GetProfilesWriter();

        [Route("")]
        public ActionResult DocumentsIndex(int profileId = ProfileIds.Undefined)
        {
            var user = UserDetails.CurrentUser();

            if (profileId != ProfileIds.Undefined && user.HasWritePermissionsToProfile(profileId) == false)
            {
                profileId = ProfileIds.Undefined;
            }

            var viewModel = GetViewModel(profileId);

            viewModel.ProfileList = new ProfileMenuHelper()
                .GetProfilesUserHasPermissionToExcludingSpecialProfiles(user);
            ViewBag.DocumentItems = _reader.GetDocumentsWithoutFileData(profileId);
            ViewBag.ProfileId = profileId;
            return View(viewModel);
        }

        [HttpPost]
        [Route("upload")]
        public ActionResult Upload(int uploadProfileId)
        {
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["fileToBeUploaded"];
                if (file != null)
                {
                    byte[] uploadedFile = new byte[file.InputStream.Length];
                    file.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                    if (uploadedFile.Length > MaxFileSizeInBytes)
                    {
                        throw new FpmException("Max file upload size is 50 MB");
                    }

                    var fileName = Path.GetFileName(file.FileName);

                    // check is filename unique.
                    var docs = _reader.GetDocumentsWithoutFileData(fileName);
                    if (docs.Count > 0)
                    {
                        // now check is this name used with current profile.
                        var docFromDatabase = docs.FirstOrDefault(x => x.ProfileId == uploadProfileId);
                        if (docFromDatabase != null)
                        {
                            docFromDatabase.ProfileId = uploadProfileId;
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
                            ProfileId = uploadProfileId,
                            FileName = fileName,
                            FileData = uploadedFile,
                            UploadedBy = new CurrentUser().Name,
                            UploadedOn = DateTime.Now
                        };
                        _writer.NewDocument(doc);
                    }
                }
            }

            return RedirectToAction("DocumentsIndex", new { profileId = uploadProfileId });
        }

        [Route("is_filename_unique")]
        public ActionResult IsFileNameUnique(string filename, int profileId)
        {
            var isUnique = new FileNameHelper(_reader).IsUnique(filename, profileId);

            return new JsonResult
            {
                Data = isUnique,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        [Route("delete")]
        public ActionResult Delete(int id)
        {
            Document doc = _reader.GetDocumentWithoutFileData(id);

            if (doc == null)
            {
                throw new FpmException("Content item could not be deleted with id " + id);
            }

            if (UserDetails.CurrentUser().HasWritePermissionsToProfile(doc.ProfileId))
            {
                _writer.DeleteDocument(doc);

                return new JsonResult
                {
                    Data = "Success",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            throw new FpmException("User does not have the right to delete document " + id);
        }

        private static DocumentsGridModel GetViewModel(int profileId)
        {
            var viewModel = new DocumentsGridModel
            {
                SortBy = "Sequence",
                SortAscending = true,
                CurrentPageIndex = 1,
                PageSize = 100,
                ProfileId = profileId
            };
            return viewModel;
        }
    }
}