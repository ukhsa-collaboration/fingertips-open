using System.Collections.Generic;
using System.Web.Mvc;
using Profiles.DataConstruction;
using Profiles.DomainObjects;
using Profiles.MainUI.Filters;
using Profiles.MainUI.Caching;
using Profiles.MainUI.Common;
using Profiles.MainUI.Models;

namespace Profiles.MainUI.Controllers
{
    [FingertipsOutputCache]
    public class ProfileController : BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Called here because the Request object is not available in the constructor
            InitPageModel();
        }

        /// <summary>
        /// Front page of site.
        /// </summary>
        /// <returns></returns>
        [CheckUserCanAccessSkin]
        public ActionResult FrontPage()
        {
            PageModel.DisplayProfileTitle = true;

            // Get a list of ProfileCollections for this skin
            IList<SkinProfileCollection> skinProfileCollections =
                CommonUtilities.GetSkinProfileCollections(PageModel.Skin.Id);
            foreach (SkinProfileCollection skinProfileCollection in skinProfileCollections)
            {
                PageModel.ProfileCollection.Add(
                    ProfileCollectionBuilder.GetCollection(skinProfileCollection.ProfileCollectionId));
            }

            ConfigureDetails();

            PageModel.PageType = PageType.SiteFrontPage;

            ViewBag.ShowUpdateDelayedMessage = appConfig.ShowUpdateDelayedMessage;

            return View(PageModel.GetSkinView("Frontpage"), PageModel);
        }

        /// <summary>
        /// A supporting information page for specific profile.
        /// </summary>
        [CheckUserCanAccessSkin]
        public ActionResult SupportingInformation(string profileKey, string contentKey)
        {
            var details = GetProfileDetails(profileKey);

            if (details == null)
            {
                ErrorController.InvokeHttp404(HttpContext);
                return new EmptyResult();
            }

            if (AccessControlHelper.ShouldDenyAccess(details))
            {
                return AccessControlHelper.GetAccessNotAllowedActionResult();
            }

            ViewBag.SupportingInformationContentItem =
                CommonUtilities.GetContentItem(contentKey, PageModel.ProfileId);

            PageModel.PageType = PageType.SupportingInformation;
            return View(PageModel);
        }

        /// <summary>
        /// Get a simple HTML page for the PHOF profile.
        /// </summary>
        [CheckUserCanAccessSkin]
        public ActionResult SimpleHtmlPage(string viewName)
        {
            PageModel.DisplayProfileTitle = true;

            ConfigureDetails();

            CheckSkinIsNotLongerLives();

            PageModel.PageType = PageType.SupportingInformation;

            return View(PageModel.GetSkinView(viewName), PageModel);
        }

        /// <summary>
        /// Data page used for standalone profiles without a front page.
        /// </summary>
        [CheckUserCanAccessSkin]
        public ActionResult Data(string profileKey)
        {
            var details = GetProfileDetails(profileKey);

            if (AccessControlHelper.ShouldDenyAccess(details))
            {
                return AccessControlHelper.GetAccessNotAllowedActionResult();
            }

            PageModel.PageType = PageType.DataPageOfProfileWithoutFrontPage;
            return View(PageModel);
        }

        private ProfileDetails GetProfileDetails(string profileKey)
        {
            ProfileDetails details = new ProfileDetailsBuilder(profileKey).Build();

            if (details != null)
            {
                PageModel.PageTitle = details.Title;
                PageModel.ProfileUrlKey = details.ProfileUrlKey;

                ConfigureWithProfile(details);

                CheckSkinIsNotLongerLives();
            }
            return details;
        }

        private void ConfigureDetails()
        {
            ProfileDetails details = new ProfileDetailsBuilder(PageModel.Skin.TemplateProfileUrlKey).Build();
            PageModel.PageTitle = details.Title;
            PageModel.ProfileUrlKey = details.ProfileUrlKey;
            ConfigureWithProfile(details);
        }

        private void CheckSkinIsNotLongerLives()
        {
            if (PageModel.Skin.IsLongerLives)
            {
                throw new FingertipsException("This view is not available for Longer Lives skin");
            }
        }
    }
}