using System.Web.UI;
using Profiles.DataConstruction;
using Profiles.DomainObjects;
using Profiles.MainUI.Caching;
using Profiles.MainUI.Helpers;
using Profiles.MainUI.Filters;
using Profiles.MainUI.Models;
using System.Web.Mvc;

namespace Profiles.MainUI.Controllers
{
    [FingertipsOutputCache]
    public class SingleProfileWithFrontPageController : BaseController
    {
        public const string DefaultViewFolder = "SingleProfileWithFrontPage";
        public const string PracticeProfilesViewFolder = "PracticeProfiles";

        public const string ViewNameFrontPage = "FrontPage";
        public const string ViewNameData = "Data";

        private ProfileDetails details;

        public ActionResult FrontPage(string urlKey)
        {
            ViewBag.ShowUpdateDelayedMessage = appConfig.ShowUpdateDelayedMessage;

            return GetPage(ViewNameFrontPage, DefaultViewFolder, urlKey,
                PageType.FrontPageOfStandaloneProfileWithFrontPage);
        }

        public ActionResult Data(string urlKey)
        {
            return GetPage(ViewNameData, DefaultViewFolder, urlKey,
                PageType.DataPageOfStandaloneProfileWithFrontPage);
        }

        public ActionResult PracticeProfilesFrontPage(string urlKey)
        {
            return GetPage(ViewNameFrontPage, PracticeProfilesViewFolder, urlKey,
                PageType.FrontPageOfStandaloneProfileWithFrontPage);
        }

        public ActionResult PracticeProfilesData(string urlKey)
        {
            return GetPage(ViewNameData, PracticeProfilesViewFolder, urlKey,
                PageType.DataPageOfStandaloneProfileWithFrontPage);
        }

        [CheckUserCanAccessSkin]
        private ActionResult GetPage(string viewName, string viewFolder, string urlKey,
            PageType pageType)
        {
            InitPageModel();
      
            details = new ProfileDetailsBuilder(urlKey).Build();

            if (details == null)
            {

                ErrorController.InvokeHttp404(HttpContext);
                return new EmptyResult();
            }

            if (AccessControlHelper.ShouldDenyAccess(details))
            {
                return AccessControlHelper.GetAccessNotAllowedActionResult();
            }

            PageModel.PageTitle = details.Title;

            ConfigureWithProfile(details);

            if (PageModel.HasExclusiveSkin == false)
            {
                PageModel.Skin.PartialViewFolder = viewFolder;
            }

            PageModel.PageType = pageType;
            PageModel.DisplayProfileTitle = true;

            return View(PageModel.GetSkinView(viewName), PageModel);
        }
    }
}
