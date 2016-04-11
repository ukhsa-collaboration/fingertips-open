using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class PdfDataTest
    {
        [TestMethod]
        public void TestPdfSpineChart()
        {
            byte[] data = GetData("spine_chart?profile_id=" + ProfileIds.Phof +
                "&area_codes=" + AreaCodes.CountyUa_Buckinghamshire + "&child_area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestNationalValuesForPdfChart()
        {
            byte[] data = GetData("national_values?profile_id=" + ProfileIds.Phof
                + "&child_area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestPdfSupportingInformation_HealthProfiles()
        {
            byte[] data = GetData("supporting_information?profile_id=" + ProfileIds.HealthProfiles +
                "&area_code=" + AreaCodes.CountyUa_Buckinghamshire);
            TestHelper.IsData(data);
        }

        private byte[] GetData(string path)
        {
            var url = TestHelper.BaseUrl + "data/pdf/" + path;
            return new WebClient().DownloadData(url);
        }

    }
}
