using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class LogExceptionTest
    {
        [TestMethod]
        public void TestLogException()
        {
            //This integration test logs a fake exception to the database.
            //Ideally this should use a mock'd interface but we don't have this setup so it's just doing a write-and-forget for now as a test stub.
            string jsonException = new JavaScriptSerializer().Serialize(new
            {
                application = "Test - WS Services Test",
                username = "Test - Username",
                message = "Test - This is a test message",
                stackTrace = "Test - This is the stack trace",
                type = "Test - This is the type",
                url = "Test - This is the URL",
                environment = "Test - This is the environment",
                server = Environment.MachineName
            });

            var request = WebRequest.Create(ConfigurationManager.AppSettings["BaseUrl"] +
                "log/Exception") as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "POST";

            var exception = ConvertJsonToByteArray(jsonException);
            request.ContentLength = exception.Length;

            using (var postStream = request.GetRequestStream())
            {
                postStream.Write(exception, 0, exception.Length);
            }
            
            var response = request.GetResponse();
            Assert.IsNotNull(response);
        }

        private byte[] ConvertJsonToByteArray(string json)
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
