using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PholioObjectsTest
{
    [TestClass]
    public class ParentAreaTest
    {
        public const string DefaultAreaCode = "a";
        public const string DifferentAreaCode = "b";
        public const int DefaultAreaTypeId = 1;
        public const int DifferentAreaTypeId = 2;

        [TestMethod]
        public void TestEqualsIncludesAreaCode()
        {
            Assert.IsFalse(
                ParentArea().Equals(
                ParentAreaWithDifferentAreaCode()));
        }

        [TestMethod]
        public void TestEqualsIncludesChildAreaTypeId()
        {
            Assert.IsFalse(
                ParentArea().Equals(
                ParentAreaWithDifferentAreaTypeId()));
        }

        [TestMethod]
        public void TestGetHashCodeIncludesAreaCode()
        {
            Assert.AreNotEqual(
                ParentArea().GetHashCode(), 
                ParentAreaWithDifferentAreaCode().GetHashCode());
        }

        [TestMethod]
        public void TestGetHashCodeIncludesChildAreaTypeId()
        {
            Assert.AreNotEqual(ParentArea().GetHashCode(), 
                ParentAreaWithDifferentAreaTypeId().GetHashCode());
        }

        private static ParentArea ParentAreaWithDifferentAreaCode()
        {
            var parentArea = ParentArea();
            parentArea.AreaCode = DifferentAreaCode;
            return parentArea;
        }

        private static ParentArea ParentAreaWithDifferentAreaTypeId()
        {
            var parentArea = ParentArea();
            parentArea.ChildAreaTypeId = DifferentAreaTypeId;
            return parentArea;
        }

        private static ParentArea ParentArea()
        {
            return new ParentArea(DefaultAreaCode, DefaultAreaTypeId);
        }
    }
}
