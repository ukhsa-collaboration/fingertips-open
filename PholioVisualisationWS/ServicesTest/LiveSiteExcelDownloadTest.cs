using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class LiveSiteExcelDownloadTest
    {
        [TestMethod]
        public void Test_Phof_Live_A()
        {
            var url = "https://livews-a.phe.org.uk/";
            AssertDataWasDownloaded(url);
        }

        [TestMethod]
        public void Test_Phof_Live_B()
        {
            var url = "https://livews-b.phe.org.uk/";
            AssertDataWasDownloaded(url);
        }

        private static void AssertDataWasDownloaded(string url)
        {
            byte[] data = new WebClient().DownloadData(url + "GetDataDownload.ashx?" +
                                                       "pid=" + ProfileIds.Phof +
                                                       "&ati=" + AreaTypeIds.CountyAndUnitaryAuthority +
                                                       "&tem=" + ProfileIds.Phof +
                                                       "&par=" + AreaCodes.England +
                                                       "&pds=0&pat=" + AreaTypeIds.GoRegion);

            Assert.AreNotEqual(0, data.Length);
        }
    }
}
