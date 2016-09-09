using IndicatorsUI.MainUISeleniumTest.Phof;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace IndicatorsUI.MainUISeleniumTest.PHOF
{
    [TestClass]
    public class PhofCompareAreasTest : PhofBaseUnitTest
    {
        private const int Area = 1;
        private const int Value = 5;
        private const int Count = 4;

        [TestMethod]
        public void TestSortByValue()
        {
            // Load compare area tab
            GoToPhofCompareAreas();

            // Sort by value test
            TestExecutor(Value);

            // Sort by count test
            TestExecutor(Count);

            // Sort by area test
            TestExecutor(Area);

        }

        private void TestExecutor(int column)
        {
            var dataBeforeSort = GetColumnData(column);
            SortBy(column);
            var dataAfterSort = GetColumnData(column);

            Assert.AreNotEqual(dataBeforeSort, dataAfterSort);
        }

        private List<string> GetColumnData(int column)
        {
            var noOfRows = driver.FindElements(By.XPath("//*[@id='indicatorDetailsTable']/tbody/tr")).Count;
            var columnData = new List<string>();
            for (var i = 0; i < noOfRows; i++)
            {
                var rowNo = i + 1;
                var data = driver.FindElement(By.XPath("//*[@id='indicatorDetailsTable']/tbody/tr[" + rowNo + "]/td[" + column + "]"));
                columnData.Add(data.Text);
            }
            return columnData;
        }

        private void SortBy(int column)
        {
            var xpath = "//*[@id='indicatorDetailsTable']/thead/tr/th[" + column + "]/a";
            var valueSortButton = driver.FindElement(By.XPath(xpath));
            valueSortButton.Click();
        }

        private void GoToPhofCompareAreas()
        {
            navigateTo.PhofCompareAreasPage();
            new WaitFor(driver).PhofCompareAreasTableToLoad();
        }
    }
}
