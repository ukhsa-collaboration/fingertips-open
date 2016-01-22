using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using FingertipsBridgeWS.Cache;

namespace FingertipsBridgeWS.Services
{
    public class PostHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add("user-agent", AppConfiguration.UserAgent);

            // Remove preceeding path, e.g. "www/ws/Update.ashx" -> "Update.ashx"
            string service = Path.GetFileName(context.Request.Path);

            string url = AppConfiguration.CoreWsUrlForAjaxBridge + "/" + service;

            NameValueCollection relevantParameters = new NameValueCollection();

            //TODO prepare relevant parameters to send to Core WS, then uncomment below
            /*
            byte[] json = wc.UploadValues(url, relevantParameters);

            context.Response.ContentType = ServiceBridge.ContentTypeJson;
            if (json.Length > 0)
            {
                context.Response.BinaryWrite(json);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                new DatabaseLogger().LogException(new Exception("Request to CoreWs received empty response"), url);
            }
             */

            context.Response.Flush();
        }

    }
}