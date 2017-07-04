using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

namespace FingertipsBridgeWS.Cache
{
    public static class ExceptionLogger
    {
        public static void LogException(Exception ex, string url)
        {
            string jsonException = new JavaScriptSerializer().Serialize(new
            {
                application = ConfigurationManager.AppSettings["ApplicationName"],
                username = "IndicatorsUI",
                message = ex.Message,
                stackTrace = ex.StackTrace,
                type = ex.GetType().FullName,
                url = ((object)url ?? DBNull.Value),
                environment = "",
                server = Environment.MachineName
            });

            CallLogService(jsonException);
        }

        private static void CallLogService(string jsonException)
        {
            var logger = WebRequest.Create(AppConfiguration.CoreWsUrlForLogging +
                "log/Exception") as HttpWebRequest;
            logger.ContentType = "application/json; charset=utf-8";
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