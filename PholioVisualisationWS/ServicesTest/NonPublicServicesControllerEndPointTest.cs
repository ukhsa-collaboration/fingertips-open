using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class NonPublicServicesControllerEndPointTest
    {
        [TestMethod]
        public void TestGetIndicatorsByAreaTypeForIndicatorList()
        {
            byte[] data = GetData("indicator-list/indicators-for-each-area-type?indicator_list_id=-1");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetGroupDataAtDataPointForIndicatorList()
        {
            byte[] data = GetData("latest_data/indicator_list_for_child_areas?indicator_list_id=-1"+
                "&area_type_id=" + AreaTypeIds.CountyAndUnitaryAuthority +
                "&parent_area_code=" + AreaCodes.England);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetIndicatorList()
        {
            byte[] data = GetData("indicator-list?id=-1");
            TestHelper.IsData(data);
        }

        public byte[] GetData(string path)
        {
            return DataControllerEndPointTest.GetData(path);
        }
    }
}
