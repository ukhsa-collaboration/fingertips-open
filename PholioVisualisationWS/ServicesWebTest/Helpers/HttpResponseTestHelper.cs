using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.ServicesWebTest.Helpers
{
    [TestClass]
    public class HttpResponseTestHelper
    {
        public static string GetStreamContent(HttpResponseMessage response)
        {
            var receiveStream = response.Content.ReadAsStreamAsync();
            var readStream = new StreamReader(receiveStream.Result, Encoding.UTF8);
            var content = readStream.ReadToEnd();
            return content;
        }
    }
}
