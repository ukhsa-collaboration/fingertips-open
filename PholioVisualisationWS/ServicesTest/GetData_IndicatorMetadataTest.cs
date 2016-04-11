
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class GetData_IndicatorMetadataTest
    {
        [TestMethod]
        public void TestGetIndicatorMetadata()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl + "GetData.ashx?s=im&gid=2000002");
            TestHelper.IsData(data);

            string s = System.Text.Encoding.Default.GetString(data);
            Assert.IsTrue(s.Contains("d"));
        }
    }
}
