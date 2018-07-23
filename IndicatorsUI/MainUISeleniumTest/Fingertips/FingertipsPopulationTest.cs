using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DomainObjects;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

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
        [TestMethod]
        public void Test_Practice_Summary_Displayed_For_Gp_Practice()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.PracticeProfiles);
            SelectPopulationTab();

            // Assert
            CheckPracticeSummaryIsPresent();
            CheckPracticeSummaryInformationPopUps();
        }

        private void SelectPopulationTab()
        {
            FingertipsHelper.SelectFingertipsTab(driver, "page-population");
        }

        private void CheckPopulationGraphDivPresent()
        {
            // Wait for the graph to load
            waitFor.FingertipsPopulationGraphToLoad();

            // Test whether the graph is visible
            var graphDiv = driver.FindElement(By.Id("population-chart"));
            Assert.IsNotNull(graphDiv);
        }

        private void CheckPracticeSummaryIsPresent()
        {
            // Wait for the pop-up to load
            waitFor.FingertipsPopulationPracticeSummaryToLoad();

            var box = driver.FindElement(By.Id("registered-persons"));
            TestHelper.AssertTextContains(box.Text, "Registered Persons");
            TestHelper.AssertTextContains(box.Text, "england" , "Area names should be displayed");
        }

        private void CheckPracticeSummaryInformationPopUps()
        {
            // Get the list of information tooltip icons based on the class name
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.ClassName(Classes.InformationTooltip));

            // Loop through the read-only collection of web elements
            foreach(IWebElement element in elements)
            {
                // Check whether the pop-up is visible
                CheckPopUpIsVisible(element);
            }
        }

        private void CheckPopUpIsVisible(IWebElement element)
        {
            // Open the information pop-up
            element.Click();

            // Wait for the pop-up to load
            waitFor.FingertipsPopulationPopupToLoad();

            // Get the reference of the information pop-up displayed
            var box = driver.FindElement(By.Id("infoBox"));
            // Check whether tthe information pop-up displayed contains the text
            TestHelper.AssertTextContains(box.Text, "Indicator Definitions and Supporting Information");

            // Find the close icon of the opened pop-up
            var closeIcon = driver.FindElement(By.ClassName(Classes.CloseIcon));
            // Close the pop-up
            closeIcon.Click();
        }
    }
}
