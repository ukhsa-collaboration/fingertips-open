using System.Collections.Generic;
using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Caching;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.MainUI.Models;

namespace IndicatorsUI.MainUI.Controllers
{
    [FingertipsOutputCache]
    public class ProfileController : BaseController
    {
        public ProfileController(IAppConfig appConfig) : base(appConfig)
        {

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Called here because the Request object is not available in the constructor
            InitPageModel();
        }

        /// <summary>
        /// Front page of site.
        /// Route defined in RouteConfig.
        /// </summary>
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

            ViewBag.ShowUpdateDelayedMessage = _appConfig.ShowUpdateDelayedMessage;

            return View("~/Views/FingertipsLandingPage/Frontpage.cshtml", PageModel);
        }

        /// <summary>
        /// Data page used for standalone profiles without a front page.
        /// Route defined in RouteConfig.
        /// </summary>
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