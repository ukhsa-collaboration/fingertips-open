using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.MainUI.Models;

namespace IndicatorsUI.MainUI.Controllers
{
    public class AreaSearchController : BaseController
    {
        public AreaSearchController(IAppConfig appConfig) : base(appConfig)
        {
            
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Called here because the Request object is not available in the constructor
            InitPageModel();
        }

        public ActionResult AreaSearchResults(string profileKey, string areaCodeList, string search_type, 
            string place_name = null, string leadProfileUrlKey = null)
        {
            var details = ConfigureFingertipsProfileAndPageModelWithProfileDetails(profileKey);

            ViewBag.AreaCodes = areaCodeList;

            ViewBag.PlaceNameToDisplay = place_name;

            if (place_name != null)
            {
                // Replace apostrophe followed by s (e.g. Acock's green => Acock green)
                ViewBag.PlaceName = place_name.ToLower().Replace("'s", " ");
            }
            
            ViewBag.SearchType = search_type;
            ViewBag.AreasToIgnoreEverywhere = details.AreasToIgnoreEverywhere;
            PageModel.PageType = PageType.AreaSearchResultsOfProfileWithFrontPage;

            if (string.IsNullOrEmpty(leadProfileUrlKey) == false)
            {
                SetProfileCollection(details, leadProfileUrlKey);
            }

            PageModel.PageTitle = details.Title;
            PageModel.DisplayProfileTitle = true;

            return View(PageModel);
        }

    }
}