using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;
using System.Linq;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class AreasOfAreaTypeActionTest
    {
        public const string UserId = "58189c36-969d-4e13-95c9-67a01832ab24";
        private const string ParentCode = "al-CsEBNuSNwk";

        [TestMethod]
        public void TestGetResponse()
        {
            var areas = new AreasOfAreaTypeAction().GetResponse(
                AreaTypeIds.CountyAndUnitaryAuthority, ProfileIds.Phof,
                ProfileIds.Phof, false, String.Empty);

            Assert.IsTrue(areas.Select(x => x.Code).Contains(AreaCodes.CountyUa_Manchester));
        }

        [TestMethod]
        public void TestGetResponse_GetAreaByAreaTypeAndParentCode()
        {
            var area = new AreasOfAreaTypeAction().GetResponse(AreaTypeIds.AreaList, ProfileIds.Phof, UserId, ParentCode);

            Assert.IsTrue(area.Code == ParentCode);
        }
    }
}
