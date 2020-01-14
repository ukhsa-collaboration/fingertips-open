using System.Web.Mvc;
using IndicatorsUI.DataConstruction;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Caching;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.MainUI.Models;
using IndicatorsUI.DataAccess;

namespace IndicatorsUI.MainUI.Controllers
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

        public SingleProfileWithFrontPageController(IAppConfig appConfig) : base(appConfig)
        {

        }

        [Route("profile/{urlKey}")]
        public ActionResult FrontPage(string urlKey)
        {
            ViewBag.ShowUpdateDelayedMessage = _appConfig.ShowUpdateDelayedMessage;

            var viewFolder = urlKey.ToLower() == ProfileUrlKeys.Phof
                ? "Phof"
                : DefaultViewFolder;

            return GetPage(ViewNameFrontPage, viewFolder, urlKey,
                PageType.FrontPageOfProfileWithFrontPage);
        }

        [Route("profile/{urlKey}/data")]
        public ActionResult Data(string urlKey)
        {
            return GetPage(ViewNameData, DefaultViewFolder, urlKey,
                PageType.DataPageOfProfileWithFrontPage);
        }

        [Route("profile/{urlKey}/supporting-information/{contentKey}")]
        public ActionResult SupportingPage(string urlKey, string contentKey)
        {
            var result = GetPage(ViewNameSupportingInformation, DefaultViewFolder, urlKey,
                PageType.SupportingInformation);

            ViewBag.SupportingInformationContentItem = 
                ContentProvider.GetContentItem(contentKey, profileDetails.Id);

            return result;
        }

        [Route("profile/{urlKey}/supporting-information/{parentContentKey}/{contentKey}")]
        public ActionResult SupportingPageUnderParent(string urlKey, string parentContentKey, string contentKey)
        {
            var result = GetPage(ViewNameSupportingInformation, DefaultViewFolder, urlKey,
                PageType.SupportingInformation);

            ViewBag.SupportingInformationContentItem = ContentProvider.GetContentItem(contentKey, profileDetails.Id);
            ViewBag.ParentContentItem = ContentProvider.GetContentItem(parentContentKey, profileDetails.Id); ;

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
            PageModel.PageType = pageType;
            PageModel.DisplayProfileTitle = true;

            return View(string.Format("~/views/{0}/{1}.cshtml", viewFolder, viewName), PageModel);
        }
    }
}
