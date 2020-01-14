using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using IndicatorsUI.DataAccess;

namespace IndicatorsUI.MainUI.Controllers
{
    public class UserFeedbackController : Controller
    {
        [HttpPost]
        [Route("user_feedback")]
        public ActionResult Submit(string url, string whatUserWasDoing, string whatWentWrong, string email)
        {

            if (!string.IsNullOrEmpty(url) && string.IsNullOrEmpty(whatUserWasDoing) &&
                string.IsNullOrEmpty(whatWentWrong))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("parameters cannot be empty.");
            }


            var apiUrl = AppConfig.Instance.CoreWsUrlForLogging + "/api/user_feedback";
            var webRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    Url = url,
                    WhatUserWasDoing = whatUserWasDoing,
                    WhatWentWrong = whatWentWrong,
                    Email = email,
                    Environment = AppConfig.Instance.Environment
                });

                streamWriter.Write(json);
            }

            var response = (HttpWebResponse)webRequest.GetResponse();
            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                var result = stream.ReadToEnd();
            }

            return null;
        }


    }
}