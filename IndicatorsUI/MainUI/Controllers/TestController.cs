using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUI.Controllers
{
    public class TestController : Controller
    {
        protected AppConfig appConfig = AppConfig.Instance;

        [Route("test")]
        public ActionResult Index()
        {
            ViewBag.BridgeServicesUrl = appConfig.BridgeWsUrl;
            ViewBag.JsPath = SetJavaScriptVersionFolder();

            return View();
        }

        [Route("test/{page}")]
        public ActionResult TestPage(string page)
        {
            string viewName = null;

            switch (page)
            {
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

        [Route("test/error")]
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
