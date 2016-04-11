using System.Collections.Generic;
using System.Web.Mvc;
using Profiles.DataAccess;
using Profiles.DataConstruction;
using Profiles.DomainObjects;
using Profiles.MainUI.Filters;
using Profiles.MainUI.Caching;
using Profiles.MainUI.Helpers;
using Profiles.MainUI.Models;

namespace Profiles.MainUI.Controllers
{
    [FingertipsOutputCache]
    public class ProfileCollectionWithFrontPageController : BaseController
    {
        private ProfileDetails details;

        [CheckUserCanAccessSkin]
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

        [CheckUserCanAccessSkin]
        public ActionResult ProfileFrontPage(string leadProfileUrlKey, string profileUrlKey)
        {
            InitPageModel();
            PageModel.PageType = PageType.FrontPageOfProfileInCollection;
            details = new ProfileDetailsBuilder(profileUrlKey).Build();

            if (AccessControlHelper.ShouldDenyAccess(details))
            {
                return AccessControlHelper.GetAccessNotAllowedActionResult();
            }

            return GetView(leadProfileUrlKey, "ProfileFrontPage");
        }

        [CheckUserCanAccessSkin]
        public ActionResult Data(string leadProfileUrlKey, string profileUrlKey)
        {
            InitPageModel();
            PageModel.PageType = PageType.DataPageOfProfileInCollection;
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

            SetProfileCollection(leadProfileUrlKey);

            ConfigureWithProfile(details);

            ViewBag.LeadProfileUrlKey = leadProfileUrlKey;

            return View(viewName, PageModel);
        }

        private void SetProfileCollection(string leadProfileUrlKey)
        {
            int? id = details.ProfileUrlKey == leadProfileUrlKey ?
                details.LeadProfileForCollectionId :
                new ProfileDetailsBuilder(leadProfileUrlKey).Build().LeadProfileForCollectionId;

            var profileCollection = new ProfileCollectionBuilder(ReaderFactory.GetProfileReader(), appConfig)
                .GetCollection(id.Value);
            profileCollection.UrlKey = leadProfileUrlKey;
            foreach (var item in profileCollection.ProfileCollectionItems)
            {
                item.ParentCollection = profileCollection;
            }
            PageModel.ProfileCollection = new List<ProfileCollection> { profileCollection };
        }
    }
}