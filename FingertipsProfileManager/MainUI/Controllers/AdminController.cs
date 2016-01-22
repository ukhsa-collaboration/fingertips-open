using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
        private ProfilesWriter writer = ReaderFactory.GetProfilesWriter();
        private ProfileRepository _profileRepository;

        public ActionResult Admin()
        {
            var model = new AdminModel
            {
                AdminType = "Please select an option"
            };

            return View(model);
        }

        public ActionResult DeleteIndicator()
        {
            var model = new AdminModel
            {
                AdminType = "Delete indicator"
            };

            return View("Admin", model);
        }

        [HttpPost]
        public JsonResult DeleteIndicator(string indicatorIds)
        {
            var indicatorList = indicatorIds.Split(',').ToList();
            foreach (var indicatorId in indicatorList)
            {
                var user = UserDetails.CurrentUser();
                if (user.IsPholioDataManager == false)
                {
                    throw new FpmException(
                        "User not allowed to delete indicators: " + user.Name);
                }

                var indicatorMetadata = writer.GetIndicatorMetadata(Convert.ToInt32(indicatorId));
                if (indicatorMetadata != null)
                {
                    writer.DeleteIndicatorMetatdataById(indicatorMetadata);
                }
            }

            return Json(new
            {
                Ids = indicatorIds
            });
        }

        [HttpGet]
        public ActionResult ChangeIndicatorOwner()
        {
            var model = new AdminModel
            {
                AdminType = "Change indicator owner",
                Profiles = _reader.GetProfiles().OrderBy(x => x.Name).ToList()
            };

            return View("Admin", model);
        }

        [HttpPost]
        public JsonResult ChangeIndicatorOwner(int indicatorId, int newOwnerProfileId)
        {
            CheckUserIsPholioDataManager();
            EnsureProfileRepositoryDefined();
            new IndicatorOwnerChanger(_reader, _profileRepository)
                .AssignIndicatorToProfile(indicatorId, newOwnerProfileId);

            return Json(new
            {
                Id = indicatorId
            });
        }

        private static void CheckUserIsPholioDataManager()
        {
            var user = UserDetails.CurrentUser();
            if (user.IsPholioDataManager == false)
            {
                throw new FpmException(
                    "User not allowed to delete indicators: " + user.Name);
            }
        }

        /// <summary>
        /// Required for when action methods called from other controllers
        /// </summary>
        private void EnsureProfileRepositoryDefined()
        {
            if (_profileRepository == null)
            {
                _profileRepository = new ProfileRepository();
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _profileRepository = new ProfileRepository();

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _profileRepository.Dispose();

            base.OnActionExecuted(filterContext);
        }
    }
}