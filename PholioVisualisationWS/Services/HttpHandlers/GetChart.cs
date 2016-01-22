
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using PholioVisualisation.Cache;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services.HttpHandlers
{
    public class GetChart : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            NameValueCollection parameters = context.Request.Params;

            var practiceChartParameters = new PracticeChartParameters(parameters);
            BaseChart chart = new PracticeChart { Parameters = practiceChartParameters };

            HttpResponse response = context.Response;
            response.ContentType = "image/png";
            MemoryStream stream = null;
            if (chart != null)
            {
                stream = chart.GetChart();

                if (stream != null)
                {
                    // Content-length required by Chrome
                    response.AddHeader("content-length", stream.Length.ToString());

                    CachePolicyHelper.SetMidnightWebCache(response);
                    response.Flush();

                    response.BinaryWrite(stream.ToArray());
                }
            }

            response.Flush();
            response.Close();

            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
                stream = null;
            }

            // Never call Dispose on chart - it prevents any more charts being created
        }
    }
}