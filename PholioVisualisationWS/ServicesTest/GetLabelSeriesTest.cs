
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServicesTest
{
    [TestClass]
    public class GetLabelSeriesTest
    {
        [TestMethod]
        public void TestDeprivation()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl + "GetLabelSeries.ashx?id=dep");
            string s = Encoding.ASCII.GetString(data);
            Assert.AreNotEqual(0, s.Length);
            Assert.IsTrue(s.ToLower().Contains("depriv"));
        }

        [TestMethod]
        public void TestQuinaryPopulation()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl + "GetLabelSeries.ashx?id=qpop");
            string s = Encoding.ASCII.GetString(data);
            Assert.AreNotEqual(0, s.Length);
            Assert.IsTrue(s.Contains("30-34"));
        }
    }
}
