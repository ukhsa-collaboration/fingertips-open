
using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccessTest
{
    [TestClass]
    public class AreaTypeIdSplitterTest
    {
        [TestMethod]
        public void TestCountyAndUnitaryAuthority()
        {
            AreaTypeIdSplitter splitter = new AreaTypeIdSplitter(
                new[] { AreaTypeIds.CountyAndUnitaryAuthority });

            Assert.AreEqual(2, splitter.Ids.Count);
            Assert.IsTrue(splitter.Ids.Contains(AreaTypeIds.County));
            Assert.IsTrue(splitter.Ids.Contains(AreaTypeIds.UnitaryAuthority));
        }

        [TestMethod]
        public void TestLocalAuthorityAndUnitaryAuthority()
        {
            AreaTypeIdSplitter splitter = new AreaTypeIdSplitter(
                new[] { AreaTypeIds.DistrictAndUnitaryAuthority });

            Assert.AreEqual(2, splitter.Ids.Count);
            Assert.IsTrue(splitter.Ids.Contains(AreaTypeIds.District));
            Assert.IsTrue(splitter.Ids.Contains(AreaTypeIds.UnitaryAuthority));
        }

        [TestMethod]
        public void TestCountyAndUnitaryAuthorityAndPct()
        {
            AreaTypeIdSplitter splitter = new AreaTypeIdSplitter(
                new[] { AreaTypeIds.CountyAndUnitaryAuthority, AreaTypeIds.Pct });

            Assert.AreEqual(3, splitter.Ids.Count);
            Assert.IsTrue(splitter.Ids.Contains(AreaTypeIds.County));
            Assert.IsTrue(splitter.Ids.Contains(AreaTypeIds.UnitaryAuthority));
            Assert.IsTrue(splitter.Ids.Contains(AreaTypeIds.Pct));
        }

        [TestMethod]
        public void TestAcuteTrustsIncludingCombinedMentalHealthTrusts()
        {
            AreaTypeIdSplitter splitter = new AreaTypeIdSplitter(
                new[] { AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts });

            Assert.AreEqual(2, splitter.Ids.Count);
            Assert.IsTrue(splitter.Ids.Contains(AreaTypeIds.AcuteTrust));
            Assert.IsTrue(splitter.Ids.Contains(AreaTypeIds.CombinedMentalHealthAndAcuteTrust));
        }

        [TestMethod]
        public void TestMentalHealthTrustsIncludingCombinedAcuteTrusts()
        {
            AreaTypeIdSplitter splitter = new AreaTypeIdSplitter(
                new[] { AreaTypeIds.MentalHealthTrustsIncludingCombinedAcuteTrusts });

            Assert.AreEqual(2, splitter.Ids.Count);
            Assert.IsTrue(splitter.Ids.Contains(AreaTypeIds.MentalHealthTrust));
            Assert.IsTrue(splitter.Ids.Contains(AreaTypeIds.CombinedMentalHealthAndAcuteTrust));
        }
    }
}
