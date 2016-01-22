using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
{
    [TestClass]
    public class ComparatorTest
    {
        private const int DefaultComparatorId = 1;
        private const int DefaultAreaTypeId = 2;
        private const string DefaultAreaCode = "a";

        [TestMethod]
        public void TestAreEqual()
        {
            Assert.AreEqual(GetComparator(), GetComparator());
        }

        [TestMethod]
        public void TestAreNotEqual()
        {
            // AreaTypeId different
            Comparator c = new Comparator
            {
                Area = new Area
                {
                    AreaTypeId = 3,
                    Code = DefaultAreaCode
                },
                ChildAreaTypeId = 3,
                ComparatorId = DefaultComparatorId,
            };
            Assert.AreNotEqual(GetComparator(), c);

            // Area code different
            c = new Comparator
            {
                Area = new Area
                {
                    AreaTypeId = DefaultAreaTypeId,
                    Code = "b"
                },
                ChildAreaTypeId = DefaultAreaTypeId,
                ComparatorId = DefaultComparatorId
            };
            Assert.AreNotEqual(GetComparator(), c);

            // ComparatorId different
            c = new Comparator
            {
                Area = new Area
                {
                    AreaTypeId = DefaultAreaTypeId,
                    Code = DefaultAreaCode
                },
                ChildAreaTypeId = DefaultAreaTypeId,
                ComparatorId = 3
            };
            Assert.AreNotEqual(GetComparator(), c);
        }

        private static Comparator GetComparator()
        {
            return new Comparator
                 {
                     Area = new Area
                         {
                             AreaTypeId = DefaultAreaTypeId,
                             Code = DefaultAreaCode
                         },
                     ChildAreaTypeId = DefaultAreaTypeId,
                     ComparatorId = DefaultComparatorId
                 };
        }

    }
}
