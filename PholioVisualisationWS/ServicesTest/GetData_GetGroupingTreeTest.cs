using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class GetData_GetGroupingTreeTest
    {
        [TestMethod]
        public void TestGetGroupingTree()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetData.ashx?s=sg&gid=" + GroupIds.PracticeProfiles_PracticeSummary + ",3000010");
            TestHelper.IsData(data);

            string s = Encoding.Default.GetString(data);
            Assert.IsTrue(s.Contains("Summary"));
        }
    }
}
