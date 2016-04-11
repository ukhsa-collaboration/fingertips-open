using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class AreasOfAreaTypeActionTest
    {
        [TestMethod]
        public void TestGetResponse()
        {
            var areas = new AreasOfAreaTypeAction().GetResponse(
                AreaTypeIds.CountyAndUnitaryAuthority, ProfileIds.Phof,
                ProfileIds.Phof, false);

            Assert.IsTrue(areas.Select(x => x.Code).Contains(AreaCodes.CountyUa_Manchester));
        }
    }
}
