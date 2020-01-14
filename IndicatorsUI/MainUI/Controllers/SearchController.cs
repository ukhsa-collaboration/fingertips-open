using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DataConstruction;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.MainUI.Models;
using IndicatorsUI.UserAccess.UserList.IRepository;

namespace IndicatorsUI.MainUI.Controllers
{
    public class SearchController : BaseController
    {
        public const string TitleSearchResults = "Search Results";

        private const string UrlKey = "search";
        private const string DataView = "Data";

        private IIndicatorListRepository _indicatorListRepository;
        private readonly IIdentityWrapper _identity;

        public SearchController(IIndicatorListRepository indicatorListRepository,
            IIdentityWrapper identity,
            IAppConfig appConfig) : base(appConfig)

        {
            _indicatorListRepository = indicatorListRepository;
            _identity = identity;
        }

        [Route("search/{searchText}")]
        public ActionResult Data(string searchText = null)
        {
            InitSearch(searchText);
            return View(GetViewName(), PageModel);
        }

        [Route("view/{searchText?}")]
        public ActionResult ViewSearchText(string searchText = null)
        {
            InitSearch(searchText);
            return View(GetViewName(), PageModel);
        }

        [Route("indicator-list/view/{listId}")]
        public ActionResult ViewIndicatorList(string listId)
        {
            InitSearch(null);
            PageModel.PageType = PageType.IndicatorListDataPage;

            var indicatorList = _indicatorListRepository.GetListByPublicId(listId);
            ViewBag.IndicatorListName = indicatorList.ListName;
            ViewBag.IndicatorListPublicId = listId;

            var user = _identity.GetApplicationUser(User);
            if (user == null)
            {
                ViewBag.ShowEditLink = false;
            }
            else
            {
                if (_indicatorListRepository.DoesUserOwnList(listId, user.Id))
                {
                    ViewBag.ShowEditLink = true;
                }
                else
                {
                    ViewBag.ShowEditLink = false;
                }
            }

            ((IList<string>)ViewBag.ExtraJsFiles).Add("js-indicator-list-view");

            return View(DataView, PageModel);
        }

        /// <summary>
        /// This included to demonstrate that a list of indicators can be displayed by including the
        /// IDs in the URL.
        /// </summary>
        /// <param name="indicatorList">Comma-separated list of indicator IDs</param>
        [Route("indicators/{indicatorList}")]
        public ActionResult SelectedIndicators(string indicatorList = "")
        {
            InitPageModel();
            ConfigureWithProfile(new ProfileDetailsBuilder(UrlKey).Build());
            PageModel.PageTitle = "Selected Data";
            ViewData["indicatorIds"] = indicatorList;
            ViewBag.SearchText = string.Empty;
            PageModel.PageType = PageType.IndicatorSearchResultsDataPage;
            return View(GetViewName(), PageModel);
        }

        private void InitSearch(string searchText)
        {
            InitPageModel();
            ConfigureWithProfile(new ProfileDetailsBuilder(UrlKey).Build());

            ProfileDetails templateProfile = new ProfileDetailsBuilder(PageModel.Skin.TemplateProfileUrlKey).Build();

            // Override according to skin
            ViewBag.DefaultAreaType = templateProfile.DefaultAreaType;
            ViewBag.EnumParentDisplay = templateProfile.EnumParentDisplay;
            ViewBag.ExtraJsFiles = templateProfile.ExtraJavaScriptFiles
                .Concat((IList<string>) ViewBag.ExtraJsFiles)
                .ToList();
            ViewBag.ExtraCssFiles = templateProfile.ExtraCssFiles;

            PageModel.PageTitle = TitleSearchResults;
            ViewBag.SearchText = searchText ?? string.Empty;

            PageModel.StartZeroYAxis = templateProfile.StartZeroYAxis;
            PageModel.DefaultFingertipsTabId = templateProfile.DefaultFingertipsTabId;
            PageModel.IgnoredSpineChartAreas = templateProfile.AreasToIgnoreForSpineCharts;

            // Profile used to provide configuration details for the search
            PageModel.TemplateProfileId = templateProfile.Id;

            PageModel.PageType = PageType.IndicatorSearchResultsDataPage;
        }

        private string GetViewName()
        {
            // Search is not available on test sites to limit access to indicators on controlled sites
            var viewName = _appConfig.IsIndicatorSearchAvailable
                ? DataView
                : "SearchNotAvailable";
            return viewName;
        }
    }
}
