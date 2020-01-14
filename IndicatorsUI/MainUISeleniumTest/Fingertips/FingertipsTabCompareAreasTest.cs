using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.Generic;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabCompareAreasTest : FingertipsBaseUnitTest
    {
        private const int Area = 1;
        private const int Value = 5;
        private const int Count = 4;

        [TestMethod]
        public void Test_Compare_Areas_Column_Sorting()
        {
            GoToPhofCompareAreas();

            TestSortByColumn(Value);
            TestSortByColumn(Count);
            TestSortByColumn(Area);
        }

        [TestMethod]
        public void Test_Clicking_Recent_Trends_Icon_Navigates_To_Trends_Tab()
        {
            navigateTo.PhofHealthImprovementDomain();
            waitFor.FingertipsOverviewTabToLoad();

            navigateTo.PhofCompareAreasTab();
            waitFor.FingertipsBarChartTableToLoad();

            // Click England trend icon
            driver.FindElement(By.Id("indicator-details-table"))
                .FindElement(By.TagName("tbody"))
                .FindElements(By.TagName("tr"))[2]
                .FindElements(By.TagName("td"))[1].Click();

            // Success if trends table loads
            waitFor.FingertipsTrendsTableToLoad();
        }

        [TestMethod]
        public void Test_All_In_England_Not_Displayed_For_Small_Areas()
        {
            var parameters = new HashParameters();
            parameters.AddAreaTypeId(AreaTypeIds.GpPractice);
            parameters.AddTabId(TabIds.CompareAreas);
            GoToUrl(ProfileUrlKeys.PracticeProfiles, parameters);

            // Assert: All in England not visible
            Assert.AreEqual(0, driver.FindElements(By.Id("compare-area-tab-option-1")).Count,
                "All in England option should not be displayed for small areas");
        }

        [TestMethod]
        public void Test_Correct_Significance_Level_Values_Displayed()
        {
            navigateTo.FingertipsDataForProfile("general-practice");
            navigateTo.PracticeProfilesDefinitionsTab();

            var indicatorName = "% who have a positive experience of their GP practice";
            fingertipsHelper.SelectIndicatorByName(indicatorName);

            var benchmarkValue = GetBenchmarkValue();
            
            navigateTo.PracticeProfilesCompareAreasTab();

            var sigLevelColumnHeader =
                driver.FindElement(By.XPath("//*[@id='indicator-details-table']/thead/tr/th[6]/div[1]")).Text;

            Assert.IsTrue(sigLevelColumnHeader.Contains(benchmarkValue), "The significance level does not match.");
        }

        private string GetBenchmarkValue()
        {
            var bodyElement = driver.FindElement(By.XPath("//*[@id='metadata-container']/ft-metadata-table/div/table"));
            var rowElements = bodyElement.FindElements(By.TagName("tr"));
            foreach (var rowElement in rowElements)
            {
                var columnElements = rowElement.FindElements(By.TagName("td"));
                for (int index = 0; index < columnElements.Count; index++)
                {
                    if (columnElements[index].Text == "Benchmarking significance level")
                    {
                        return columnElements[index + 1].Text;
                    }
                }
            }

            return string.Empty;
        }

        private void GoToUrl(string profileKey, HashParameters parameters)
        {
            navigateTo.GoToUrl(profileKey + parameters.HashParameterString);
            waitFor.PageToFinishLoading();
            waitFor.AjaxLockToBeUnlocked();
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
            var noOfRows = driver.FindElements(By.XPath("//*[@id='indicator-details-table']/tbody/tr")).Count;
            var columnData = new List<string>();
            for (var i = 0; i < noOfRows; i++)
            {
                var rowNo = i + 1;
                var data = driver.FindElement(By.XPath("//*[@id='indicator-details-table']/tbody/tr[" + rowNo + "]/td[" + Area + "]"));
                columnData.Add(data.Text);
            }
            return string.Concat(columnData);
        }

        private void SortBy(int column)
        {
            var xpath = "//*[@id='indicator-details-table']/thead/tr/th[" + column + "]/a";
            var valueSortButton = driver.FindElement(By.XPath(xpath));
            valueSortButton.Click();
            waitFor.AjaxLockToBeUnlocked();
        }

        private void GoToPhofCompareAreas()
        {
            // Go to compare areas tab
            navigateTo.PhofCompareAreasTab();
            waitFor.PhofCompareAreasTableToLoad();

            // Select an indicator with counts (in wider determinants of health domain)
            driver.FindElement(By.Id("domain1000041")).Click();
            waitFor.PhofCompareAreasTableToLoad();
        }
    }
}
