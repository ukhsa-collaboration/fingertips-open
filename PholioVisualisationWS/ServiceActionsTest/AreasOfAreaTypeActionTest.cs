using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;
using System.Linq;
using PholioVisualisation.UserDataTest;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class AreasOfAreaTypeActionTest
    {
        public const string UserId = FingertipsUserIds.TestUser;
        private const string ParentCode = AreaListCodes.TestListId;

        [TestMethod]
        public void TestGetResponse()
        {
            var areas = new AreasOfAreaTypeAction().GetResponse(
                AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019, ProfileIds.Phof,
                ProfileIds.Phof, false, String.Empty);

            Assert.IsTrue(areas.Select(x => x.Code).Contains(AreaCodes.CountyUa_Manchester));
        }

        [TestMethod]
        public void TestGetResponse_GetAreaByAreaType_For_Area_List()
        {
            var areaListHelper = new AreaListTestHelper();
            areaListHelper.CreateTestList(new List<string>());

            try
            {
                var area = new AreasOfAreaTypeAction().GetResponse(AreaTypeIds.AreaList, ProfileIds.Phof, UserId,
                    ParentCode);

                Assert.IsTrue(area.Code == ParentCode);
            }
            finally
            {
                areaListHelper.DeleteTestList();
            }
        }
    }
}
