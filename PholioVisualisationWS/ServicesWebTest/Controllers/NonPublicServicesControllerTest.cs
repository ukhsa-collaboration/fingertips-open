using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using ServicesWeb.Controllers;

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
        public void TestGetChimatResourceId()
        {
            var id = new NonPublicServicesController().GetChimatResourceId(AreaCodes.CountyUa_Cumbria, 105);
            Assert.AreEqual(ChimatResourceIds.Cumbria, id);
        }

        [TestMethod]
        public void TestGetChimatWayResourceId()
        {
            var id = new NonPublicServicesController().GetChimatResourceId(AreaCodes.CountyUa_Cumbria, 94);
            Assert.AreEqual(ChimatWayResourceIds.Cumbria, id);
        }
    }
}
