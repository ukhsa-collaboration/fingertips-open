using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class GetData_ParentAreasTest
    {
        [TestMethod]
        public void TestPracticeParents()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetData.ashx?s=pa&are=d81033&ati=5,2,19");
            TestHelper.IsData(data);

            string s = Encoding.Default.GetString(data).ToLower();

            // Parent CCG
            Assert.IsTrue(s.Contains(AreaCodes.Ccg_CambridgeshirePeterborough.ToLower()));

            // Parent PCT
            Assert.IsTrue(s.Contains(AreaCodes.Pct_Cambridgeshire.ToLower()));

            // Parent SHA
            Assert.IsTrue(s.Contains(AreaCodes.Sha_EastOfEngland.ToLower()));
        }
    }
}
