using System;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUITest.Controllers
{
    /// <summary>
    /// NOTE: The web application must be running before these tests can pass.
    /// </summary>
    [TestClass]
    public class AjaxBridgeTest
    {
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
