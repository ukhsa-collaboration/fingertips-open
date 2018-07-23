using IndicatorsUI.DataAccess;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.MainUI.Models;
using IndicatorsUI.MainUI.Skins;
using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IndicatorsUI.MainUI.Controllers
{
    public class ErrorController : BaseController
    {
        private const string ViewName = "Error";

        private ErrorPageModel errorPageModel;

        public ErrorController(IAppConfig appConfig) : base(appConfig)
        {

        }

        protected override void NewPageModel()
        {
            errorPageModel = new ErrorPageModel(AppConfig.Instance);
            PageModel = errorPageModel;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Called here because the Request object is not available in the constructor
            InitPageModel();
        }

        [Route("access-not-allowed")]
        public ActionResult AccessNotAllowed()
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            PageModel.PageTitle = "Access denied";
            errorPageModel.Error = new Error { Message = "You do not have permission to view this page" };
            return View(ViewName, errorPageModel);
        }

        public ActionResult Http404(string url)
        {
            ExceptionLogger.LogException(new Exception("Page not found"), url);

            HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

            if (SkinFactory.GetSkin().Name == SkinNames.Mortality)
            {
                return new LongerLivesController(ReaderFactory.GetProfileReader(), _appConfig).Get404Error();
            }

            PageModel.PageTitle = "Page not found";
            errorPageModel.Error = new Error { Message = "Sorry, that page could not be found..." };
            return View(ViewName, errorPageModel);
        }

        public ActionResult Http500(Exception exception)
        {
            ExceptionLogger.LogException(exception, System.Web.HttpContext.Current.Request.Url.AbsoluteUri);

            HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (SkinFactory.GetSkin().Name == SkinNames.Mortality)
            {
                return new LongerLivesController(ReaderFactory.GetProfileReader(), _appConfig).Get500Error();
            }

            PageModel.PageTitle = "Unexpected Error";

            errorPageModel.Error = new Error
            {
                Message = "Sorry, an unexpected error has occured..."
            };

            return View(ViewName, errorPageModel);
        }

        [Route("browser-not-supported")]
        public ActionResult BrowserNotSupported()
        {
            return View("BrowserNotSupported");
        }


        public static ActionResult InvokeHttp404(HttpContextBase httpContext)
        {
            IController errorController = new ErrorController(AppConfig.Instance);
            var errorRoute = new RouteData();
            errorRoute.Values.Add("controller", "Error");
            errorRoute.Values.Add("action", "Http404");
            errorRoute.Values.Add("url", httpContext.Request.Url.OriginalString);
            errorController.Execute(new RequestContext(
                 httpContext, errorRoute));

            return new EmptyResult();
        }
    }
}