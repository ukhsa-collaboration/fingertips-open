using IndicatorsUI.DataAccess;
using IndicatorsUI.DataConstruction;
using IndicatorsUI.MainUI.Caching;
using System.Web.Mvc;

namespace IndicatorsUI.MainUI.Controllers
{
    [FingertipsOutputCache]
    public class LongerLivesController : BaseController
    {
        private readonly string _defaultProfileKey;

        private readonly ProfileReader _profileReader;

        private const string ProfileKey = "public-health-dashboard";

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
        }

        /// <summary>
        /// Healthier Lives front page
        /// </summary>
        public ActionResult Home()
        {
            return Home(ProfileKey);
        }

        [Route("topic/{profile}")]
        public ActionResult Home(string profile)
        {
            if (!IsCallingPublicHealthDashboard(profile))
                return SentToNotFoundPage();

            InitPage(ProfileKey, "Home");
            return GetProfileView("Home");
        }

        [Route("topic/{profile}/map-with-data")]
        public ActionResult MapWithData(string profile)
        {
            if (!IsCallingPublicHealthDashboard(profile))
                return SentToNotFoundPage();

            InitPage(ProfileKey, "Map");
            ViewBag.MapNoData = true;
            return GetProfileView("Home");
        }

        [Route("topic/{profile}/about-data")]
        public ActionResult AboutData(string profile)
        {
            if (!IsCallingPublicHealthDashboard(profile))
                return SentToNotFoundPage();

            InitPage(ProfileKey, "About The Data");
            return View("AboutData", PageModel);
        }

        [Route("topic/{profile}/comparisons")]
        public ActionResult PracticeRankings(string profile)
        {
            if (!IsCallingPublicHealthDashboard(profile))
                return SentToNotFoundPage();

            InitPage(ProfileKey, "National comparisons");
            return GetProfileView("Rankings");
        }

        [Route("topic/{profile}/area-details")]
        public ActionResult AreaDetails(string profile)
        {
            if (!IsCallingPublicHealthDashboard(profile))
                return SentToNotFoundPage();

            InitPage(ProfileKey, "Area Details");
            return View("Diabetes/DiabetesAreaDetails", PageModel);
        }


        [Route("topic/{profile}/area-search-results")]
        public ActionResult AreaSearchResults(string profile)
        {
            if (!IsCallingPublicHealthDashboard(profile))
                return SentToNotFoundPage();

            var parameters = Request.QueryString;
            ViewBag.Area = parameters["place_name"];
            ViewBag.Easting = parameters["easting"];
            ViewBag.Northing = parameters["northing"];
            InitPage(ProfileKey, "Search Results");
            return GetProfileView("AreaSearchResults");
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


        [Route("about-data")]
        public ActionResult SendToAboutData()
        {
            return AboutData(ProfileKey);
        }

        public ActionResult SentToNotFoundPage()
        {
            return Get404Error();
        }

        public ActionResult GetProfileView(string viewName)
        {
            return View("Diabetes/Diabetes" + viewName, PageModel);
        }

        public void InitPage(string profileKey, string pageName)
        {
            var profileDetails = new ProfileDetailsBuilder(profileKey).Build();
            ConfigureWithProfile(profileDetails);
            PageModel.PageTitle = pageName;
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

        private bool IsCallingPublicHealthDashboard(string profile)
        {
            return ProfileKey.Equals(profile);
        }

    }
}