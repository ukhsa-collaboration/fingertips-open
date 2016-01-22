using System;
using System.Collections.Generic;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.ExceptionLogging;

namespace ServicesWeb.Controllers
{
    public class BaseController : ApiController
    {
        public const int UndefinedProfileId = -1;

        protected List<int> ParseIntList(string intListString)
        {
            return new IntListStringParser(intListString).IntList;
        }

        protected void Log(Exception ex)
        {
            string url = string.Empty;
            try
            {
                url = Request.RequestUri.AbsoluteUri;
            }
            catch { }
            ExceptionLog.LogException(ex, url);
        }

    }
}