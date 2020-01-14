using System;
using System.Linq;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Controllers
{
    [AdminUsersOnly]
    [RoutePrefix("admin")]
    public class AdminController : Controller
    {
        private readonly IProfilesReader _reader;
        private readonly IProfilesWriter _writer;

        private IProfileRepository _profileRepository;

        public AdminController(IProfilesReader reader, IProfilesWriter writer, IProfileRepository profileRepository)
        {
            _reader = reader;
            _writer = writer;

            _profileRepository = profileRepository;
        }

        [Route]
        public ActionResult Admin()
        {
            var model = new AdminModel
            {
                AdminType = "Please select an option"
            };

            return View(model);
        }

        [Route("delete-indicator")]
        public ActionResult DeleteIndicator()
        {
            var model = new AdminModel
            {
                AdminType = "Delete indicator"
            };

            return View("Admin", model);
        }

        [HttpPost]
        [Route("delete-indicator")]
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

                var indicatorMetadata = _writer.GetIndicatorMetadata(Convert.ToInt32(indicatorId));
                if (indicatorMetadata != null)
                {
                    _writer.DeleteIndicatorMetatdataById(indicatorMetadata);
                }
            }

            return Json(new
            {
                Ids = indicatorIds
            });
        }

        [HttpGet]
        [Route("change-indicator-owner")]
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
        [Route("change-indicator-owner")]
        public JsonResult ChangeIndicatorOwner(int indicatorId, int newOwnerProfileId)
        {
            EnsureProfileRepositoryDefined();
            new IndicatorOwnerChanger(_reader, _profileRepository)
                .AssignIndicatorToProfile(indicatorId, newOwnerProfileId);

            return Json(new
            {
                Id = indicatorId
            });
        }

        [HttpGet]
        [Route("get-indicator-owner")]
        public JsonResult GetIndicatorOwner(int indicatorId)
        {
            var profile = _reader.GetOwnerProfilesByIndicatorIds(indicatorId);

            return Json(profile, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Required for when action methods called from other controllers
        /// </summary>
        private void EnsureProfileRepositoryDefined()
        {
            if (_profileRepository == null)
            {
                _profileRepository = new ProfileRepository(NHibernateSessionFactory.GetSession());
            }
        }
    }
}