using System;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace IndicatorsUITest
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
            var json = GetJson("data/pdf/spine_chart?" +
                "profile_id=" + ProfileIds.Phof +
                "&area_codes=" + AreaCodes.Derby +
                "&child_area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority);

            Assert.IsTrue(json.Contains("IndicatorId"),
                "Both IndicatorsUI and PholioVisualisationWS need to be running for this to work");
        }

        private string GetJson(string path)
        {
            var data = new WebClient().DownloadData(AppConfig.Instance.BridgeWsUrl + path);
            return Encoding.Default.GetString(data);
        }
    }
}
