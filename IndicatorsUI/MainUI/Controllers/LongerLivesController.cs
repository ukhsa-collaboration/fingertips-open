using System.Globalization;
using System.Web.Mvc;
using Profiles.DataAccess;
using Profiles.DataConstruction;
using Profiles.DomainObjects;
using Profiles.MainUI.Filters;
using Profiles.MainUI.Caching;
using Profiles.MainUI.Helpers;

namespace Profiles.MainUI.Controllers
{
    [FingertipsOutputCache]
    public class LongerLivesController : BaseController
    {
        private ProfileReader profileReader = ReaderFactory.GetProfileReader();
        private const string DefaultProfileKey = ProfileUrlKeys.Diabetes;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Called here because the Request object is not available in the constructor
            InitPageModel();

            SetMetaTagContent();
        }

        [CheckUserCanAccessSkin]
        public ActionResult Home(string profileKey = DefaultProfileKey)
        {
            InitPage(profileKey, "Home");
            return GetProfileView(profileKey, "Home");
        }

        [CheckUserCanAccessSkin]
        public ActionResult AboutProject(string profileKey = DefaultProfileKey)
        {
            InitPage(profileKey, "About The Project");
            return View("AboutProject", PageModel);
        }

        [CheckUserCanAccessSkin]
        public ActionResult AboutData(string profileKey = DefaultProfileKey)
        {
            InitPage(profileKey, "About The Data");
            return View("AboutData", PageModel);
        }

        [CheckUserCanAccessSkin]
        public ActionResult MortalityRankings()
        {
            var profileKey = "mortality";
            InitPage(profileKey, "Mortality Rankings");
            return GetProfileView(profileKey, "Rankings");
        }

        [CheckUserCanAccessSkin]
        public ActionResult MortalityAreaDetails()
        {
            var profileKey = "mortality";
            InitPage(profileKey, "Mortality Rankings");
            return GetProfileView(profileKey, "AreaDetails");
        }

        [CheckUserCanAccessSkin]
        public ActionResult HealthIntervention(string intervention)
        {
            InitPage(ProfileUrlKeys.LongerLives, "Health Interventions");
            ViewBag.Intervention = intervention;
            return View("Mortality/MortalityHealthIntervention" + intervention.ToLower(), PageModel);
        }

        [CheckUserCanAccessSkin]
        public ActionResult PracticeRankings(string profileKey)
        {
            InitPage(profileKey, "Practice List");
            return GetProfileView(profileKey, "Rankings");
        }

        [CheckUserCanAccessSkin]
        public ActionResult PracticeDetails(string profileKey)
        {
            InitPage(profileKey, "Practice Details");
            return GetProfileView(profileKey, "PracticeDetails");
        }

        [CheckUserCanAccessSkin]
        public ActionResult AreaDetails(string profileKey)
        {
            InitPage(profileKey, "Area Details");
            return View("Diabetes/DiabetesAreaDetails", PageModel);
        }

        [CheckUserCanAccessSkin]
        public ActionResult Connect(string profileKey = DefaultProfileKey)
        {
            InitPage(profileKey, "Connect");
            return GetProfileView(profileKey, "Connect");
        }

        [CheckUserCanAccessSkin]
        public ActionResult AreaSearchResults(string profileKey = DefaultProfileKey)
        {
            var parameters = Request.QueryString;
            ViewBag.Area = parameters["place_name"];
            ViewBag.Easting = parameters["easting"];
            ViewBag.Northing = parameters["northing"];
            InitPage(profileKey, "Search Results");
            return GetProfileView(profileKey, "AreaSearchResults");
        }

        [CheckUserCanAccessSkin]
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

        [CheckUserCanAccessSkin]
        public void InitPage(string profileKey, string pageName)
        {
            var profileDetails = new ProfileDetailsBuilder(profileKey).Build();
            ConfigureWithProfile(profileDetails);
            PageModel.PageTitle = pageName;
        }

        private void SetMetaTagContent()
        {
            ViewBag.MetaDescription = ContentHelper.RemoveHtmlTags(
                profileReader.GetContentItem(ContentKeys.MetaDescription, ProfileIds.LongerLives).Content);

            ViewBag.MetaKeywords = ContentHelper.RemoveHtmlTags(
                profileReader.GetContentItem(ContentKeys.MetaKeywords, ProfileIds.LongerLives).Content);
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

            ConfigureWithProfile(new ProfileDetailsBuilder(DefaultProfileKey).Build());
            PageModel.PageTitle = title;
            ViewBag.ErrorMessage = message;
            return View("../LongerLives/Error", PageModel);
        }

        public ActionResult Policy(string policyType)
        {
            ConfigureWithProfile(new ProfileDetailsBuilder(DefaultProfileKey).Build());

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
    }
}
