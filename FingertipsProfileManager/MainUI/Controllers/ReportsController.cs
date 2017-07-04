using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        [Route("reports")]
        public ActionResult Index()
        {
            return View();
        }
    }
}