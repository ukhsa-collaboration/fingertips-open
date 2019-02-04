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
        public void TestGetProfile()
        {
            byte[] data = EndPointTestHelper.GetData("profile?" +
                "profile_id=" + ProfileIds.PhysicalActivity);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetProfiles()
        {
            byte[] data = EndPointTestHelper.GetData("profiles");
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetProfilesPerIndicatorWithAreaTypeId()
        {
            byte[] data = EndPointTestHelper.GetData("profiles_containing_indicators?indicator_ids=" +
                                                     IndicatorIds.DeprivationScoreIMD2010 + "&AreaTypeId=" +
                                                     AreaTypeIds.GpPractice);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetProfilesPerIndicator()
        {
            byte[] data = EndPointTestHelper.GetData("profiles_containing_indicators?indicator_ids=" +
                                                     IndicatorIds.DeprivationScoreIMD2010);
            TestHelper.IsData(data);
        }


        [TestMethod]
        public void TestGetAreaTypesWithPdfs()
        {
            byte[] data = EndPointTestHelper.GetData("profile/area_types_with_pdfs?" +
                "profile_id=" + ProfileIds.PhysicalActivity);
            TestHelper.IsData(data);
        }
    }
}
