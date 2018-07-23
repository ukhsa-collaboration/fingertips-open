using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class ContentControllerEndPointTest
    {
        [TestMethod]
        public void TestGetContent()
        {
            byte[] data = EndPointTestHelper.GetData("content?"+
                                                     "profile_Id=" + ProfileIds.Phof +
                                                     "&key=contact-us");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetContent_Test_Content()
        {
            byte[] data = EndPointTestHelper.GetData("content?" +
                                                     "profile_id=" + ProfileIds.PhysicalActivity +
                                                     "&key=test-key");
            TestHelper.IsData(data);
        }
    }
}
