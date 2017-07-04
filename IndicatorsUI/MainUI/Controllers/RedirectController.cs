using System.Web.Mvc;

namespace Profiles.MainUI.Controllers
{
    public class RedirectController : Controller
    {
        public ActionResult RedirectToUrl(string newUrl)
        {
            return Redirect(newUrl);
        }
    }
}