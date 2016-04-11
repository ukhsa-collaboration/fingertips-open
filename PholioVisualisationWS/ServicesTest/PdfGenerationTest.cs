using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class PdfGenerationTest
    {
        public const string BasePath = "http://www.nepho.org.uk/maps/dev/pdfonthefly/index.php?";

        /// <summary>
        /// Codes used by the PDF generator to identify profiles
        /// </summary>
        private class PdfProfileIds
        {
            public const string HealthProfiles = "hp2";
            public const string Phof = "phof";
            public const string Tobacco = "tobacco";
            public const string CommunityMentalHealthProfiles = "cmhp";
            public const string SevereMentalIllness = "mh-severe";
            public const string CommonMentalHealthDisorders = "mh-common";
            public const string Neurology = "neurology";
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestPhof_County()
        {
            AssertPdf(PdfProfileIds.Phof, AreaCodes.CountyUa_Buckinghamshire,
                AreaCodes.Gor_SouthEast, AreaTypeIds.CountyAndUnitaryAuthority);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestTobacco_County()
        {
            AssertPdf(PdfProfileIds.Tobacco, AreaCodes.CountyUa_Buckinghamshire,
                AreaCodes.Gor_SouthEast, AreaTypeIds.CountyAndUnitaryAuthority);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestNeurology_Ccg()
        {
            AssertPdf(PdfProfileIds.Neurology, AreaCodes.Ccg_Barnet,
                AreaCodes.CommissioningRegionLondon, AreaTypeIds.Ccg);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestSevereMentalIllness_Ccg()
        {
            AssertPdf(PdfProfileIds.SevereMentalIllness, AreaCodes.Ccg_Barnet,
                AreaCodes.CommissioningRegionLondon, AreaTypeIds.Ccg);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestCommunityMentalHealthProfiles_Ccg()
        {
            AssertPdf(PdfProfileIds.CommunityMentalHealthProfiles, AreaCodes.Ccg_Barnet,
                AreaCodes.CommissioningRegionLondon, AreaTypeIds.Ccg);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestCommonMentalHealthDisorders_Ccg()
        {
            AssertPdf(PdfProfileIds.CommonMentalHealthDisorders, AreaCodes.Ccg_Barnet,
                AreaCodes.CommissioningRegionLondon, AreaTypeIds.Ccg);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestHealthProfiles_District()
        {
            AssertPdf(PdfProfileIds.HealthProfiles, AreaCodes.La_Hyndburn,
                AreaCodes.Gor_NorthWest, AreaTypeIds.DistrictAndUnitaryAuthority);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestHealthProfiles_UA()
        {
            AssertPdf(PdfProfileIds.HealthProfiles, AreaCodes.CountyUa_Newcastle,
                AreaCodes.Gor_NorthWest, AreaTypeIds.DistrictAndUnitaryAuthority);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestHealthProfiles_LondonBorough()
        {
            AssertPdf(PdfProfileIds.HealthProfiles, AreaCodes.La_Haringay,
                AreaCodes.Gor_London, AreaTypeIds.DistrictAndUnitaryAuthority);
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void TestHealthProfiles_County()
        {
            AssertPdf(PdfProfileIds.HealthProfiles, AreaCodes.CountyUa_Leicestershire,
                AreaCodes.Gor_EastMidlands, AreaTypeIds.CountyAndUnitaryAuthority);
        }

        private static void AssertPdf(string profileCode, string areaCode, string regionCode, int areaTypeId)
        {
            var path = BasePath + "f=" + profileCode +
                   "&area_code=" + areaCode +
                   "&region_code=" + regionCode +
                   "&child_area_type_id=" + areaTypeId
                   + "&clear_cache=silent";

            Console.WriteLine(path);
            var request = WebRequest.Create(path) as HttpWebRequest;
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/pdf";
            request.UserAgent = "PHE unit test";
            var response = request.GetResponse() as HttpWebResponse;

            Assert.IsTrue(response.ContentLength > 200000,
                "PDF is expected to be greater than 200kB but is " +
                response.ContentLength / 1000 + "kB" + Environment.NewLine +
                path);
        }
    }
}
