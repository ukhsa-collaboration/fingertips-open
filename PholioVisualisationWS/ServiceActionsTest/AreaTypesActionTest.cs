using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace ServiceActionsTest
{
    [TestClass]
    public class AreaTypesActionTest
    {
        [TestMethod]
        public void TestGetResponse()
        {
            var areaTypes = new AreaTypesAction();
                var areaTypesList = areaTypes.GetResponse(new List<int>{ProfileIds.Phof});
                Assert.IsTrue(areaTypesList.Any());
        }
    }
}
