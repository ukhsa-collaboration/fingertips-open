using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Controllers;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class NonPublicServicesControllerTest
    {
        [TestMethod]
        public void TestGetNhsChoicesAreaId()
        {
            var nhsChoicesId = new NonPublicServicesController().GetNhsChoicesAreaId(AreaCodes.Gp_Burnham);
            Assert.AreEqual("43611", nhsChoicesId);
        }

        [TestMethod]
        public void TestGetIndicatorsByAreaTypeForIndicatorList()
        {
            var ids = new NonPublicServicesController().GetIndicatorsByAreaTypeForIndicatorList("-1");
            Assert.IsNotNull(ids);
        }

        [TestMethod]
        public void TestGetGroupDataAtDataPointForIndicatorList()
        {
            var ids = new NonPublicServicesController().GetGroupDataAtDataPointForIndicatorList("-1",
                AreaTypeIds.CountyAndUnitaryAuthority, AreaCodes.England);
            Assert.IsNotNull(ids);
        }
    }
}
