using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Profiles.DataConstruction;
using Profiles.MainUI.Filters;
using Profiles.MainUI.Helpers;
using Profiles.DomainObjects;

namespace Profiles.MainUI.Controllers
{
    public class SearchController : BaseController
    {
        public const string TitleSelectedIndicators = "Selected Indicators";
        public const string TitleSearchResults = "Search Results";

        private const string UrlKey = "search";
        private const string DataView = "Data";

        [CheckUserCanAccessSkin] 
        public ActionResult Data(string searchText = null)
        {
            InitPageModel();
            ConfigureWithProfile(new ProfileDetailsBuilder(UrlKey).Build());

            ProfileDetails templateProfile = new ProfileDetailsBuilder(PageModel.Skin.TemplateProfileUrlKey).Build();

            // Override according to skin
            ViewBag.DefaultAreaType = templateProfile.DefaultAreaType;
            ViewBag.EnumParentDisplay = templateProfile.EnumParentDisplay;
            ViewBag.ExtraJsFiles = templateProfile.ExtraJavaScriptFiles.Concat(ViewBag.ExtraJsFiles as IList<string>);
            ViewBag.ExtraCssFiles = templateProfile.ExtraCssFiles;

            PageModel.PageTitle = TitleSearchResults;
            ViewBag.SearchText = searchText ?? string.Empty;

            PageModel.RagColourId = templateProfile.RagColourId;
            PageModel.StartZeroYAxis = templateProfile.StartZeroYAxis;
            PageModel.DefaultFingertipsTabId = templateProfile.DefaultFingertipsTabId;
            PageModel.IgnoredSpineChartAreas = templateProfile.AreasToIgnoreForSpineCharts;

            //Check to see if there's a profileCollection added for the current skin. Restrict the search to just the profiles of this ProfileCollection if there is.
            GetProfileCollection();
            var searchableProfiles =
                PageModel.ProfileCollection.SelectMany(x => x.ProfileCollectionItems)
                    .Select(c => c.ProfileId)
                    .ToArray();

            PageModel.ProfileCollectionIdList = searchableProfiles;
            PageModel.TemplateProfileId = templateProfile.Id;

            return View(GetViewName(), PageModel);
        }

        private string GetViewName()
        {
            // Search is not available on test sites to limit access to indicators on controlled sites
            var viewName = appConfig.IsIndicatorSearchAvailable
                ? DataView
                : "SearchNotAvailable";
            return viewName;
        }

        private void GetProfileCollection()
        {
            //get a list of ProfileCollections for this skin
            var skinProfileCollection = ProfileCollectionProvider.GetSkinProfileCollections(PageModel.Skin.Id);

            foreach (var pc in skinProfileCollection)
            {
                var profileCollection = ProfileCollectionProvider.GetProfileCollection(pc.ProfileCollectionId);

                profileCollection.ProfileCollectionItems = ProfileCollectionProvider.GetProfileCollectionItems(profileCollection.Id);
                foreach (var pci in profileCollection.ProfileCollectionItems)
                {
                    pci.ProfileDetails = new ProfileDetailsBuilder(pci.ProfileId).Build();
                }
                PageModel.ProfileCollection.Add(profileCollection);
            }
        }

        /// <summary>
        /// This included to demonstrate that a list of indicators can be displayed by including the
        /// IDs in the URL.
        /// </summary>
        /// <param name="indicatorList">Comma-separated list of indicator IDs</param>
        [CheckUserCanAccessSkin] 
        public ActionResult SelectedIndicators(string indicatorList = "")
        {
            InitPageModel();
            ConfigureWithProfile(new ProfileDetailsBuilder(UrlKey).Build());
            PageModel.PageTitle = "Selected Data";
            ViewData["indicatorIds"] = indicatorList;
            ViewBag.SearchText = string.Empty;
            return View(GetViewName(), PageModel);
        }
    }
}
