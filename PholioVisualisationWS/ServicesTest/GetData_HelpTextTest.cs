using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class GetData_HelpTextTest
    {
        [TestMethod]
        public void TestGetHelpText()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetData.ashx?s=ht&key=timeline");
            TestHelper.IsData(data);

            string s = Encoding.Default.GetString(data);

            // Assert text OK
            Assert.IsTrue(s.Contains("<p>"));

            // Assert key also returned
            Assert.IsTrue(s.Contains("timeline"));
        }
    }
}
