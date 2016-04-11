using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class AreaHierarchyTest
    {
        public const string DefaultParentAreaCode = "a";
        public const string DefaultChildAreaCode = "b";
        public const string DifferentAreaCode = "c";

        [TestMethod]
        public void TestEqualsIncludesParentAreaCode()
        {
            Assert.IsFalse(
                AreaHierarchy().Equals(
                AreaHierarchyWithDifferentParentAreaCode()));
        }

        [TestMethod]
        public void TestEqualsIncludesChildAreaCode()
        {
            Assert.IsFalse(
                AreaHierarchy().Equals(
                AreaHierarchyWithDifferentChildAreaCode()));
        }

        [TestMethod]
        public void TestGetHashCodeIncludesParentAreaCode()
        {
            Assert.AreNotEqual(
                AreaHierarchy().GetHashCode(), 
                AreaHierarchyWithDifferentParentAreaCode().GetHashCode());
        }

        [TestMethod]
        public void TestGetHashCodeIncludesChildAreaCode()
        {
            Assert.AreNotEqual(
                AreaHierarchy().GetHashCode(), 
                AreaHierarchyWithDifferentChildAreaCode().GetHashCode());
        }

        private static AreaHierarchy AreaHierarchyWithDifferentChildAreaCode()
        {
            var areaHierarchy = AreaHierarchy();
            areaHierarchy.ChildAreaCode = DifferentAreaCode;
            return areaHierarchy;
        }

        private static AreaHierarchy AreaHierarchyWithDifferentParentAreaCode()
        {
            var areaHierarchy = AreaHierarchy();
            areaHierarchy.ParentAreaCode = DifferentAreaCode;
            return areaHierarchy;
        }

        private static AreaHierarchy AreaHierarchy()
        {
            var a = new AreaHierarchy
            {
                ParentAreaCode = DefaultParentAreaCode,
                ChildAreaCode = DefaultChildAreaCode
            };
            return a;
        }

    }
}
