using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    public class UserController : Controller
    {
        private UserRepository _userRepository;

        [AuthorizedUsers]
        public ActionResult UserIndex()
        {
            var model = new UserGridModel();

            var allUsers = _userRepository.GetAllFpmUsers().OrderBy(x => x.UserName);
            model.UserGrid = allUsers;

            return View(model);
        }

        public ActionResult EditUser(int userId)
        {
            var userDetails = UserDetails.NewUserFromUserId(userId);
            var fpmUser = userDetails.FpmUser;

            if (HttpContext.Request.UrlReferrer != null)
            {
                fpmUser.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();
            }

            ViewBag.ProfilesUserHasPermissionTo = userDetails.GetProfilesUserHasPermissionsTo();

            return View("EditUser", fpmUser);
        }

        public ActionResult ReloadUserItems()
        {
            throw new NotImplementedException();
        }

        public ActionResult CreateUser()
        {
            var user = new FpmUser();
            if (HttpContext.Request.UrlReferrer != null) user.ReturnUrl = HttpContext.Request.UrlReferrer.ToString();

            return View("CreateUser", user);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateUser(FpmUser user)
        {
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