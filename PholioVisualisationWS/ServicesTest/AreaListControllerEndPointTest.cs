using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.UserData;
using PholioVisualisation.UserData.Repositories;
using PholioVisualisation.UserDataTest;

namespace PholioVisualisation.ServicesTest
{
    /// <summary>
    /// Summary description for AreaListControllerEndPointTest
    /// </summary>
    [TestClass]
    public class AreaListControllerEndPointTest
    {
        public const string UserId = FingertipsUserIds.TestUser;
        public const string PublicId = AreaListCodes.TestListId;
        public int _areaListId;

        private AreaListTestHelper _areaListTestHelper = new  AreaListTestHelper();

        [TestInitialize]
        public void TestInitialize()
        {
            var codes = new List<string> { AreaCodes.CountyUa_Cambridgeshire };
            _areaListTestHelper = new AreaListTestHelper();
            _areaListId = _areaListTestHelper.CreateTestList(codes);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _areaListTestHelper.DeleteTestList();
        }

        [TestMethod]
        public void TestGetAreaLists()
        {
            byte[] data = EndPointTestHelper.GetData("arealists?user_id=" + UserId);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAreaList()
        {
            byte[] data = EndPointTestHelper.GetData("arealist?area_list_id=" + _areaListId);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAreaListByPublicId()
        {
            byte[] data =
                EndPointTestHelper.GetData("arealist/by_public_id?public_id=" + PublicId + "&user_id=" + UserId);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAreaListAreaCodes()
        {
            byte[] data = EndPointTestHelper.GetData("arealist/areacodes?area_list_id=" + _areaListId);
            TestHelper.IsData(data);
        }
    }
}
