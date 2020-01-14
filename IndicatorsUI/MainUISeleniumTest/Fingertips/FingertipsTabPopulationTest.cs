using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabPopulationTest : FingertipsBaseUnitTest
    {
        private const int WaitForHighchartsToRenderChartInSeconds = 2;

        [TestMethod]
        public void Test_Population_Chart_Displayed_When_Data_Available()
        {
            navigateTo.PopulationTab();
            CheckPopulationChartPresent();
        }

        [TestMethod]
        public void Test_Practice_Summary_Displayed_For_Gp_Practice()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.PracticeProfiles);
            SelectPopulationTab();

            // Assert
            CheckPracticeSummaryIsPresent();
            CheckPracticeSummaryInformationPopUps();
        }

        [TestMethod]
        public void Test_SubNational_Population_Not_Displayed_For_Nearest_Neighbours()
        {
            navigateTo.PopulationTab();

            // Make sure district UA is selected
            fingertipsHelper.SelectAreaType(AreaTypeIds.DistrictAndUAPreApr2019);
            CheckSubNationalLegendDisplayed();

            // Switch to nearest neighbours mode
            fingertipsHelper.SelectNearestNeighbours();
            CheckSubNationalLegendIsHidden();
        }

        private void CheckSubNationalLegendDisplayed()
        {
            // Extra wait for Jenkins
            WaitFor.ThreadWaitInSeconds(WaitForHighchartsToRenderChartInSeconds);

            Assert.IsTrue(IsSubNationalLegendDisplayed(), "Subnational legend should be displayed");
        }

        private void CheckSubNationalLegendIsHidden()
        {
            // Extra wait for Jenkins
            WaitFor.ThreadWaitInSeconds(WaitForHighchartsToRenderChartInSeconds);

            Assert.IsFalse(IsSubNationalLegendDisplayed(), "Subnational legend should not be displayed");
        }

        private bool IsSubNationalLegendDisplayed()
        {
            var isSubNationalLegendDisplayed = driver.FindElements(By.TagName("tspan"))
                .Any(tag => tag.Text.Contains("East Midlands region"));
            return isSubNationalLegendDisplayed;
        }

        private void SelectPopulationTab()
        {
            fingertipsHelper.SelectTab("page-population");
        }

        private void CheckPopulationChartPresent()
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
            TestHelper.AssertTextContains(box.Text, "england", "Area names should be displayed");
        }

        private void CheckPracticeSummaryInformationPopUps()
        {
            // Get the list of information tooltip icons based on the class name
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.ClassName(Classes.InformationTooltip));

            // Loop through the read-only collection of web elements
            foreach (IWebElement element in elements)
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
            // Check whether the information pop-up displayed contains the text
            TestHelper.AssertTextContains(box.Text, "Indicator Definitions and Supporting Information");

            // Find the close icon of the opened pop-up
            var closeIcon = driver.FindElement(By.ClassName(Classes.CloseIcon));
            // Close the pop-up
            closeIcon.Click();
        }
    }
}
