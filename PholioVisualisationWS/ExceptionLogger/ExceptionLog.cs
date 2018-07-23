using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.ExceptionLogging
{
    public class ExceptionLog
    {
        public static void LogException(Exception ex, string url)
        {
            string filePath = ApplicationConfiguration.Instance.ExceptionLogFilePath;
            if (string.IsNullOrEmpty(filePath) == false)
            {
                // Log to local file if log file
                new FileLogger(filePath).WriteException(ex);
            }
            else
            {
                LogToRemoteEndpoint(ex, url);
            }
        }

        private static void LogToRemoteEndpoint(Exception ex, string url)
        {
            string jsonException = new JavaScriptSerializer().Serialize(new
            {
                application = ApplicationConfiguration.Instance.ApplicationName,
                username = "PholioVisualisationWS",
                message = ex.Message,
                stackTrace = ex.StackTrace,
                type = ex.GetType().FullName,
                url = ((object) url ?? DBNull.Value),
                environment = "",
                server = Environment.MachineName
            });

            var logger = WebRequest.Create(ApplicationConfiguration.Instance.CoreWsUrlForLogging + "log/exception") as HttpWebRequest;
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