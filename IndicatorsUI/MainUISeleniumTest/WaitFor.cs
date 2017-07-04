using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace IndicatorsUI.MainUISeleniumTest
{
    public class WaitFor
    {
        private IWebDriver driver;
        public const int TimeoutLimitInSeconds = 30;

        public WaitFor(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void ElementToContainText(IWebElement element, string text)
        {
            const double totalSecondsToCheckFor = TimeoutLimitInSeconds;
            double secondsCheckedFor = 0;
            while (secondsCheckedFor < totalSecondsToCheckFor)
            {
                if (element.Text.ToLower().Contains(text.ToLower()))
                {
                    return;
                }

                // Wait until next try
                const double waitDurationInSeconds = 0.2;
                ThreadWait(waitDurationInSeconds);
                secondsCheckedFor += waitDurationInSeconds;
            }

            Assert.Fail("Element did not contain expected text after waiting");
        }

        public void AjaxLockToBeUnlocked()
        {
            const int timeToWaitInMilliseconds = 100;
            const int maxCheckCount = (TimeoutLimitInSeconds * 1000) / timeToWaitInMilliseconds;
            int checks = 0;
            while (checks < maxCheckCount)
            {
                var isUnlocked = (bool)(driver as IJavaScriptExecutor)
                    .ExecuteScript("return FT.ajaxLock === null;");
                if (isUnlocked)
                {
                    break;
                }
                checks++;
                Thread.Sleep(timeToWaitInMilliseconds);
            }
        }

        public void InequalitiesTabToLoad()
        {
            var inequalitiesHeader = By.Id("inequalities-header");
            ExpectedElementToBeVisible(inequalitiesHeader);
        }

        public void SearchResultNotFoundToLoad()
        {
            var centralMessage = By.ClassName(Classes.NoSearchResultMessage);
            ExpectedElementToBeVisible(centralMessage);
        }

        public void DistrictUaRankingsToLoad()
        {
            ExpectedElementToBeVisible(By.LinkText(AreaNames.DistrictUaTendring));
        }

        public void CcgRankingsToLoad()
        {
            ExpectedElementToBeVisible(By.LinkText(AreaNames.CcgWestLancashire));
        }

        public void CountyUaRankingsToLoad()
        {
            ExpectedElementToBeVisible(By.LinkText(AreaNames.CountyUaBathAndNorthEastSomerset));
        }

        public void MortalityAreaDetailsToLoad()
        {
            ExpectedElementToBeVisible(By.Id("national_verdict"));
        }

        public void AutoCompleteSearchResultsToBeDisplayed()
        {
            ExpectedElementToBeVisible(By.Id(LongerLivesIds.AreaSearchAutocompleteOptions));
            GoogleMapToLoad();
        }

        public void HealthierLivesSearchResultsToLoad()
        {
            ExpectedElementToBeVisible(By.ClassName("national"));
            GoogleMapToLoad();
        }

        public void PracticeRankingsForWestLancashireCcgToLoad()
        {
            ExpectedElementToBeVisible(By.LinkText("Parkgate Surgery"));
        }

        public void PracticeDetailsToLoad()
        {
            // ImplicitlyWait for element only found on practice details page
            ExpectedElementToBeVisible(By.ClassName("area-details"));

            // ImplicitlyWait until the table rows have been created
            ExpectedElementToBeVisible(
                By.XPath("//*[@id=\"diabetes-rankings-table\"]/tbody/tr[1]/td[1]"));
        }

        public void FingertipsSpineChartToLoad()
        {
            ExpectedElementToBeVisible(By.Id("single-area-table"));
        }

        public void FingertipsTartanRugToLoad()
        {
            ExpectedElementToBeVisible(By.Id("left-tartan-table"));
        }

        public void FingertipsBarChartTableToLoad()
        {
            ExpectedElementToBeVisible(By.Id("indicator-details-table"));
        }

        public void FingertipsScatterPlotChartToLoad()
        {
            ExpectedElementToBeVisible(By.Id("scatter-plot-chart-box"));
        }

        public void ChosenSearch()
        {
            ExpectedElementToBeVisible(By.ClassName("chosen-search"));
        }

        public void FingertipsNearestNeighboursMenuToLoad()
        {
            ExpectedElementToBeVisible(By.Id("nearest-neighbour-links"));
        }

        public void FingertipsNearestNeighboursTartunRugToLoad()
        {
            ExpectedElementToBeVisible(By.Id("rightTartanTable"));
        }

        public void FingertipsAreaSearchResultsPageToLoad()
        {
            ExpectedElementToBeVisible(By.Id("area-results-box"));
            PageToFinishLoading();

            // This is necessary but not clear why
            ThreadWait(0.5);
        }

        public void PhofDomainsToLoad()
        {
            ExpectedElementToBeVisible(By.ClassName("phof-domains"));
        }

        public void PhofTrendOptionButtonToLoad()
        {
            ExpectedElementToBeVisible(By.ClassName("tab-options"));
        }

        public void PhofCompareAreasTableToLoad()
        {
            ExpectedElementToBeVisible(By.Id("indicator-details-table"));
        }

        public void PhofInequalitiesFilters()
        {
            ExpectedElementToBeVisible(By.Id("inequalities-trend-filters"));
        }

        public void PracticeProfilesSearchTabToLoad()
        {
            ExpectedElementToBeVisible(By.Id("searchText"));
        }

        public void PracticeProfilesSummaryTabToLoad()
        {
            ExpectedElementToBeVisible(By.Id("ethnicity"));
        }

        public void PracticeProfilesSpineChartsTabToLoad()
        {
            ExpectedElementToBeVisible(By.Id("spine-table-box"));
        }

        public void MortalityAreaDetailRankingToLoad()
        {
            ExpectedElementToBeVisible(By.Id("main_ranking"));
        }

        public void MortalityRankingsTableToLoad()
        {
            ExpectedElementToBeVisible(By.Id("mortality-rankings-table"));
        }

        public void PracticeProfilesBarChartsTabToLoad()
        {
            ExpectedElementToBeVisible(By.Id("barBox"));
            ExpectedElementToBeVisible(By.Id("indicatorMenu"));
        }

        public void FingertipsTrendsTableToLoad()
        {
            ExpectedElementToBeVisible(By.Id("trendTable0"));
        }

        public void FingertipsProfileFrontPageToLoad()
        {
            ExpectedElementToBeVisible(By.Id("introduction"));
        }

        public void HealthInterventionPageToLoad()
        {
            ExpectedElementToBeVisible(By.ClassName("useful_links"));
        }

        public void GoogleMapToLoad()
        {
            /*  http://stackoverflow.com/questions/5779102/selenium-tests-for-google-maps
             *  mnoprint is an anchor tag for google maps which shows terms of use. 
             *  ImplicitlyWait till it is loaded into the DOM tree.  
             */
            ExpectedElementToBePresent(By.ClassName("gmnoprint"));
        }

        /// <summary>
        /// Wait for an element to be present on the page.
        /// </summary>
        public void ExpectedElementToBePresent(By element)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutLimitInSeconds))
                .Until(ExpectedConditions.ElementExists(element));
        }

        /// <summary>
        /// Wait for an element to be present on the page.
        /// </summary>
        public void ExpectedElementToBeVisible(By element)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutLimitInSeconds))
                .Until(ExpectedConditions.ElementIsVisible(element));
        }

        public void PageToFinishLoading()
        {

            const double totalSecondsToCheckFor = TimeoutLimitInSeconds;
            double secondsCheckedFor = 0;
            while (secondsCheckedFor < totalSecondsToCheckFor)
            {
                if (((IJavaScriptExecutor)driver).ExecuteScript(
                    "return document.readyState").Equals("complete"))
                {
                    return;
                }

                // Wait until next try
                const double waitDurationInSeconds = 0.2;
                ThreadWait(waitDurationInSeconds);
                secondsCheckedFor += waitDurationInSeconds;
            }

            Assert.Fail("Document ready state never became complete");
        }

        /// <summary>
        /// Wait for the inequalities trend chart to be loaded
        /// </summary>
        public void InequalitiesTrendChart()
        {
            ExpectedElementToBeVisible(By.Id("inequalities-trend-chart"));
        }

        /// <summary>
        /// DO NOT USE unless you really have to!!
        /// Use ExpectedElementToBeVisible to be visible instead.
        /// </summary>
        public static void ThreadWait(double seconds)
        {
            Thread.Sleep((int)(seconds * 1000));
        }
    }
}