using Fpm.MainUI.ActionFilter;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.MainUI.ViewModels;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    [AdminUsersOnly]
    [RoutePrefix("user")]
    public class UserController : Controller
    {
        private readonly IProfilesReader _reader;
        private readonly IUserRepository _userRepository;

        public UserController(IProfilesReader reader, IUserRepository userRepository)
        {
            _reader = reader;
            _userRepository = userRepository;
        }

        [Route("user-index")]
        public ActionResult UserIndex()
        {
            var model = new UserGridModel();

            var allUsers = _userRepository.GetAllFpmUsers().OrderBy(x => x.UserName);
            model.UserGrid = allUsers;

            return View(model);
        }

        [HttpGet]
        [Route("user-edit")]
        public ActionResult EditUser(int userId)
        {
            EditUserViewModel viewModel = new EditUserViewModel();

            // FPM user properties
            var userDetails = UserDetails.NewUserFromUserId(userId);
            var fpmUser = userDetails.FpmUser;
            viewModel.UserName = fpmUser.UserName;
            viewModel.DisplayName = fpmUser.DisplayName;
            viewModel.FpmUserId = fpmUser.Id;
            viewModel.IsAdministrator = fpmUser.IsAdministrator;
            viewModel.IsReviewer = fpmUser.IsReviewer;
            viewModel.IsCurrent = fpmUser.IsCurrent;
            viewModel.IsMemberOfFpmSecurityGroup = userDetails.IsMemberOfFpmSecurityGroup;

            ViewBag.ProfilesUserHasPermissionTo = userDetails.GetProfilesUserHasPermissionsTo();

            // Profile drop down
            viewModel.ProfileList = new ProfileMenuHelper().GetAllProfiles(_reader);

            return View("EditUser", viewModel);
        }

        [Route("user-create")]
        public ActionResult CreateUser()
        {
            var user = new FpmUser();
            return View("CreateUser", user);
        }

        [HttpPost]
        [Route("user-insert")]
        [ValidateInput(false)]
        public ActionResult InsertUser(FpmUser user)
        {
            if (!TryUpdateModel(user))
            {
                ViewBag.UpdateError = "Update Failure";
                return View("CreateUser", user);
            }

            user.IsCurrent = true;
            _userRepository.CreateUser(user, UserDetails.CurrentUser().Name);

            // Add default user group permissions
            UserGroupPermissions userGroupPermissions = new UserGroupPermissions
            {
                UserId = user.Id,
                ProfileId = ProfileIds.IndicatorsForReview
            };
            _userRepository.SaveUserGroupPermissions(userGroupPermissions);

            return RedirectToAction("UserIndex");
        }

        [HttpPost]
        [ValidateInput(false)]
        [Route("user-edit")]
        [MultipleButton(Name = "action", Argument = "UpdateUser")]
        public ActionResult UpdateUser(EditUserViewModel viewModel)
        {
            var user = new FpmUser
            {
                Id = viewModel.FpmUserId,
                DisplayName = viewModel.DisplayName,
                IsAdministrator = viewModel.IsAdministrator,
                IsReviewer = viewModel.IsReviewer,
                UserName = viewModel.UserName,
                IsCurrent = viewModel.IsCurrent
            };


            //fpm_userAudit userid should be equal to fpm_user id
            _userRepository.UpdateUser(user, UserDetails.CurrentUser().Name);

            return RedirectToAction("UserIndex");
        }

        [HttpPost]
        [ValidateInput(false)]
        [Route("user-edit")]
        [MultipleButton(Name = "action", Argument = "AddProfilePermissionToUser")]
        public ActionResult AddProfilePermissionToUser(string profileId, [Bind(Include = "FpmUserId")]EditUserViewModel viewModel)
        {
            // Add user permission if profile ID valid
            int profileIdInt;
            if (int.TryParse(profileId, out profileIdInt))
            {
                UserGroupPermissions user = new UserGroupPermissions
                {
                    UserId = viewModel.FpmUserId,
                    ProfileId = profileIdInt
                };
                _userRepository.SaveUserGroupPermissions(user);
            }

            // Stay on user edit page
            return Redirect(Request.RawUrl);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Route("user-edit")]
        [MultipleButton(Name = "action", Argument = "RemoveProfilePermissionFromUser")]
        public ActionResult RemoveProfilePermissionFromUser(string profileId, [Bind(Include = "FpmUserId")]EditUserViewModel viewModel)
        {
            var userId = viewModel.FpmUserId;

            // Remove user permission if profile ID valid
            int profileIdInt;
            if (int.TryParse(profileId, out profileIdInt))
            {
                _userRepository.DeleteUserGroupPermissions(profileIdInt, userId);
            }

            // Stay on user edit page.
            return Redirect(Request.RawUrl);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Route("user-edit")]
        [MultipleButton(Name = "action", Argument = "RemoveAllProfilePermissionsFromUser")]
        public ActionResult RemoveAllProfilePermissionsFromUser(string profileId, [Bind(Include = "FpmUserId")]EditUserViewModel viewModel)
        {
            var userId = viewModel.FpmUserId;

            // Remove all user permissions
            var userDetails = UserDetails.NewUserFromUserId(userId);
            var profiles = userDetails.GetProfilesUserHasPermissionsTo();
            foreach (var profile in profiles)
            {
                _userRepository.DeleteUserGroupPermissions(profile.Id, userId);
            }

            // Stay on user edit page.
            return Redirect(Request.RawUrl);
        }

        [HttpGet]
        [Route("users")]
        public ActionResult GetAllUsers()
        {
            var allUsers = _userRepository.GetAllFpmUsers().OrderBy(x => x.UserName);
            return Json(allUsers, JsonRequestBehavior.AllowGet);
        }

        [Route("user-audit")]
        public ActionResult GetUserAudit(IEnumerable<int> jdata)
        {
            var auditLog = _userRepository.GetUserAudit(jdata.ToList());

            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserAudit", auditLog);
            }

            return View("UserIndex");
        }
    }
}