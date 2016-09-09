using System.Web.Mvc;
using Profiles.MainUI.Filters;
using Profiles.MainUI.Models;

namespace Profiles.MainUI.Controllers
{
    public class AreaSearchController : BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Called here because the Request object is not available in the constructor
            InitPageModel();
        }

        [CheckUserCanAccessSkin]
        public ActionResult AreaSearchResults(string profileKey, string areaCodeList, string search_type, 
            string place_name = null, string leadProfileUrlKey = null)
        {
            var details = ConfigureFingertipsProfileAndPageModelWithProfileDetails(profileKey);

            ViewBag.AreaCodes = areaCodeList;
            ViewBag.PlaceName = place_name;
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