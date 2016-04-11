
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class GetData_IndicatorStatsTest
    {
        [TestMethod]
        public void TestGetIndicatorStats()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetData.ashx?s=is&gid=" + GroupIds.PracticeProfiles_PracticeSummary + "&ati=7&&off=0&par=" + AreaCodes.England);
            TestHelper.IsData(data);

            string s = Encoding.Default.GetString(data);
            Assert.IsTrue(s.Contains("Stats"));
        }
    }
}
