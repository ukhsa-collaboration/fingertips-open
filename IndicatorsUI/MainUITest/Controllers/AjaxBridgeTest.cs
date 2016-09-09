using System;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace IndicatorsUI.MainUITest.Controllers
{
    /// <summary>
    /// NOTE: The web application must be running before these tests can pass.
    /// </summary>
    [TestClass]
    public class AjaxBridgeTest
    {
        [TestMethod]
        public void TestGetPdfSpineChartData()
        {
            var json = GetJson("api/pdf/spine_chart?" +
                "profile_id=" + ProfileIds.Phof +
                "&area_codes=" + AreaCodes.Derby +
                "&child_area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority);

            Assert.IsTrue(json.Contains("IndicatorId"),
                "Both IndicatorsUI and PholioVisualisationWS need to be running for this to work");
        }

        [TestMethod]
        public void Test_Data_Can_Be_Used_Instead_Of_Api_For_Backwards_Compatibility()
        {
            var json = GetJson("data/value_notes");
            Assert.IsTrue(json.Contains("Value missing"));
        }

        [TestMethod]
        public void Test_Api_Call()
        {
            var json = GetJson("api/value_notes");
            Assert.IsTrue(json.Contains("Value missing"));
        }

        private string GetJson(string path)
        {
            var url = AppConfig.Instance.BridgeWsUrl + path;
            var data = new WebClient().DownloadData(url);
            return Encoding.Default.GetString(data);
        }
    }
}
