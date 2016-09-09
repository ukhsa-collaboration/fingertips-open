using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class ProfilesControllerEndPointTest
    {
        [TestMethod]
        public void TestGetContent()
        {
            byte[] data = DataControllerEndPointTest.GetData("content?" +
                "profile_id=" + ProfileIds.PhysicalActivity +
                "&key=test-key");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetProfile()
        {
            byte[] data = DataControllerEndPointTest.GetData("profile?" +
                "profile_id=" + ProfileIds.PhysicalActivity);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetProfiles()
        {
            byte[] data = DataControllerEndPointTest.GetData("profiles");
            TestHelper.IsData(data);
        }
    }
}
