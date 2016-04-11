
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class GetData_AreaDataTest
    {
        [TestMethod]
        public void TestGetAreaDataForPractice()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetData.ashx?s=ad&gid=" + GroupIds.PracticeProfiles_PracticeSummary + 
                "&ati=" + AreaTypeIds.GpPractice +
                "&are=" + AreaCodes.Gp_Addingham +
                "&com=" + AreaCodes.England +"," + AreaCodes.Ccg_AireDaleWharfdaleAndCraven);

            AssertDataOk(data);
        }

        [TestMethod]
        public void TestGetAreaDataForCcg()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetData.ashx?s=ad&gid=" + GroupIds.PracticeProfiles_PracticeSummary + 
                "&ati=" + AreaTypeIds.GpPractice +
                "&are=" + AreaCodes.Ccg_Farnham + 
                "&com=" + AreaCodes.England +
                "&pyr=2011");

            AssertDataOk(data);
        }

        [TestMethod]
        public void TestGetDiabetesDataForPractice()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetData.ashx?s=ad&gid=" + GroupIds.Diabetes_TreatmentTargets +
                "&ati=" + AreaTypeIds.GpPractice +
                "&are=" + AreaCodes.Gp_Addingham + 
                "&com=" + AreaCodes.England);

            AssertDataOk(data);
        }

        private static void AssertDataOk(byte[] data)
        {
            TestHelper.IsData(data);

            string s = Encoding.Default.GetString(data);
            Assert.IsTrue(s.Contains("IID"));
        }
    }
}
