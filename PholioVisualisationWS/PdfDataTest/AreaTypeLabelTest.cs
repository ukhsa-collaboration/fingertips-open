using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfDataTest
{
    [TestClass]
    public class AreaTypeLabelTest
    {
        [TestMethod]
        public void TestGetLabelFromAreaCode_County()
        {
            LabelAsExpected(AreaCodes.CountyUa_Cambridgeshire, AreaTypeLabel.County);
        }

        [TestMethod]
        public void TestGetLabelFromAreaCode_UnitaryAuthority()
        {
            LabelAsExpected(AreaCodes.CountyUa_CentralBedfordshire, AreaTypeLabel.UnitaryAuthority);
        }

        [TestMethod]
        public void TestGetLabelFromAreaCode_LondonBorough()
        {
            LabelAsExpected(AreaCodes.CountyUa_CityOfLondon, AreaTypeLabel.UnitaryAuthority);
        }

        [TestMethod]
        public void TestGetLabelFromAreaCode_CountyDistrict()
        {
            LabelAsExpected(AreaCodes.La_Wychavon, AreaTypeLabel.District);
        }

        [TestMethod]
        public void TestGetLabelFromAreaCode_MetropolitanCounty()
        {
            LabelAsExpected(AreaCodes.CountyUa_Manchester, AreaTypeLabel.UnitaryAuthority);
        }

        private static void LabelAsExpected(string areaCode, string label)
        {
            Assert.AreEqual(label, AreaTypeLabel.GetLabelFromAreaCode(areaCode));
        }
    }
}
