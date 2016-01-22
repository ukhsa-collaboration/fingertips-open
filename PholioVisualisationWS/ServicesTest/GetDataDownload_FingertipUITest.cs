using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace ServicesTest
{
    [TestClass]
    public class GetDataDownload_FingertipUITest
    {
        /// <summary>
        ///     Regression test for specific area: eastmidlands.
        /// </summary>
        [TestMethod]
        public void TestDownloadExcelFileEastMidlands()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl + "GetDataDownload.ashx?" +
                "pid=" + ProfileIds.HealthProfiles +
                "&ati=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&tem=" + ProfileIds.HealthProfiles +
                "&par=" + AreaCodes.Gor_EastMidlands +
                "&pds=0&pat=" + AreaTypeIds.GoRegion);

            Assert.AreNotEqual(0, data.Length);
        }

        [TestMethod]
        public void TestDownloadExcelSearchResultsWithUnrestrictedProfileId()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetDataDownload.ashx?pid=" + ProfileIds.Search +
                "&ati=102&iids=41201,41202,41203,41204,41401,41402,41403&tem=1&par=" +
                AreaCodes.Sha_EastOfEngland + "&pds=0&pat=6");

            Assert.AreNotEqual(0, data.Length);
        }
    }
}