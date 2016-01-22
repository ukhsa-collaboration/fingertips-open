using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace ServiceActionsTest
{
    [TestClass]
    public class SpcForDsrLimitsActionTest
    {
        [TestMethod]
        public void Test_GetResponse_ComparatorValueAssignedCorrectly()
        {
            Assert.AreEqual(1, Action.GetResponse(1, 2, 2, 2, 2).ComparatorValue);
        }

        [TestMethod]
        public void Test_GetResponse_PointsCountAsExpected()
        {
            var points = Action.GetResponse(1, 1, 100, 1, 1).Points;
            Assert.AreEqual(100, points.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void Test_GetResponse_ComparatorValueMustBeGreaterThanZero()
        {
            Action.GetResponse(0, 1, 1, 1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void Test_GetResponse_PopulationMinMustBeGreaterThanZero()
        {
            Action.GetResponse(1, 0, 1, 1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void Test_GetResponse_PopulationMaxMustBeGreaterThanZero()
        {
            Action.GetResponse(1, 1, 0, 1, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void Test_GetResponse_UnitValueMustBeGreaterThanZero()
        {
            Action.GetResponse(1, 1, 1, 0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(FingertipsException))]
        public void Test_GetResponse_YearRangeMustBeGreaterThanZero()
        {
            Action.GetResponse(1, 1, 1, 1, 0);
        }

        private SpcForDsrLimitsAction Action
        {
            get
            {
                return new SpcForDsrLimitsAction();
            }
        }
    }
}
