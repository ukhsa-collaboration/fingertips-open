using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace ServicesWeb.Controllers
{
    public class LogController : BaseController
    {
        /// <summary>
        /// Logs an exception that can be viewed in FPM
        /// </summary>
        /// <param name="exception">Serialised exception taken from the request body</param>
        [HttpPost]
        [Route("log/exception")]
        public void Exception([FromBody]string exception)
        {
            Dictionary<string, object> exceptionDetails = new JavaScriptSerializer()
                .Deserialize<Dictionary<string, object>>(exception);

            ReaderFactory.GetExceptionWriter().Save(new ExceptionForStorage
            {
                Application = GetProperty(exceptionDetails, "application"),
                Date = DateTime.Now,
                Environment = GetProperty(exceptionDetails, "environment"),
                Message = GetProperty(exceptionDetails, "message"),
                Server = GetProperty(exceptionDetails, "server"),
                StackTrace = GetProperty(exceptionDetails, "stackTrace"),
                Type = GetProperty(exceptionDetails, "type"),
                Url = GetProperty(exceptionDetails, "url"),
                UserName = GetProperty(exceptionDetails, "username")
            });
        }

        private static string GetProperty(Dictionary<string, object> properties, string name)
        {
            var property = properties[name];
            return property == null ? string.Empty : property.ToString();
        }
    }
}
