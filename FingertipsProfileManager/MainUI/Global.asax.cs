using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Fpm.MainUI.Controllers;
using Fpm.MainUI.Helpers;

namespace Fpm.MainUI
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutoMapperConfig.RegisterMappings();
        }

        private static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            // Log error here in case problem in ErrorController or dependencies
            try
            {
                ExceptionLogger.LogException(exception, "Global.asax");
            }
            catch (Exception ex)
            {
                WriteExceptionToFile(exception, ex);
            }

            var routeData = new RouteData();
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

        private static void WriteExceptionToFile(Exception exception, Exception ex)
        {
            StreamWriter writer = null;
            try
            {
                writer = File.CreateText(AppConfig.ErrorFile);
                if (exception != null)
                {
                    writer.WriteLine(exception.Message + Environment.NewLine + (exception.StackTrace ?? ""));
                }
                if (ex != null)
                {
                    writer.WriteLine(ex.Message + Environment.NewLine + (ex.StackTrace ?? ""));
                }
            }
            catch
            {
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }
    }
}