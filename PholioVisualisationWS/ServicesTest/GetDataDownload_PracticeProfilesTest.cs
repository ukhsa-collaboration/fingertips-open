using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace ServicesTest
{
    [TestClass]
    public class GetDataDownload_PracticeProfilesTest
    {
        /// <summary>
        /// Asserts topic data can be downloaded where the area is a CCG
        /// </summary>
        [TestMethod]
        public void TestDownloadPracticeProfilesTopicDataCcg()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetData.ashx?s=db&ati=19&are=" + AreaCodes.Ccg_Kernow +
                "&pro=pp&gid=" + GroupIds.PracticeProfiles_Cvd + "&lyr=2011");
            Assert.AreNotEqual(0, data.Length);
        }

        /// <summary>
        /// Asserts topic data can be downloaded where the area is a practice
        /// </summary>
        [TestMethod]
        public void TestDownloadPracticeProfilesTopicDataPractice()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetData.ashx?s=db&ati=19&are=" + AreaCodes.Gp_MonkfieldCambourne +
                "&pro=pp&gid=" + GroupIds.PracticeProfiles_Cvd + "&lyr=2011");
            Assert.AreNotEqual(0, data.Length);
        }

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
