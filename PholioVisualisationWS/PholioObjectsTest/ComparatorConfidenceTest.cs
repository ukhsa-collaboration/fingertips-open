using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;

namespace PholioObjectsTest
{
    [TestClass]
    public class ComparatorConfidenceTest
    {
        public const double DefaultConfidenceValue = 95;
        public const double DifferentConfidenceValue = 99.8;
        public const int DefaultMethodId = 1;
        public const int DifferentMethodId = 2;

        [TestMethod]
        public void TestEqualsIncludesMethodId()
        {
            Assert.IsFalse(
                ComparatorConfidence().Equals(
                ComparatorConfidenceWithDifferentMethodId()));
        }

        [TestMethod]
        public void TestEqualsIncludesConfidenceValue()
        {
            Assert.IsFalse(
                ComparatorConfidence().Equals(
                ComparatorConfidenceWithDifferentConfidenceValue()));
        }

        [TestMethod]
        public void TestGetHashCodeIncludesMethodId()
        {
            Assert.AreNotEqual(
                ComparatorConfidence().GetHashCode(), 
                ComparatorConfidenceWithDifferentMethodId().GetHashCode());
        }

        [TestMethod]
        public void TestGetHashCodeIncludesConfidenceValue()
        {
            Assert.AreNotEqual(
                ComparatorConfidence().GetHashCode(),
                ComparatorConfidenceWithDifferentConfidenceValue().GetHashCode());
        }

        private static ComparatorConfidence ComparatorConfidenceWithDifferentConfidenceValue()
        {
            var parentArea = ComparatorConfidence();
            parentArea.ConfidenceValue = DifferentConfidenceValue;
            return parentArea;
        }

        private static ComparatorConfidence ComparatorConfidenceWithDifferentMethodId()
        {
            var parentArea = ComparatorConfidence();
            parentArea.ComparatorMethodId = DifferentMethodId;
            return parentArea;
        }

        private static ComparatorConfidence ComparatorConfidence()
        {
            return new ComparatorConfidence {
                ComparatorMethodId = DefaultMethodId,
                ConfidenceValue = DefaultConfidenceValue
            };
        }
    }
}
