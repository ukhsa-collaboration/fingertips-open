using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PholioVisualisation.ServicesTest
{
    /// <summary>
    /// Summary description for AreaListControllerEndPointTest
    /// </summary>
    [TestClass]
    public class AreaListControllerEndPointTest
    {
        public const string UserId = "58189c36-969d-4e13-95c9-67a01832ab24";
        public const int AreaListId = 10;
        public const string PublicId = "al-ZY6zmuVONE";

        [TestMethod]
        public void TestGetAreaLists()
        {
            byte[] data = EndPointTestHelper.GetData("arealists?user_id=" + UserId);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetAreaList()
        {
            byte[] data = EndPointTestHelper.GetData("arealist?area_list_id=" + AreaListId);
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
            byte[] data = EndPointTestHelper.GetData("arealist/areacodes?area_list_id=" + AreaListId);
            TestHelper.IsData(data);
        }
    }
}
