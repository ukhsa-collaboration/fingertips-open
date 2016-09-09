using System.Collections.Generic;
using System.Web.Mvc;
using Profiles.DomainObjects;
using Profiles.MainUI.Filters;
using Profiles.MainUI.Caching;
using Profiles.MainUI.Helpers;
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
            // Get a list of ProfileCollections for this skin
            IList<SkinProfileCollection> skinProfileCollections =
                ProfileCollectionProvider.GetSkinProfileCollections(PageModel.Skin.Id);
            foreach (SkinProfileCollection skinProfileCollection in skinProfileCollections)
            {
                PageModel.ProfileCollections.Add(
                    ProfileCollectionBuilder.GetCollection(skinProfileCollection.ProfileCollectionId));
            }

            ConfigureFingertipsProfileAndPageModelWithProfileDetails(PageModel.Skin.TemplateProfileUrlKey);

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
            var details = ConfigureFingertipsProfileAndPageModelWithProfileDetails(profileKey);

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
                ContentProvider.GetContentItem(contentKey, PageModel.ProfileId);
            PageModel.PageTitle = details.Title;
            PageModel.DisplayProfileTitle = true;

            PageModel.PageType = PageType.SupportingInformation;
            return View(PageModel);
        }

        /// <summary>
        /// Get a simple HTML page for the PHOF profile.
        /// </summary>
        [CheckUserCanAccessSkin]
        public ActionResult SimpleHtmlPage(string viewName)
        {
            ConfigureFingertipsProfileAndPageModelWithProfileDetails(PageModel.Skin.TemplateProfileUrlKey);

            PageModel.PageType = PageType.SupportingInformation;

            return View(PageModel.GetSkinView(viewName), PageModel);
        }

        /// <summary>
        /// Data page used for standalone profiles without a front page.
        /// </summary>
        [CheckUserCanAccessSkin]
        public ActionResult Data(string profileKey)
        {
            PageModel.DisplayProfileTitle = true;

            var details = ConfigureFingertipsProfileAndPageModelWithProfileDetails(profileKey);

            if (AccessControlHelper.ShouldDenyAccess(details))
            {
                return AccessControlHelper.GetAccessNotAllowedActionResult();
            }

            PageModel.PageType = PageType.DataPageOfProfileWithoutFrontPage;
            return View(PageModel);
        }
    }
}