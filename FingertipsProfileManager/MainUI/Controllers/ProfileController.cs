using Fpm.MainUI.ActionFilter;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Mappers;
using Fpm.MainUI.Models;
using Fpm.MainUI.ViewModels;
using Fpm.MainUI.ViewModels.Profile;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("profiles")]
    public class ProfileController : Controller
    {
        private readonly IProfilesReader _reader;
        private readonly IProfilesWriter _writer;

        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILookUpsRepository _lookUpsRepository;

        private int _userId;

        public ProfileController(IProfilesReader reader, IProfilesWriter writer,
            IProfileRepository profileRepository, IUserRepository userRepository,
            ILookUpsRepository lookUpsRepository)
        {
            _reader = reader;
            _writer = writer;

            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _lookUpsRepository = lookUpsRepository;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var user = UserDetails.CurrentUser();
            _userId = user.Id;
        }

        [AdminUsersOnly]
        [Route]
        public ActionResult ProfileIndex()
        {
            var model = new ProfileGridModel
            {
                SortBy = "Sequence",
                SortAscending = true,
                CurrentPageIndex = 1,
                PageSize = 200,
                ShowNewProfileButton = true
            };

            GetAllProfiles(model);

            ViewBag.UserLookup = _userRepository.GetAllFpmUsers().ToDictionary(
                x => x.Id, x => x.DisplayName);

            return View(model);
        }

        [Route("profiles-editable-by-user")]
        [HttpGet]
        public ActionResult ProfilesEditableByUser()
        {
            var model = new ProfileGridModel
            {
                SortBy = "Sequence",
                SortAscending = true,
                CurrentPageIndex = 1,
                PageSize = 200,
                ShowNewProfileButton = false,
                UserId = _userId
            };

            GetProfilesEditableByUser(model);

            var systemProfileIds = new List<int> { ProfileIds.UnassignedIndicators, ProfileIds.IndicatorsForReview };

            ViewBag.UserProfiles = model.ProfileGrid.Where(x => systemProfileIds.Contains(x.Id) == false).ToList();
            ViewBag.SystemProfiles = model.ProfileGrid.Where(x => systemProfileIds.Contains(x.Id)).ToList();

            return View("UserProfiles", model);
        }

        [HttpGet]
        [Route("create-profile")]
        public ActionResult CreateProfile()
        {
            var profileViewModel = new ProfileViewModel()
            {
                ProfileUsers = new List<ProfileUser>()
            };

            if (HttpContext.Request.UrlReferrer != null)
            {
                profileViewModel.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();
            }

            ViewBag.AllUsers = _userRepository.GetAllFpmUsers().ToProfileUserList();

            ViewBag.ContactUserIds = GetFpmUserList();

            return View("CreateProfile", profileViewModel);
        }

        [HttpPost]
        [Route("create-profile")]
        public ActionResult CreateProfile(ProfileViewModel profileViewModel)
        {
            profileViewModel.ProfileUsers = JsonConvert.DeserializeObject<IEnumerable<ProfileUser>>(Request["ProfileUsers"]);

            // Create new profile
            var profile = profileViewModel.ToProfileDetails();
            profile.SetDefaultValues(new ExtraJsFileHelper().GetExtraJsFiles());
            int newProfileId = _profileRepository.CreateProfile(profile);

            // New domain
            _writer.NewGroupingMetadata(profile.DefaultDomainName, 1, newProfileId);

            CreateDefaultContentItemsForProfile(newProfileId);

            return RedirectToAction("ProfileIndex");
        }

        [Route("edit-profile")]
        [HttpGet]
        public ActionResult EditProfile(string profileKey)
        {
            // Init view model
            var profileDetails = _reader.GetProfileDetails(profileKey);
            var profileViewModel = profileDetails.ToProfileViewModel();
            AssignExtraJsFilesToViewModel(profileDetails, profileViewModel);

            if (HttpContext.Request.UrlReferrer != null)
            {
                profileViewModel.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();
            }

            profileViewModel.SelectedPdfAreaTypes = _reader.GetAreaTypesWhichContainsPdf(profileViewModel.Id).ToProfileAreaTypeList();

            // Get all dropdowns in ViewBag
            var allUsers = _userRepository.GetAllFpmUsers().ToProfileUserList();
            profileViewModel.AllUsers = new SelectList(allUsers, "Id", "Name");

            ViewBag.SkinId = new SelectList(_lookUpsRepository.GetSkins(), "Id", "Name");
            ViewBag.FpmUsers = _userRepository.GetAllFpmUsers();

            ViewBag.SelectedContactUserIds = String.Join(",", profileViewModel.ContactUserIds);

            DefineNonAdminProfileProperties(profileViewModel);

            return View("EditProfile", profileViewModel);

        }

        [Route("edit-profile")]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "UpdateProfile")]
        public ActionResult EditProfile(ProfileViewModel profileViewModel)
        {
            var profile = GetProfileDetailsFromViewModel(profileViewModel);

            _profileRepository.UpdateProfile(profile);

            return RedirectToAction("ProfileIndex");
        }

        [Route("copy-profile")]
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "CopyProfile")]
        public ActionResult CopyProfile(ProfileViewModel profileViewModel)
        {
            // Check URL key has been changed
            var profileFromDb = _reader.GetProfileDetails(profileViewModel.UrlKey);
            if (profileFromDb != null)
            {
                ModelState.AddModelError("UrlKey", "Profile has not been copied: URL key must be unique");
                return EditProfile(profileViewModel.UrlKey);
            }

            // Change name
            if (profileViewModel.Name.ToLower().Contains("copy") == false)
            {
                profileViewModel.Name += " [COPY]";
            }

            // Copy profile
            var sourceProfileId = profileViewModel.Id;
            var profile = GetProfileDetailsFromViewModel(profileViewModel);
            var profileCopier = new ProfileDetailsCopier(_profileRepository, _writer);
            var newProfileId = profileCopier.CreateCopy(profile);
            profileCopier.CopyContentItems(sourceProfileId, newProfileId);

            return RedirectToAction("ProfileIndex");
        }

        [Route("set-data-point")]
        [HttpPost]
        public ActionResult SetDataPoint(int profileId, int areaTypeId, int indicatorId,
            int sexId, int ageId, int year, int quarter, int month, int yearRange)
        {
            if (UserDetails.CurrentUser().HasWritePermissionsToProfile(profileId) == false)
            {
                throw new FpmException("User does not have rights to change time period");
            }

            var groupIds = _reader.GetGroupingIds(profileId);
            var groupings = _writer.GetGroupings(groupIds, areaTypeId, indicatorId, sexId,
                ageId, yearRange);

            // Change data points
            foreach (var grouping in groupings)
            {
                grouping.DataPointYear = year;
                grouping.DataPointQuarter = quarter;
                grouping.DataPointMonth = month;
            }

            _writer.UpdateGroupingList(groupings);

            return Json("Saved OK");
        }

        [Route("profile/add-profile-user")]
        [HttpPost]
        public ActionResult AddProfileUser(int profileId, [Bind(Include = "FpmUserId")]EditUserViewModel viewModel)
        {
            var userId = viewModel.FpmUserId;

            // Add user permission if profile ID valid
            if (userId != 0)
            {
                if (_userRepository.GetUserGroupPermissions(profileId, userId) == null)
                {
                    UserGroupPermissions user = new UserGroupPermissions
                    {
                        UserId = userId,
                        ProfileId = profileId
                    };
                    _userRepository.SaveUserGroupPermissions(user);
                }
            }

            //Refresh the data
            var profileUsers = _reader.GetProfileUsers(profileId).ToProfileUserList();
            return PartialView("_UserPermissions", profileUsers);
        }

        [HttpPost]
        [Route("profile/remove-profile-user")]
        public ActionResult RemoveProfileUser(int profileId, [Bind(Include = "FpmUserId")]EditUserViewModel viewModel)
        {
            var userId = viewModel.FpmUserId;

            // Remove user permission if user ID valid
            if (userId != 0)
            {
                _userRepository.DeleteUserGroupPermissions(profileId, userId);
            }

            // Refresh the data
            var profileUsers = _reader.GetProfileUsers(profileId).ToProfileUserList();
            return PartialView("_UserPermissions", profileUsers);
        }

        [Route("profile/edit-profile-non-admin")]
        [HttpGet]
        public ActionResult EditProfileNonAdmin(int profileId = ProfileIds.Undefined)
        {
            var user = UserDetails.CurrentUser();

            if (profileId != ProfileIds.Undefined && user.HasWritePermissionsToProfile(profileId) == false)
            {
                profileId = ProfileIds.Undefined;
            }

            var profileIndexViewModel = new ProfileIndexViewModel
            {
                ProfileId = profileId,
                ProfileViewModel = GetProfileViewModel(profileId)
            };

            profileIndexViewModel.ProfileList = ProfileMenuHelper.GetProfileListForCurrentUser();

            ViewBag.ProfileId = profileId;

            if (profileIndexViewModel.ProfileViewModel != null)
            {
                ViewBag.SelectedContactUserIds = String.Join(",", profileIndexViewModel.ProfileViewModel.ContactUserIds);
            }
            else
            {
                ViewBag.SelectedContactUserIds = "1";
            }

            return View("EditProfileNonAdmin", profileIndexViewModel);
        }

        [Route("profile/non-admin-profile-details")]
        [HttpGet]
        public ActionResult NonAdminProfileDetails(int profileId)
        {
            var profileViewModel = GetProfileViewModel(profileId);

            ViewBag.SelectedContactUserIds = String.Join(",", profileViewModel.ContactUserIds);

            return PartialView("_NonAdminProfileDetails", profileViewModel);
        }

        [Route("profile/non-admin-profile-details")]
        [HttpPost]
        public ActionResult NonAdminProfileDetails(ProfileViewModel profileViewModel)
        {
            var profileId = profileViewModel.Id;

            CheckUserHasPermissionToEditProfile(profileId);

            var profile = _profileRepository.GetProfileDetailsById(profileId);

            profile.DefaultAreaTypeId = profileViewModel.DefaultAreaTypeId;
            profile.Name = profileViewModel.Name;
            profile.ContactUserIds = String.Join(",", profileViewModel.ContactUserIds);
            profile.DefaultFingertipsTabId = profileViewModel.DefaultFingertipsTabId;
            profile.SpineChartMinMaxLabelId = profileViewModel.SpineChartMinMaxLabelId;
            profile.StartZeroYAxis = profileViewModel.StartZeroYAxis;
            profile.ShowDataQuality = profileViewModel.ShowDataQuality;
            profile.ShowOfficialStatistic = profileViewModel.ShowOfficialStatistic;
            profile.HasTrendMarkers = profileViewModel.HasTrendMarkers;
            profile.UseTargetBenchmarkByDefault = profileViewModel.UseTargetBenchmarkByDefault;
            profile.IsChangeFromPreviousPeriodShown = profileViewModel.IsChangeFromPreviousPeriodShown;
            profile.NewDataDeploymentCount = profileViewModel.NewDataDeploymentCount;
            profile.AreIndicatorNamesDisplayedWithNumbers = profileViewModel.AreIndicatorNamesDisplayedWithNumbers;

            SetExtraJsFiles(profileViewModel, profile);
            SetFrontPageAreaSearchAreaTypes(profileViewModel, profile);

            ProfileDetailsCleaner.CleanUserInput(profile);

            _profileRepository.UpdateProfileDetail(profile);

            return Json(new { IsSaved = true }, JsonRequestBehavior.AllowGet);
        }

        [Route("all-profiles")]
        [HttpGet]
        public ActionResult GetAllProfiles()
        {
            var result = _reader.GetProfiles().OrderBy(x => x.Name).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Route("user-profiles")]
        [HttpGet]
        public ActionResult GetUserProfiles()
        {
            var profiles = UserDetails.CurrentUser().GetProfilesUserHasPermissionsTo()
                .Where(x => x.Id != ProfileIds.UnassignedIndicators)
                .OrderBy(x => x.Name)
                .ToList();

            // Only return the required properties 
            var result = profiles.Select(x => new { x.Id, x.Name });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void CreateDefaultContentItemsForProfile(int newProfileId)
        {
            var userName = UserDetails.CurrentUser().Name;
            new DefaultProfileContentWriter(_writer, newProfileId, userName)
                .CreateContentItems();
        }

        private void AssignExtraJsFilesToViewModel(ProfileDetails profileDetails,
    ProfileViewModel profileViewModel)
        {
            if (profileDetails.ExtraJsFiles != null)
            {
                AutoMapper.Mapper.Map(new ExtraJsFileHelper(profileDetails.ExtraJsFiles), profileViewModel);
            }
        }

        private ProfileDetails GetProfileDetailsFromViewModel(ProfileViewModel profileViewModel)
        {
            profileViewModel.SelectedPdfAreaTypes =
                JsonConvert.DeserializeObject<IEnumerable<ProfileAreaType>>(Request["SelectedPdfAreaTypes"]);

            profileViewModel.ProfileUsers = _reader.GetProfileUsers(profileViewModel.Id).ToProfileUserList();

            var profile = profileViewModel.ToProfileDetails();

            SetExtraJsFiles(profileViewModel, profile);
            SetFrontPageAreaSearchAreaTypes(profileViewModel, profile);

            ProfileDetailsCleaner.CleanUserInput(profile);
            return profile;
        }

        /// <summary>
        /// Workaround so null can be set as value 
        /// </summary>
        private static void SetFrontPageAreaSearchAreaTypes(ProfileViewModel profileViewModel, ProfileDetails profile)
        {
            profile.FrontPageAreaSearchAreaTypes = profileViewModel.FrontPageAreaSearchAreaTypes == FrontPageAreaSearchOptions.NoSearchText
                ? null
                : profileViewModel.FrontPageAreaSearchAreaTypes;
        }

        private static void SetExtraJsFiles(ProfileViewModel profileViewModel, ProfileDetails profile)
        {
            var extraJsFileHelper = new ExtraJsFileHelper();
            AutoMapper.Mapper.Map(profileViewModel, extraJsFileHelper);
            profile.ExtraJsFiles = extraJsFileHelper.GetExtraJsFiles();
        }

        private ProfileViewModel GetProfileViewModel(int profileId)
        {
            if (profileId == ProfileIds.Undefined) return null;
            var profileDetails = _reader.GetProfileDetailsByProfileId(profileId);
            var profileViewModel = profileDetails.ToProfileViewModel();
            AssignExtraJsFilesToViewModel(profileDetails, profileViewModel);
            DefineNonAdminProfileProperties(profileViewModel);
            return profileViewModel;
        }

        private static IEnumerable<SelectListItem> GetListOfFingertipsTabs()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem { Text = "Not Applicable", Value = "-1" },
                    new SelectListItem { Text = "Overview", Value = TabIds.TartanRug.ToString() },
                    new SelectListItem { Text = "Map", Value = TabIds.Map.ToString() },
                    new SelectListItem { Text = "Trends", Value = TabIds.Trends.ToString()},
                    new SelectListItem { Text = "Compare areas", Value = TabIds.CompareAreas.ToString() },
                    new SelectListItem { Text = "Area profiles", Value = TabIds.AreaProfile.ToString() },
                    new SelectListItem { Text = "Compare indicators", Value = TabIds.CompareAreas.ToString() },
                    new SelectListItem { Text = "Reports", Value = TabIds.Reports.ToString() }
                };
        }

        private static void CheckUserHasPermissionToEditProfile(int profileId)
        {
            var userDetails = UserDetails.CurrentUser();
            if (userDetails.HasWritePermissionsToProfile(profileId) == false)
            {
                throw new FpmException(string.Format(
                    "{0} does not have permission to edit profile {1}", userDetails.Name, profileId));
            }
        }

        private void GetAllProfiles(ProfileGridModel model)
        {
            model.ProfileGrid = _reader.GetProfiles().OrderBy(x => x.Name).ToList();
        }

        private void GetProfilesEditableByUser(ProfileGridModel model)
        {
            model.ProfileGrid = _reader.GetProfilesEditableByUser(model.UserId).OrderBy(x => x.Name).ToList();
        }

        private IEnumerable<SelectListItem> GetFpmUserList()
        {
            return new SelectList(_userRepository.GetAllFpmUsers()
                .OrderBy(x => x.DisplayName)
                .Select(x => new SelectListItem { Text = x.DisplayName, Value = x.Id.ToString() })
                .ToList(), "Value", "Text");
        }

        private void DefineNonAdminProfileProperties(ProfileViewModel viewModel)
        {
            var profileId = viewModel.Id;
            viewModel.ProfileUsers = _reader.GetProfileUsers(profileId).ToProfileUserList();

            // Define drop down options
            ViewBag.AvailableAreaTypes = _reader.GetAreaTypes(profileId).ToProfileAreaTypeList();
            ViewBag.DefaultAreaTypeId = new SelectList(_reader.GetSupportedAreaTypes(), "Id", "ShortName");
            ViewBag.DefaultFingertipsTabId = new SelectList(GetListOfFingertipsTabs(), "Value", "Text");
            ViewBag.ContactUserIds = GetFpmUserList();
            ViewBag.SpineChartMinMaxLabelId = new SelectList(_reader.GetSpineChartMinMaxLabelOptions(), "Id", "Description");
            ViewBag.FrontPageAreaSearchAreaTypes = new FrontPageAreaSearchOptions().GetOptions(
                viewModel.FrontPageAreaSearchAreaTypes);
            ViewBag.NewDataDeploymentCount = new NewDataDeploymentCount()
                .GetOptions(viewModel.NewDataDeploymentCount.ToString());

            // To maintain profile between pages
            ViewBag.ProfileId = profileId;
        }
    }
}