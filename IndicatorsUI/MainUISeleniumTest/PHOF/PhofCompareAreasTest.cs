using IndicatorsUI.MainUISeleniumTest.Phof;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Threading;

namespace IndicatorsUI.MainUISeleniumTest.PHOF
{
    [TestClass]
    public class PhofCompareAreasTest : PhofBaseUnitTest
    {
        private const int Area = 1;
        private const int Value = 5;
        private const int Count = 4;

        [TestMethod]
        public void TestCompareAreasColumnSorting()
        {
            // Load compare area tab
            GoToPhofCompareAreas();

            // Sort by value test
            TestSortByColumn(Value);

            // Sort by count test
            TestSortByColumn(Count);

            // Sort by area test
            TestSortByColumn(Area);
        }


        private void TestSortByColumn(int column)
        {
            var dataBeforeSort = GetColumnData();
            SortBy(column);
            var dataAfterSort = GetColumnData();

            Assert.AreNotEqual(dataBeforeSort, dataAfterSort);
        }

        private string GetColumnData()
        {
            var noOfRows = driver.FindElements(By.XPath("//*[@id='indicatorDetailsTable']/tbody/tr")).Count;
            var columnData = new List<string>();
            for (var i = 0; i < noOfRows; i++)
            {
                var rowNo = i + 1;
                var data = driver.FindElement(By.XPath("//*[@id='indicatorDetailsTable']/tbody/tr[" + rowNo + "]/td[" + Area + "]"));
                columnData.Add(data.Text);
            }
            return string.Concat(columnData);
        }

        private void SortBy(int column)
        {
            var xpath = "//*[@id='indicatorDetailsTable']/thead/tr/th[" + column + "]/a";
            var valueSortButton = driver.FindElement(By.XPath(xpath));
            valueSortButton.Click();
            waitFor.AjaxLockToBeUnlocked();
        }

        private void GoToPhofCompareAreas()
        {
            navigateTo.PhofCompareAreas();
            waitFor.PhofCompareAreasTableToLoad();
        }
    }
}
