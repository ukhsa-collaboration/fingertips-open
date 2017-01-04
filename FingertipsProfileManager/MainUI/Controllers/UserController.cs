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
    public class UserController : Controller
    {
        private UserRepository _userRepository;
        private readonly ProfilesReader _profilesReader = ReaderFactory.GetProfilesReader();

        [AuthorizedUsers]
        public ActionResult UserIndex()
        {
            var model = new UserGridModel();

            var allUsers = _userRepository.GetAllFpmUsers().OrderBy(x => x.UserName);
            model.UserGrid = allUsers;

            return View(model);
        }

        [HttpGet]
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

            // Return URL
            if (HttpContext.Request.UrlReferrer != null)
            {
                viewModel.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();
            }

            ViewBag.ProfilesUserHasPermissionTo = userDetails.GetProfilesUserHasPermissionsTo();

            // Profile drop down
            var profileList = _profilesReader.GetProfiles().OrderBy(x => x.Name);
            viewModel.ProfileId = profileList.First().Id;
            ViewBag.ProfileId = new SelectList(profileList, "Id", "Name");

            return View("EditUser", viewModel);
        }

        public ActionResult CreateUser()
        {
            var user = new FpmUser();
            if (HttpContext.Request.UrlReferrer != null) user.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();

            return View("CreateUser", user);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultipleButton(Name = "action", Argument = "UpdateUser")]
        public ActionResult UpdateUser(EditUserViewModel viewModel)
        {
            var user = new FpmUser();

            user.Id = viewModel.FpmUserId;
            user.DisplayName = viewModel.DisplayName;
            user.IsAdministrator = viewModel.IsAdministrator;
            user.UserName = viewModel.UserName;

            //fpm_userAudit userid should be equal to fpm_user id
            _userRepository.UpdateUserItem(user, UserDetails.CurrentUser().Name);

            return RedirectToAction("UserIndex");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult InsertUser(FpmUser user)
        {
            if (!TryUpdateModel(user))
            {
                ViewBag.updateError = "Update Failure";
                return View("CreateUser", user);
            }

            _userRepository.CreateUserItem(user, UserDetails.CurrentUser().Name);

            return RedirectToAction("UserIndex");
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultipleButton(Name = "action", Argument = "AddProfile")]
        public ActionResult AddProfile(string profileId, [Bind(Include = "FpmUserId")]EditUserViewModel viewModel)
        {
            // Add user permission if profile ID valid
            int profileIdInt;
            if (int.TryParse(profileId, out profileIdInt))
            {
                UserGroupPermissions user = new UserGroupPermissions();
                user.UserId = viewModel.FpmUserId;
                user.ProfileId = profileIdInt;
                _userRepository.SaveUserGroupPermissions(user);
            }

            // Stay on user edit page
            return Redirect(Request.RawUrl);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultipleButton(Name = "action", Argument = "RemoveProfile")]
        public ActionResult RemoveProfile(string profileId, [Bind(Include = "FpmUserId")]EditUserViewModel viewModel)
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

        [HttpGet]
        public ActionResult GetAllUsers()
        {
            var allUsers = _userRepository.GetAllFpmUsers().OrderBy(x => x.UserName);
            return Json(allUsers, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetUserAudit(IEnumerable<int> jdata)
        {
            var auditLog = _userRepository.GetUserAudit(jdata.ToList());

            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserAudit", auditLog);
            }

            return View("UserIndex");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _userRepository = new UserRepository();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _userRepository.Dispose();

            base.OnActionExecuted(filterContext);
        }

    }
}