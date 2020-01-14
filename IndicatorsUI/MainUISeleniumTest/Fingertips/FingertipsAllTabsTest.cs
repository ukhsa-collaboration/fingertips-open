using IndicatorsUI.DomainObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Text.RegularExpressions;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsAllTabsTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_All_Tabs_Load_For_Health_Profiles()
        {
            CheckAllTabsLoadForProfile(ProfileUrlKeys.HealthProfiles);
        }

        [TestMethod]
        public void Test_All_Tabs_Load_For_Sexual_Health()
        {
            CheckAllTabsLoadForProfile(ProfileUrlKeys.SexualHealth);
        }

        [TestMethod]
        public void Test_All_Tabs_Load_For_Tb_Strategy()
        {
            CheckAllTabsLoadForProfile(ProfileUrlKeys.TbStrategy);
        }

        [TestMethod]
        public void Test_All_Tabs_Load_For_Phof()
        {
            CheckAllTabsLoadForProfile(ProfileUrlKeys.Phof);
        }

        [TestMethod]
        public void Test_All_Tabs_Load_For_Practice_Profiles()
        {
            CheckAllTabsLoadForProfile(ProfileUrlKeys.PracticeProfiles);
        }

        /// <summary>
        /// 'undefined' on a page indicates some data has not been found in JavaScript
        /// </summary>
        [TestMethod]
        public void Test_No_Tab_Contains_Undefined()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.HealthProfiles);

            var tabIds = new [] { FingertipsIds.TabMap, FingertipsIds.TabCompareIndicators,
                FingertipsIds.TabTrends, FingertipsIds.TabCompareAreas,
                FingertipsIds.TabAreaProfiles, FingertipsIds.TabInequalities,
                FingertipsIds.TabDefinitions, FingertipsIds.TabOverview, FingertipsIds.TabBoxPlot,
                FingertipsIds.TabPopulation, FingertipsIds.TabEngland
            };

            foreach (var tabId in tabIds)
            {
                if (driver.FindElement(By.Id(tabId)) != null)
                {
                    AssertPageDoesNotContainUndefined(tabId);
                }
            }
        }

        private void CheckAllTabsLoadForProfile(string urlKey)
        {
            navigateTo.FingertipsDataForProfile(urlKey);
            fingertipsHelper.SelectEachFingertipsTabInTurnAndCheckDownloadIsLast();
        }

        private void AssertPageDoesNotContainUndefined(string tabId)
        {
            fingertipsHelper.SelectTab(tabId);

            var body = driver.FindElement(By.TagName("body"));
            var html = body.GetAttribute("innerHTML");

            var regex = new Regex(Regex.Escape(html));
            var undefinedCount = regex.Matches("undefined").Count;
            var ignoreUndefinedCount = regex.Matches("highcharts-color-undefined").Count;

            Assert.AreEqual(0, undefinedCount - ignoreUndefinedCount, "'undefined' found on " + tabId);
        }


    }
}
