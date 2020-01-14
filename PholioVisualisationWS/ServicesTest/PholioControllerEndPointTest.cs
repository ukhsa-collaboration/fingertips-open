using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServicesTest
{
    /// <summary>
    /// Summary description for PholioControllerEndPointTest
    /// </summary>
    [TestClass]
    public class PholioControllerEndPointTest
    {
        [TestMethod]
        public void TestGetMetadata()
        {
            byte[] data = EndPointTestHelper.GetData("metadata?" +
                "indicator_id=" + IndicatorIds.DeprivationScoreIMD2010);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGroupings()
        {
            byte[] data = EndPointTestHelper.GetData("groupings?" +
                                                     "profile_id=" + ProfileIds.Phof +
                                                     "&indicator_id=" + IndicatorIds.DeprivationScoreIMD2010);
            TestHelper.IsData(data);
        }

        [TestMethod]
        public void TestGetCoreDataSets()
        {
            byte[] data = EndPointTestHelper.GetData("coredata?" +
                                                     "indicator_id=" + IndicatorIds.IndicatorThatDoesNotExist);
            TestHelper.IsData(data);
        }
    }
}
