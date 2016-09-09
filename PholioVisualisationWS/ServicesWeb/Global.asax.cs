using PholioVisualisation.ExceptionLogging;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Http;
using ServicesWeb.Helpers;
using StackExchange.Profiling;

namespace ServicesWeb
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            //FIN-859 - Dependency injection registration commented out because does not work on live servers
            //IoC.Register();

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        // Uncomment these two methods to enable Mini Profiler


        protected void Application_BeginRequest()
        {
            // need to start one here in order to render out the UI
//            MiniProfiler.Start();
        }

        protected void Application_EndRequest()
        {
            //            // Use step to measure duration of code execution
            //            using (MiniProfiler.Current.Step("GetPageIndex"))
            //            {
            //                // Do something here
            //            }

//            MiniProfiler.Stop();
//            new MiniProfilerWriter().Write(MiniProfiler.Current);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var request = Context.Request;
            var exception = Context.Server.GetLastError();

            // Log by web service to staging pholio database
            string url = (request != null && request.RawUrl != null) ?
                request.RawUrl :
                "n/a";

            ExceptionLog.LogException(exception, url);

            Context.Server.ClearError();

            // Empty JSON is interpreted as indication of error by clients
            Response.Write("");
        }

    }
}
