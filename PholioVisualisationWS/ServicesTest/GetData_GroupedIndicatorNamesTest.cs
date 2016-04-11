using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class GetData_GroupedIndicatorNamesTest
    {
        [TestMethod]
        public void TestGetGroupedIndicatorNames()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl + "GetData.ashx?s=gi&gid=2000006,2000008");
            TestHelper.IsData(data);

            string s = Encoding.Default.GetString(data);
            Assert.IsTrue(s.Contains("COPD"));
            Assert.IsTrue(s.Contains("CKD"));
        }
    }
}
