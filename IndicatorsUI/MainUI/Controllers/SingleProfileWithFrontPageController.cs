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
        public const string ViewNameSupportingInformation = "SupportingInformation";

        private ProfileDetails profileDetails;

        public ActionResult FrontPage(string urlKey)
        {
            ViewBag.ShowUpdateDelayedMessage = appConfig.ShowUpdateDelayedMessage;

            return GetPage(ViewNameFrontPage, DefaultViewFolder, urlKey,
                PageType.FrontPageOfProfileWithFrontPage);
        }

        public ActionResult Data(string urlKey)
        {
            return GetPage(ViewNameData, DefaultViewFolder, urlKey,
                PageType.DataPageOfProfileWithFrontPage);
        }

        public ActionResult SupportingPage(string urlKey, string contentKey)
        {
            var result = GetPage(ViewNameSupportingInformation, DefaultViewFolder, urlKey,
                PageType.SupportingInformation);

            ViewBag.SupportingInformationContentItem = 
                ContentProvider.GetContentItem(contentKey, profileDetails.Id);

            return result;
        }

        public ActionResult PracticeProfilesFrontPage(string urlKey)
        {
            return GetPage(ViewNameFrontPage, PracticeProfilesViewFolder, urlKey,
                PageType.FrontPageOfProfileWithFrontPage);
        }

        public ActionResult PracticeProfilesData(string urlKey)
        {
            return GetPage(ViewNameData, PracticeProfilesViewFolder, urlKey,
                PageType.DataPageOfProfileWithFrontPage);
        }

        [CheckUserCanAccessSkin]
        private ActionResult GetPage(string viewName, string viewFolder, string urlKey,
            PageType pageType)
        {
            InitPageModel();
      
            profileDetails = new ProfileDetailsBuilder(urlKey).Build();

            if (profileDetails == null)
            {

                ErrorController.InvokeHttp404(HttpContext);
                return new EmptyResult();
            }

            if (AccessControlHelper.ShouldDenyAccess(profileDetails))
            {
                return AccessControlHelper.GetAccessNotAllowedActionResult();
            }

            PageModel.PageTitle = profileDetails.Title;

            ConfigureWithProfile(profileDetails);

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
