using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class GroupingValidatorTest
    {
        [TestMethod]
        public void TestAgesInLive()
        {
            Assert.IsTrue(Ages().Contains(AgeIds.From18To64));
        }

        [TestMethod]
        public void TestAgesNotInLive()
        {
            Assert.IsFalse(Ages().Contains(AgeIds.NotAnActualAge));
        }

        [TestMethod]
        public void TestAreaTypesInLive()
        {
            Assert.IsTrue(AreaTypes().Contains(AreaTypeIds.CombinedAuthorities));
        }

        [TestMethod]
        public void TestAreaTypesNotInLive()
        {
            Assert.IsFalse(AreaTypes().Contains(AreaTypeIds.Undefined));
        }

        private static IList<int> Ages()
        {
            return ReaderFactory.GetPholioReader().GetAllAgeIds();
        }

        private static IList<int> AreaTypes()
        {
            return ReaderFactory.GetAreasReader().GetAllAreaTypeIds();
        }
    }
}
