
using System.Web.Mvc;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace Profiles.MainUI.Controllers
{
    public class TestController : Controller
    {
        protected AppConfig appConfig = AppConfig.Instance;
        public ActionResult Index()
        {
            ViewBag.BridgeServicesUrl = appConfig.BridgeWsUrl;
            ViewBag.JsPath = SetJavaScriptVersionFolder();

            return View();
        }

        public ActionResult TestPage(string page)
        {
            string viewName = null;

            switch (page)
            {
                case "practice-profiles":
                    viewName = "PracticeProfilesTest";
                    break;

                case "fingertips":
                    viewName = "FingertipsTest";
                    break;

                case "diabetes":
                    viewName = "DiabetesTest";
                    break;

                case "diabetes-rankings":
                    viewName = "DiabetesRankingsTest";
                    break;

                default:
                    throw new FingertipsException("Test view is not defined");
            }

            return View(viewName);
        }

        public void TestError()
        {
            throw new FingertipsException("This exception was deliberately thrown in a test");
        }

        private string SetJavaScriptVersionFolder()
        {
            var url = appConfig.StaticContentUrl + appConfig.JavaScriptVersionFolder;
            return url + "js/";
        }

    }
}
