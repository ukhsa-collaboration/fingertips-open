using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DomainObjects;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsPopulationTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Population_Graph_Displayed_When_Data_Available()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.HealthProfiles);
            SelectPopulationTab();
            CheckPopulationGraphDivPresent();
        }

        /// <summary>
        /// Excluded from Jenkins until GP data is available in a profile.
        /// </summary>
        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void Test_Practice_Summary_Displayed_For_Gp_Practice()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.AmrLocalIndicators);

            FingertipsHelper.SelectAreaType(driver, AreaTypeIds.GpPractice);
            FingertipsHelper.SelectDomain(driver, GroupIds.AMR_AntibioticPrescribing);
            SelectPopulationTab();

            // Assert
            CheckPopulationGraphDivPresent();
            CheckPracticeSummaryIsPresent();
        }

        private void SelectPopulationTab()
        {
            FingertipsHelper.SelectFingertipsTab(driver, "page-population");
        }

        private void CheckPopulationGraphDivPresent()
        {
            var graphDiv = driver.FindElement(By.Id("population-chart"));
            Assert.IsNotNull(graphDiv);
        }

        private void CheckPracticeSummaryIsPresent()
        {
            var box = driver.FindElement(By.Id("population-table-box"));
            TestHelper.AssertTextContains(box.Text, "Registered Persons");
        }
    }
}
