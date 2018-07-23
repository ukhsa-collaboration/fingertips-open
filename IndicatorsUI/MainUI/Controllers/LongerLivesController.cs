using IndicatorsUI.DataAccess;
using IndicatorsUI.DataConstruction;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Caching;
using IndicatorsUI.MainUI.Helpers;
using System.Globalization;
using System.Web.Mvc;

namespace IndicatorsUI.MainUI.Controllers
{
    [FingertipsOutputCache]
    public class LongerLivesController : BaseController
    {
        private readonly string _defaultProfileKey;

        private readonly ProfileReader _profileReader;

        public LongerLivesController(ProfileReader profileReader, IAppConfig appConfig) : base(appConfig)
        {
            _profileReader = profileReader;
            _defaultProfileKey = _appConfig.LongerLivesFrontPageProfileKey;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Called here because the Request object is not available in the constructor
            InitPageModel();

            SetMetaTagContent();
        }

        /// <summary>
        /// Healthier Lives front page
        /// </summary>
        public ActionResult Home()
        {
            return Home(null);
        }

        [Route("topic/{profileKey}")]
        public ActionResult Home(string profileKey)
        {
            profileKey = GetProfileKey(profileKey);
            InitPage(profileKey, "Home");
            return GetProfileView(profileKey, "Home");
        }

        [Route("topic/{profileKey}/map-with-data")]
        public ActionResult MapWithData(string profileKey = null)
        {
            profileKey = GetProfileKey(profileKey);
            InitPage(profileKey, "Map");
            ViewBag.MapNoData = true;
            return GetProfileView(profileKey, "Home");
        }

        [Route("topic/{profileKey}/about-project")]
        [Route("about-project")]
        public ActionResult AboutProject(string profileKey = null)
        {
            profileKey = GetProfileKey(profileKey);
            InitPage(profileKey, "About The Project");
            return View("AboutProject", PageModel);
        }

        [Route("topic/{profileKey}/about-data")]
        [Route("about-data")]
        public ActionResult AboutData(string profileKey = null)
        {
            profileKey = GetProfileKey(profileKey);
            InitPage(profileKey, "About The Data");
            return View("AboutData", PageModel);
        }

        [Route("topic/mortality/comparisons")]
        public ActionResult MortalityRankings()
        {
            var profileKey = "mortality";
            InitPage(profileKey, "Mortality Rankings");
            return GetProfileView(profileKey, "Rankings");
        }

        [Route("topic/mortality/area-details")]
        public ActionResult MortalityAreaDetails()
        {
            var profileKey = "mortality";
            InitPage(profileKey, "Mortality Rankings");
            return GetProfileView(profileKey, "AreaDetails");
        }

        [Route("topic/{profileKey}/health-intervention/{intervention}")]
        public ActionResult HealthIntervention(string intervention)
        {
            InitPage(ProfileUrlKeys.LongerLives, "Health Interventions");
            ViewBag.Intervention = intervention;
            return View("Mortality/MortalityHealthIntervention" + intervention.ToLower(), PageModel);
        }

        [Route("topic/{profileKey}/comparisons")]
        public ActionResult PracticeRankings(string profileKey)
        {
            InitPage(profileKey, "National comparisons");
            return GetProfileView(profileKey, "Rankings");
        }

        [Route("topic/{profileKey}/practice-details")]
        public ActionResult PracticeDetails(string profileKey)
        {
            InitPage(profileKey, "Practice Details");
            return GetProfileView(profileKey, "PracticeDetails");
        }

        [Route("topic/{profileKey}/area-details")]
        public ActionResult AreaDetails(string profileKey)
        {
            InitPage(profileKey, "Area Details");
            return View("Diabetes/DiabetesAreaDetails", PageModel);
        }

        [Route("topic/{profileKey}/connect")]
        [Route("connect")]
        public ActionResult Connect(string profileKey = null)
        {
            profileKey = GetProfileKey(profileKey);
            InitPage(profileKey, "Connect");
            return GetProfileView(profileKey, "Connect");
        }

        [Route("topic/{profileKey}/area-search-results")]
        public ActionResult AreaSearchResults(string profileKey = null)
        {
            profileKey = GetProfileKey(profileKey);
            var parameters = Request.QueryString;
            ViewBag.Area = parameters["place_name"];
            ViewBag.Easting = parameters["easting"];
            ViewBag.Northing = parameters["northing"];
            InitPage(profileKey, "Search Results");
            return GetProfileView(profileKey, "AreaSearchResults");
        }

        public ActionResult GetProfileView(string profileKey, string viewName)
        {
            TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;

            string prefix = textInfo.ToTitleCase(profileKey);
            if (prefix.Equals("Mortality") == false)
            {
                prefix = "Diabetes";
            }

            return View(prefix + "/" + prefix + viewName, PageModel);
        }

        public void InitPage(string profileKey, string pageName)
        {
            var profileDetails = new ProfileDetailsBuilder(profileKey).Build();
            ConfigureWithProfile(profileDetails);
            PageModel.PageTitle = pageName;
        }

        private void SetMetaTagContent()
        {
            ViewBag.MetaDescription = ContentHelper.RemoveHtmlTags(
                _profileReader.GetContentItem(ContentKeys.MetaDescription, ProfileIds.LongerLives).Content);

            ViewBag.MetaKeywords = ContentHelper.RemoveHtmlTags(
                _profileReader.GetContentItem(ContentKeys.MetaKeywords, ProfileIds.LongerLives).Content);
        }

        public ActionResult Get404Error()
        {
            return GetErrorView("Page not found", "Sorry, but the page you were trying to view does not exist.");
        }

        public ActionResult Get500Error()
        {
            return GetErrorView("Error", "Sorry, a unexpected error has occured.");
        }

        private ActionResult GetErrorView(string title, string message)
        {
            if (PageModel == null)
            {
                InitPageModel();
            }

            ConfigureWithProfile(new ProfileDetailsBuilder(_defaultProfileKey).Build());
            PageModel.PageTitle = title;
            ViewBag.ErrorMessage = message;
            return View("../LongerLives/Error", PageModel);
        }

        [Route("policy/{policyType}")]
        public ActionResult Policy(string policyType)
        {
            ConfigureWithProfile(new ProfileDetailsBuilder(_defaultProfileKey).Build());

            switch (policyType.ToLower())
            {
                case "privacy":
                    PageModel.PageTitle = "Privacy Policy";
                    return View("PolicyPrivacy", PageModel);

                case "accessibility":
                    PageModel.PageTitle = "Accessibility";
                    return View("PolicyAccessibility", PageModel);

                case "cookies":
                    PageModel.PageTitle = "Accessibility";
                    return View("PolicyCookies", PageModel);
            }

            return Get404Error();
        }

        private string GetProfileKey(string profileKey)
        {
            if (profileKey == null)
            {
                return _defaultProfileKey;
            }

            return profileKey;
        }
    }
}
