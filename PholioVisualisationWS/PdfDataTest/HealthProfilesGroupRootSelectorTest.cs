using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PdfData;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfDataTest
{
    [TestClass]
    public class HealthProfilesGroupRootSelectorTest
    {
        [TestMethod]
        public void TestSlopeIndexOfInequalityForLifeExpectancyMale()
        {
            var root = SelectorForSlopeIndexOfInequalityForLifeExpectancy().SlopeIndexOfInequalityForLifeExpectancyMale;
            Assert.AreEqual(root.IndicatorId, IndicatorIds.SlopeIndexOfInequalityForLifeExpectancy);
            Assert.AreEqual(root.SexId, SexIds.Male);
        }

        [TestMethod]
        public void TestSlopeIndexOfInequalityForLifeExpectancyFemale()
        {
            var root = SelectorForSlopeIndexOfInequalityForLifeExpectancy().SlopeIndexOfInequalityForLifeExpectancyFemale;
            Assert.AreEqual(root.IndicatorId, IndicatorIds.SlopeIndexOfInequalityForLifeExpectancy);
            Assert.AreEqual(root.SexId, SexIds.Female);
        }

        private static HealthProfilesGroupRootSelector SelectorForSlopeIndexOfInequalityForLifeExpectancy()
        {
            var indicatorId = IndicatorIds.SlopeIndexOfInequalityForLifeExpectancy;

            var roots = new List<GroupRoot>
            {
                new GroupRoot {IndicatorId = IndicatorIds.ChildrenInPoverty, SexId = SexIds.Male},
                new GroupRoot {IndicatorId = IndicatorIds.ChildrenInPoverty, SexId = SexIds.Female},
                new GroupRoot {IndicatorId = indicatorId, SexId = SexIds.Male},
                new GroupRoot {IndicatorId = indicatorId, SexId = SexIds.Female}
            };

            var selector = new HealthProfilesGroupRootSelector();
            selector.SupportingGroupRoots = roots;
            return selector;
        }

    }
}
