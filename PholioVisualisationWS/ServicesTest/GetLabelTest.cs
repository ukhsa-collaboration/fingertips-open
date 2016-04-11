
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class GetLabelTest
    {
        [TestMethod]
        public void TestAllLabels()
        {
            WebClient wc = new WebClient();
            string json = wc.DownloadString(TestHelper.BaseUrl + "GetLabel.ashx?age=1&yti=2&key=3");
            Assert.IsTrue(json.Contains("Financial"));
            Assert.IsTrue(json.Contains("All ages"));
        }

        [TestMethod]
        public void TestYearTypeOnly()
        {
            WebClient wc = new WebClient();
            string json = wc.DownloadString(TestHelper.BaseUrl + "GetLabel.ashx?yti=2&key=4");
            Assert.IsTrue(json.Contains("Financial"));
        }

        [TestMethod]
        public void TestAgeOnly()
        {
            WebClient wc = new WebClient();
            string json = wc.DownloadString(TestHelper.BaseUrl + "GetLabel.ashx?age=1&key=5");
            Assert.IsTrue(json.Contains("All ages"));
        }
    }
}
