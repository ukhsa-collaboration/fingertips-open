using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Profiles.DataAccess;
using Profiles.MainUI.Common;
using Profiles.MainUI.Models;
using Profiles.MainUI.Skins;

namespace Profiles.MainUI.Controllers
{
    public class ErrorController : BaseController
    {
        private const string ViewName = "Error";

        private ErrorPageModel errorPageModel;

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
                return new LongerLivesController().Get404Error();
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
                return new LongerLivesController().Get500Error();
            }

            PageModel.PageTitle = "Unexpected Error";

            errorPageModel.Error = new Error
            {
                Message = "Sorry, an unexpected error has occured..."
            };

            return View(ViewName, errorPageModel);
        }


        public static ActionResult InvokeHttp404(HttpContextBase httpContext)
        {
            IController errorController = new ErrorController();
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