using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

namespace Fpm.MainUI.Helpers
{
    public static class ExceptionLogger
    {
        public static void LogException(Exception exception, string url)
        {
            var jsonException = GetExpectionString(exception, url);

            try
            {
                var path = ApplicationConfiguration.CoreWsUrlForLogging + "log/exception";
                var logger = WebRequest.Create(path) as HttpWebRequest;
                logger.ContentType = "application/json";
                logger.Method = "POST";

                var bytes = ConvertJsonToByteArray(jsonException);
                logger.ContentLength = bytes.Length;
                using (var postStream = logger.GetRequestStream())
                {
                    postStream.Write(bytes, 0, bytes.Length);
                    postStream.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string GetExpectionString(Exception exception, string url)
        {
            string jsonException = new JavaScriptSerializer().Serialize(new
            {
                application = ConfigurationManager.AppSettings["ApplicationName"],
                username = UserDetails.CurrentUser().Name,
                message = exception.Message,
                stackTrace = exception.StackTrace,
                type = exception.GetType().FullName,
                url = ((object) url ?? DBNull.Value),
                environment = "",
                server = Environment.MachineName
            });
            return jsonException;
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