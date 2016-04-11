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
    public class GetGroupDataAtDataPointBySearchTest
    {
        [TestMethod]
        public void TestSearchFindsResultsForAllAreaTypesWhereDataIsAvailable()
        {
            foreach (var areaTypeId in new [] { AreaTypeIds.DistrictAndUnitaryAuthority, AreaTypeIds.CountyAndUnitaryAuthority })
            {
                byte[] data = new WebClient().DownloadData(TestHelper.BaseUrl +
                    string.Format("GetGroupDataAtDataPointBySearch.ashx?gid=1&ati={0}&par=EMREG&pid=13&iids=767", (int)areaTypeId));
                TestHelper.IsData(data);
                string s = Encoding.Default.GetString(data);
                Assert.IsFalse(s.Contains("groupRoots:[]"));

            }
        }
    }
}
