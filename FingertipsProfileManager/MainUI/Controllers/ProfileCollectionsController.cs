using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.ViewModels.ProfileCollections;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Controllers
{
    [AdminUsersOnly]
    [RoutePrefix("profile-collections")]
    public class ProfileCollectionsController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private ProfileRepository _profileRepository;

        [Route("")]
        public ActionResult ProfileCollectionsIndex()
        {
            var model = new ProfileCollectionGridViewModel
            {
                SortBy = "Sequence",
                SortAscending = true,
                CurrentPageIndex = 1,
                PageSize = 200
            };

            GetAllProfileCollections(model);

            return View(model);
        }

        [Route("edit-profile-collection")]
        public ActionResult EditProfileCollection(int profileCollectionId)
        {
            //Get all profiles
            IOrderedEnumerable<ProfileDetails> allProfiles = _reader.GetProfiles().OrderBy(x => x.Name);

            ProfileCollection profileCollection = _reader.GetProfileCollection(profileCollectionId);
            profileCollection.ProfileCollectionItems = new List<ProfileCollectionItem>();

            profileCollection.Id = profileCollectionId;
            IList<ProfileCollectionItem> profileCollectionItems = _reader.GetProfileCollectionItems(profileCollectionId);

            foreach (ProfileDetails profile in allProfiles)
            {
                profileCollection.ProfileCollectionItems.Add(new ProfileCollectionItem
                {
                    ProfileId = profile.Id,
                    profileDetails = profile,
                    Selected = profileCollectionItems.Any(x => x.ProfileId == profile.Id),
                    DisplayDomains =
                        profileCollectionItems.Where(x => x.ProfileId == profile.Id)
                            .Select(x => x.DisplayDomains)
                            .FirstOrDefault()
                });
            }

            if (HttpContext.Request.UrlReferrer != null)
                profileCollection.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();

            return View("EditProfileCollection", profileCollection);
        }

        [Route("create-profile-collection")]
        public ActionResult CreateProfileCollection()
        {
            //Get all profiles
            IOrderedEnumerable<ProfileDetails> allProfiles = _reader.GetProfiles().OrderBy(x => x.Name);
            var profileCollection = new ProfileCollection { ProfileCollectionItems = new List<ProfileCollectionItem>() };

            foreach (ProfileDetails profileDetails in allProfiles)
            {
                profileCollection.ProfileCollectionItems.Add(new ProfileCollectionItem { profileDetails = profileDetails });
            }

            if (HttpContext.Request.UrlReferrer != null)
                profileCollection.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();

            return View("CreateProfileCollection", profileCollection);
        }

        [Route("update-profile-collection")]
        public ActionResult UpdateProfileCollection(int id, string assignedProfiles,
            string collectionName, string collectionSkinTitle)
        {
            _profileRepository.UpdateProfileCollection(id, assignedProfiles,
               collectionName.Trim(), collectionSkinTitle.Trim());

            return RedirectToAction("ProfileCollectionsIndex");
        }

        [Route("insert-profile-collection")]
        public ActionResult InsertProfileCollection(ProfileCollection profileCollection, string assignedProfiles)
        {
            var newProfileCollection = new ProfileCollection
            {
                CollectionName = profileCollection.CollectionName,
                CollectionSkinTitle = profileCollection.CollectionSkinTitle
            };

            _profileRepository.CreateProfileCollection(newProfileCollection, assignedProfiles);

            return RedirectToAction("ProfileCollectionsIndex");
        }

        private void GetAllProfileCollections(ProfileCollectionGridViewModel viewModel)
        {
            viewModel.ProfileCollectionGrid = _reader.GetProfileCollections().OrderBy(x => x.CollectionName).ToList();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _profileRepository = new ProfileRepository(NHibernateSessionFactory.GetSession());

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _profileRepository.Dispose();

            base.OnActionExecuted(filterContext);
        }
    }
}