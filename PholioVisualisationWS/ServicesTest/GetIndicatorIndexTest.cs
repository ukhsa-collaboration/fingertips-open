/* 
 * Created by: Daniel Flint    
 * Date: 21/09/2011
 */
using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServicesTest
{
    [TestClass]
    public class GetIndicatorIndexTest
    {
        [TestMethod]
        public void TestGetIndicatorIndex()
        {
            byte[] data = new WebClient().DownloadData(GetLabelTest.BaseUrl + "GetIndicatorIndex.ashx");
            Assert.AreNotEqual(0, data.Length);
        }
    }
}
