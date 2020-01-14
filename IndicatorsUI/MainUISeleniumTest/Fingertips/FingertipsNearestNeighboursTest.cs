using System.Collections.ObjectModel;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsNearestNeighboursTest : FingertipsBaseUnitTest
    {
        const int ExtraColumns = 3;
        /// <summary>
        ///     Checks if NN link loaded correctly
        /// </summary>
        [TestMethod]
        public void Check_Nearest_Neighbours_Link()
        {
            navigateTo.PhofOverviewTab();

            var selectArea = fingertipsHelper.GetSelectedAreaNameFromMenu();
            var selectedNearestNeighbourArea = driver.FindElement(By.Id("nearest-neighbour-link")).Text;

            Assert.AreEqual("CIPFA nearest neighbours to " + selectArea, selectedNearestNeighbourArea);
        }

        /// <summary>
        ///     Check if NN menu loaded correctly after NN link click
        /// </summary>
        [TestMethod]
        public void Check_Nearest_Neighbours_Menu_Loaded_Correctly()
        {
            navigateTo.PhofOverviewTab();
            var selectedAreaName = fingertipsHelper.GetSelectedAreaNameFromMenu();
            fingertipsHelper.SelectNearestNeighbours();

            // Check the back button text
            var exitNearestNeighboursText = driver.FindElement(By.Id("nearest-neighbour-links")).Text;
            TestHelper.AssertTextContains(exitNearestNeighboursText, "Exit nearest neighbours");
            TestHelper.AssertTextContains(exitNearestNeighboursText, "More information");

            // Check NN text loaded correctly
            var selectAreaText = driver.FindElement(By.Id("nearest-neighbour-header")).Text;
            Assert.IsTrue(selectAreaText.StartsWith(selectedAreaName + " and its CIPFA nearest neighbours"));
        }

        /// <summary>
        ///     Right hand table should only have 16 columns
        /// </summary>
        [TestMethod]
        public void Test_Nearest_Neighbours_For_An_Area_Loaded_Correctly()
        {
            navigateTo.PhofOverviewTab();
            fingertipsHelper.SelectNearestNeighbours();

            // Assert: Expected number of columns
            var columns = GetRightTableColumns();
            Assert.AreEqual(16, columns.Count - ExtraColumns);
        }

        /// <summary>
        ///     Test the back to menu link
        /// </summary>
        [TestMethod]
        public void Test_Exit_Nearest_Neighbours_Link()
        {
            navigateTo.PhofOverviewTab();
            fingertipsHelper.SelectNearestNeighbours();
            fingertipsHelper.ExitNearestNeighbours();

            // Assert: nearest neighbours is no longer displayed
            var columns = GetRightTableColumns();
            Assert.AreEqual(9, columns.Count - ExtraColumns);     
        }

        /// <summary>
        ///     Test the nearest neighbours benchmark
        /// </summary>
        [TestMethod]
        public void Test_Nearest_Neighbours_Benchmark()
        {
            // Navigate to the overview tab
            navigateTo.PhofOverviewTab();

            // Select nearest neighbours link
            fingertipsHelper.SelectNearestNeighbours();

            // Check whether benchmark dropdown has nearest neighbours option
            var benchmarkDropdown = driver.FindElement(By.Id("comparator"));
            var nnOption = benchmarkDropdown.FindElement(By.Id("subnational-benchmark"));
            Assert.AreEqual("nearest neighbours", nnOption.Text.ToLower(), "Nearest neighbours benchmark option is not available.");
            nnOption.Click();
            waitFor.FingertipsOverviewTabToLoad();

            // Check whether nearest neighbours column is visible in the table
            var nnColumn = driver.FindElement(By.XPath("//*[@id='left-tartan-table']/thead/tr/th[5]/img"));
            Assert.IsTrue(nnColumn.GetAttribute("src").ToLower().Contains("neighbours"), "Nearest neighbours column is not visible.");
        }

        private ReadOnlyCollection<IWebElement> GetRightTableColumns()
        {
            var columns = driver.FindElements(By.XPath("//*[@id='rightTartanTable']/thead/tr/th"));
            return columns;
        }
    }
}