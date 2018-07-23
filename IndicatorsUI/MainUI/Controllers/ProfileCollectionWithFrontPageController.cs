using System.Collections.Generic;
using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DataConstruction;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Caching;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.MainUI.Models;

namespace IndicatorsUI.MainUI.Controllers
{
    [FingertipsOutputCache]
    public class ProfileCollectionWithFrontPageController : BaseController
    {
        private ProfileDetails details;

        public ProfileCollectionWithFrontPageController(IAppConfig appConfig) : base(appConfig)
        {

        }

        [Route("profile-group/{leadProfileUrlKey}")]
        public ActionResult FrontPage(string leadProfileUrlKey)
        {
            InitPageModel();
            PageModel.PageType = PageType.ProfileCollectionFrontPage;
            details = new ProfileDetailsBuilder(leadProfileUrlKey).Build();

            if (AccessControlHelper.ShouldDenyAccess(details))
            {
                return AccessControlHelper.GetAccessNotAllowedActionResult();
            }

            return GetView(leadProfileUrlKey, "FrontPage");
        }

        public ActionResult ProfileFrontPage(string leadProfileUrlKey, string profileUrlKey)
        {
            InitPageModel();
            PageModel.PageType = PageType.FrontPageOfProfileWithFrontPage;
            details = new ProfileDetailsBuilder(profileUrlKey).Build();

            if (AccessControlHelper.ShouldDenyAccess(details))
            {
                return AccessControlHelper.GetAccessNotAllowedActionResult();
            }

            return GetView(leadProfileUrlKey, "ProfileFrontPage");
        }

        [Route("profile-group/{leadProfileUrlKey}/profile/{profileUrlKey}/data")]
        public ActionResult Data(string leadProfileUrlKey, string profileUrlKey)
        {
            InitPageModel();
            PageModel.PageType = PageType.DataPageOfProfileWithFrontPage;
            details = new ProfileDetailsBuilder(profileUrlKey).Build();

            if (AccessControlHelper.ShouldDenyAccess(details))
            {
                return AccessControlHelper.GetAccessNotAllowedActionResult();
            }

            return GetView(leadProfileUrlKey, "Data");
        }

        private ActionResult GetView(string leadProfileUrlKey, string viewName)
        {
            if (details == null)
            {
                ErrorController.InvokeHttp404(HttpContext);
                return new EmptyResult();
            }

            PageModel.Skin.PartialViewFolder = "ProfileCollectionWithFrontPage";
            PageModel.DisplayProfileTitle = true;
            PageModel.PageTitle = details.Title;
            ViewBag.ProfileUrlKey = details.ProfileUrlKey;

            SetProfileCollection(details, leadProfileUrlKey);

            ConfigureWithProfile(details);

            return View(viewName, PageModel);
        }
    }
}