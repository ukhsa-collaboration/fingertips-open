using System;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace ServicesTest
{
    [TestClass]
    public class GetData_AreaCategoriesTest
    {
        [TestMethod]
        public void TestGetData()
        {
            byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                "GetData.ashx?s=ac" + 
                "&category_type_id=" + CategoryTypeIds.DeprivationDecileCountyAndUnitaryAuthority + 
                "&ati=" + AreaTypeIds.CountyAndUnitaryAuthority);

            AssertDataOk(data);
        }

        private static void AssertDataOk(byte[] data)
        {
            TestHelper.IsData(data);

            string s = Encoding.Default.GetString(data);
            Assert.IsTrue(s.Contains(AreaCodes.CountyUa_Cumbria));
        }
    }
}
