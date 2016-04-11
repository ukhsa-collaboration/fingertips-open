using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Script.Serialization;
using Profiles.DataAccess;

namespace Profiles.MainUI.Helpers
{
    public static class ExceptionLogger
    {
        public static void LogException(Exception ex, string url)
        {
            if (ex != null)
            {
                string jsonException = new JavaScriptSerializer().Serialize(new
                {
                    application = AppConfig.Instance.ApplicationName,
                    username = "IndicatorsUI",
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    type = ex.GetType().FullName,
                    url = ((object) url ?? DBNull.Value),
                    environment = "",
                    server = Environment.MachineName
                });

                CallLogService(jsonException, "exception");
            }
        }

        public static void LogClientSideException(string errorMessage)
        {
            string jsonException = new JavaScriptSerializer().Serialize(new
            {
                application = AppConfig.Instance.ApplicationName,
                username = "IndicatorsUI",
                message = errorMessage,
                stackTrace = string.Empty,
                type = "Javascript Error",
                url = HttpContext.Current.Request.UrlReferrer.ToString(),
                environment = "",
                server = Environment.MachineName
            });

            CallLogService(jsonException, "exception");
        }

        public static void LogExport(string areaCode, string profileId, string downloadType)
        {
            string jsonException = new JavaScriptSerializer().Serialize(new
            {
                areaCode, profileId, downloadType
            });

            CallLogService(jsonException, "export");
        }

        private static void CallLogService(string jsonException, string serviceAction)
        {
            var logger = WebRequest.Create(AppConfig.Instance.CoreWsUrlForLogging + "log/" + serviceAction) as HttpWebRequest;
            logger.ContentType = "application/json";
            logger.Method = "POST";

            var exception = ConvertJsonToByteArray(jsonException);
            logger.ContentLength = exception.Length;
            using (var postStream = logger.GetRequestStream())
            {
                postStream.Write(exception, 0, exception.Length);
            }
        }

        static byte[] ConvertJsonToByteArray(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(string));
            var ms = new MemoryStream();
            serializer.WriteObject(ms, json);
            ms.Position = 0;
            new StreamReader(ms);
            return ms.ToArray();
        }
    }
}