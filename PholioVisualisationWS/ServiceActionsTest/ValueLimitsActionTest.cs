using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.ServiceActions;

namespace PholioVisualisation.ServiceActionsTest
{
    [TestClass]
    public class ValueLimitsActionTest
    {
        [TestMethod]
        public void TestGetResponse_CcgLimits()
        {
            AssertLimitsDefinedlimitsList(GetLimits(AreaCodes.Ccg_Chiltern));
        }

        [TestMethod]
        public void TestGetResponse_NationalLimits()
        {
            AssertLimitsDefinedlimitsList(GetLimits(AreaCodes.England));
        }

        [TestMethod]
        public void TestGetResponse_NoParentAreaCode()
        {
            AssertLimitsDefinedlimitsList(GetLimits(null));
        }

        [TestMethod]
        public void TestGetResponse_NationalAndCcgLimitsAreDifferent()
        {
            var ccgLimitsList = GetLimits(AreaCodes.Ccg_Chiltern);
            var nationalLimitsList = GetLimits(AreaCodes.England);

            int notEqualCount = 0;
            for (var index = 0; index < ccgLimitsList.Count; index++)
            {
                var ccgLimits = ccgLimitsList[index];
                var nationalLimits = nationalLimitsList[index];

                if (ccgLimits != null)
                {
                    if (ccgLimits.Min != nationalLimits.Min)
                    {
                        notEqualCount++;
                    }

                    if (ccgLimits.Max != nationalLimits.Max)
                    {
                        notEqualCount++;
                    }
                }
            }

            // Asser at least half of limits are different
            Assert.IsTrue(notEqualCount > nationalLimitsList.Count);
        }

        private static IList<Limits> GetLimits(string areaCode)
        {
            var action = new ValueLimitsAction();
            var limitsList = action.GetResponse(GroupIds.PracticeProfiles_PracticeSummary,
                AreaTypeIds.GpPractice, areaCode);
            return limitsList;
        }

        /// <summary>
        /// Assert at least one limits object was defined (tolerate indicators without data).
        /// </summary>
        private void AssertLimitsDefinedlimitsList(IList<Limits> limitsList)
        {
            int limitsDefinedCount = limitsList.Count(limits => limits != null);
            Assert.IsTrue(limitsDefinedCount > 0);
        }

    }
}
