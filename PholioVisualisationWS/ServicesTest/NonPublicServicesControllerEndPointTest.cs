using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class NonPublicServicesControllerEndPointTest
    {
        [TestMethod]
        public void TestGetChimatResourceId()
        {
            byte[] data = GetData("area/chimat_resource_id?" +
                "area_code=" + AreaCodes.CountyUa_Buckinghamshire + "&profile_id=" + ProfileIds.ChildHealthProfiles);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetChimatWayResourceId()
        {
            byte[] data = GetData("area/chimat_resource_id?" +
                "area_code=" + AreaCodes.CountyUa_Buckinghamshire + "&profile_id=" + ProfileIds.WhatAboutYouth);
            TestHelper.IsData(data);
        }

        public byte[] GetData(string path)
        {
            return DataControllerEndPointTest.GetData(path);
        }
    }
}
