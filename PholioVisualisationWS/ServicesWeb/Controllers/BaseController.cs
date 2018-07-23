using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.ExceptionLogging;
using PholioVisualisation.Parsers;

namespace PholioVisualisation.ServicesWeb.Controllers
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
            // Will not have Request object if running unit test outside web context
            string url = string.Empty;
            try
            {
                url = Request.RequestUri.AbsoluteUri;
                ExceptionLog.LogException(ex, url);
            }
            catch { }
        }


        /// <summary>
        /// Parses a list of comma-separated profile IDs. If the list is empty then use all 
        /// available searchable IDs.
        /// </summary>
        public IList<int> GetProfileIds(string profile_ids)
        {
            IList<int> profileIds = new IntListStringParser(profile_ids).IntList;
            if (profileIds.Any() == false)
            {
                profileIds = new ProfileIdListProvider(ReaderFactory.GetProfileReader()).GetSearchableProfileIds();
            }
            return profileIds;
        }
    }
}