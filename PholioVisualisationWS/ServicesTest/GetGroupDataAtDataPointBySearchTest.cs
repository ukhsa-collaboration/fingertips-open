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
            foreach (var areaTypeId in new[] { AreaTypeIds.DistrictAndUnitaryAuthority, AreaTypeIds.CountyAndUnitaryAuthority })
            {
                var url = TestHelper.BaseUrl + string.Format("GetGroupDataAtDataPointBySearch.ashx?" +
                    "gid=" + GroupIds.Search +
                    "&ati={0}" +
                    "&par=" + AreaCodes.Gor_EastMidlands +
                    "&pid=" + ProfileIds.Search +
                    "&res=" + ProfileIds.Phof +
                    "&iids=" + IndicatorIds.ChildrenInLowIncomeFamilies, (int)areaTypeId);

                byte[] data = new WebClient().DownloadData(url);

                TestHelper.IsData(data);
                string s = Encoding.Default.GetString(data);
                Assert.IsFalse(s.Contains("groupRoots:[]"));

            }
        }
    }
}
