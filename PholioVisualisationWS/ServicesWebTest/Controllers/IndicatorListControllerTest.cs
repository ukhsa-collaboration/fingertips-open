using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServicesWeb.Controllers;

namespace PholioVisualisation.ServicesWebTest.Controllers
{
    [TestClass]
    public class IndicatorListControllerTest
    {
        [TestMethod]
        public void TestGetIndicatorsByAreaTypeForIndicatorList()
        {
            var ids = new InternalServicesController().GetIndicatorsByAreaTypeForIndicatorList("-1");
            Assert.IsNotNull(ids);
        }

        [TestMethod]
        public void TestGetGroupDataAtDataPointForIndicatorList()
        {
            var ids = new InternalServicesController().GetGroupDataAtDataPointForIndicatorList("-1",
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019, AreaCodes.England);
            Assert.IsNotNull(ids);
        }
    }
}
