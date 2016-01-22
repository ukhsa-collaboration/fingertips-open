using System;
using System.Net;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.Models;

namespace Fpm.MainUI.Controllers
{
    public class ErrorController : Controller
    {
        private const string ViewName = "Error";

        private Error error = new Error();

        public ActionResult Http404(string url)
        {
            ExceptionLogger.LogException(new Exception("Page not found"), Request.Url.AbsoluteUri);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

            error = new Error { Message = "Sorry, that page could not be found..." };

            return View(ViewName, error);
        }

        public ActionResult Http500(Exception exception)
        {
            ExceptionLogger.LogException(exception,System.Web.HttpContext.Current.Request.Url.AbsoluteUri);

            HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            if (exception != null)
            {
                error = new Error
                            {
                                Message = "Sorry, an unexpected error has occured..." 
                            };
            }

            return View(ViewName, error);
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