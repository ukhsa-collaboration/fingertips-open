using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.MainUI.Models.AreaList;

namespace IndicatorsUI.MainUI.Controllers
{
    [RoutePrefix("user-account/area-list")]
    public class AreaListController : BaseController
    {
        private readonly IIdentityWrapper _identityWrapper;

        public AreaListController(IIdentityWrapper identityWrapper, IAppConfig appConfig) : base(appConfig)
        {
            _identityWrapper = identityWrapper;
        }
        
        [Route("")]
        public ActionResult Index()
        {
            if (!IsUserSignedIn())
            {
                return RedirectToAction("Login", "UserAccount");
            }

            var user = _identityWrapper.GetApplicationUser(User);

            AreaListViewModel model = new AreaListViewModel()
            {
                UserId = user.Id
            };

            return View("AreaListIndex", model);
        }

        [Route("create")]
        public ActionResult Create()
        {
            if (!IsUserSignedIn())
            {
                return RedirectToAction("Login", "UserAccount");
            }

            var user = _identityWrapper.GetApplicationUser(User);

            AreaListViewModel model = new AreaListViewModel()
            {
                UserId = user.Id
            };

            return View("CreateAreaList", model);
        }

        [Route("edit")]
        public ActionResult Edit(string list_id)
        {
            if (!IsUserSignedIn())
            {
                return RedirectToAction("Login", "UserAccount");
            }

            var user = _identityWrapper.GetApplicationUser(User);

            AreaListViewModel model = new AreaListViewModel()
            {
                UserId = user.Id,
                PublicId = list_id
            };

            return View("EditAreaList", model);
        }

        private bool IsUserSignedIn()
        {
            return _identityWrapper.IsUserSignedIn(User);
        }
    }
}