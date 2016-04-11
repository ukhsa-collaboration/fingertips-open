using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Profiles.DataAccess;
using Profiles.MainUI.Helpers;
using Profiles.MainUI.Controllers;
using Profiles.MainUI.Routes;

namespace Profiles.MainUI
{
    public class MvcApplication : HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            string filePath = AppConfig.Instance.ExceptionLogFilePath;
            if (string.IsNullOrEmpty(filePath) == false)
            {
                // Log to local file if log file path is defined in web.config
                new FileLogger(filePath).WriteException(exception);
            }
            else
            {
                // Log by web service to staging pholio database
                ExceptionLogger.LogException(exception, "Global.asax");
            }
            
            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "Http500");
            routeData.Values.Add("exception", exception);

            // Clear the error on server.
            Server.ClearError();

            // Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;

            // Call target Controller and pass the routeData.
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(
                 new HttpContextWrapper(Context), routeData));
        }
    }
}