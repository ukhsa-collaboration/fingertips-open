using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DataAccess;

namespace IndicatorsUI.MainUITest
{
    [TestClass]
    public class SwaggerBridgeControllerTest
    {
        [TestMethod]
        public void Test_Landing_Page()
        {
            AssertDataIsReturned("api");
        }

        [TestMethod]
        public void Test_JS_File_Returned()
        {
            AssertDataIsReturned("api/asset/swagger-ui-min-js");
        }

        [TestMethod]
        public void Test_Css_File_Returned()
        {
            AssertDataIsReturned("api/asset/css/reset-css");
        }

        [TestMethod]
        public void Test_Service_Details_Returned()
        {
            AssertDataIsReturned("swagger/docs/v1");
        }

        private void AssertDataIsReturned(string path)
        {
            var url = AppConfig.Instance.BridgeWsUrl + path;
            var data = new WebClient().DownloadData(url);
            Assert.AreNotEqual(0, data.Length);
        }
    }
}
