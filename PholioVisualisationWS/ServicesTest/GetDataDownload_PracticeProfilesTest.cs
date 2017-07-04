using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class GetDataDownload_PracticeProfilesTest
    {
        /// <summary>
        /// Asserts population data can be downloaded where the area is a CCG
        /// </summary>
        [TestMethod]
        public void TestDownloadPracticeProfilesPopulationDataCcg()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetData.ashx?s=db&ati=19&pro=qp&gid=" + GroupIds.PracticeProfiles_SupportingIndicators +
                "&are=" + AreaCodes.Ccg_Kernow);
            Assert.AreNotEqual(0, data.Length);
        }

        /// <summary>
        /// Asserts population data can be downloaded where the area is a practice
        /// </summary>
        [TestMethod]
        public void TestDownloadPracticeProfilesPopulationDataPractice()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetData.ashx?s=db&ati=19&pro=qp&gid=" + GroupIds.PracticeProfiles_SupportingIndicators +
                "&are=" + AreaCodes.Gp_KingStreetBlackpool);
            Assert.AreNotEqual(0, data.Length);
        }
    }
}
