
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstructionTest
{
    [TestClass]
    public class ComparatorMapBuilderTest
    {
        public static ParentArea GetRegion102()
        {
            return new ParentArea(AreaCodes.Gor_EastOfEngland,
                AreaTypeIds.CountyAndUnitaryAuthority);
        }

        public static ParentArea GetShaForPct()
        {
            return new ParentArea(AreaCodes.Sha_EastOfEngland, AreaTypeIds.Pct);
        }

        [TestMethod]
        public void TestBuild()
        {
            ComparatorMapBuilder builder = new ComparatorMapBuilder(new List<ParentArea> { GetRegion102(), GetShaForPct() });
            Assert.AreEqual(4, builder.ComparatorMap.Comparators.Count());
            Assert.AreEqual(2, (from c in builder.ComparatorMap.Comparators where c.ComparatorId == ComparatorIds.Subnational select c).Count());
            Assert.AreEqual(2, (from c in builder.ComparatorMap.Comparators where c.ComparatorId == ComparatorIds.England select c).Count());
        }

        [TestMethod]
        public void TestComparatorMapGetRegionalComparatorByRegion()
        {
            ComparatorMapBuilder builder = new ComparatorMapBuilder(GetShaForPct());

            Assert.AreEqual(builder.ComparatorMap.GetRegionalComparatorByRegion(GetShaForPct()).ChildAreaTypeId,
                AreaTypeIds.Pct);
        }

        [TestMethod]
        public void TestComparatorMapGetRegionalComparator()
        {
            ComparatorMapBuilder builder = new ComparatorMapBuilder(GetShaForPct());
            Assert.AreEqual(builder.ComparatorMap.GetSubnationalComparator().Area.Code, AreaCodes.Sha_EastOfEngland);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void TestComparatorMapGetRegionalComparatorFailsIfTwoRegionalComparators()
        {
            ComparatorMapBuilder builder = new ComparatorMapBuilder(new List<ParentArea> { GetRegion102(), GetShaForPct() });
            builder.ComparatorMap.GetSubnationalComparator();
        }

        [TestMethod]
        public void TestComparatorMapGetNationalComparator()
        {
            ComparatorMapBuilder builder = new ComparatorMapBuilder(GetShaForPct());
            Assert.AreEqual(builder.ComparatorMap.GetNationalComparator().Area.Code, AreaCodes.England);
        }

        [TestMethod]
        public void TestComparatorMapGetNationalComparatorWorksIfTwoNationalComparators()
        {
            ComparatorMapBuilder builder = new ComparatorMapBuilder(new List<ParentArea> { GetRegion102(), GetShaForPct() });
            Assert.AreEqual(builder.ComparatorMap.GetNationalComparator().Area.Code, AreaCodes.England);
        }

        [TestMethod]
        public void TestComparatorMapLimitByAreaType()
        {
            ComparatorMapBuilder builder = new ComparatorMapBuilder(new List<ParentArea> { GetRegion102(), GetShaForPct() });
            Assert.AreEqual(4, builder.ComparatorMap.Count);

            ComparatorMap map = builder.ComparatorMap.LimitByParentArea(GetShaForPct());
            foreach (var comparator in map.Comparators)
            {
                Assert.AreEqual(AreaTypeIds.Pct, comparator.ChildAreaTypeId);
            }
        }


    }
}
