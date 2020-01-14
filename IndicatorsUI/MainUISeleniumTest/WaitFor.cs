using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace IndicatorsUI.MainUISeleniumTest
{
    public class WaitFor
    {
        public const int TimeoutLimitInSeconds = 40;
        private IWebDriver _driver;

        public WaitFor(IWebDriver driver)
        {
            _driver = driver;
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
                ThreadWaitInSeconds(waitDurationInSeconds);
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
                var isUnlocked = (bool)(_driver as IJavaScriptExecutor)
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
            var inequalitiesHeader = By.Id("inequalities-container");
            ExpectedElementToBeVisible(inequalitiesHeader);
        }

        public void InequalitiesTrendTableToLoad()
        {
            var inequalitiesTrendTable = By.Id("inequalities-trend-table");
            ExpectedElementToBeVisible(inequalitiesTrendTable);
        }

        public void SearchResultNotFoundToLoad()
        {
            var centralMessage = By.ClassName(Classes.NoSearchResultMessage);
            ExpectedElementToBeVisible(centralMessage);
        }

        public void RankingsToLoad()
        {
            ExpectedElementToBeVisible(By.LinkText(AreaNames.EastSussex));
        }

        public void AreaRankingsToLoad()
        {
            ExpectedElementToBeVisible(By.LinkText("All local authorities"));
        }

        public void FingertipsSpineChartToLoad()
        {
            ExpectedElementToBeVisible(By.Id("single-area-table"));
        }

        public void FingertipsOverviewTabToLoad()
        {
            ExpectedElementToBeVisible(By.Id("left-tartan-table"));
        }

        public void FingertipsBarChartTableToLoad()
        {
            ExpectedElementToBeVisible(By.Id("indicator-details-table"));
        }

        public void FingertipsScatterPlotChartToLoad()
        {
            ExpectedElementToBeVisible(By.Id("scatter-plot-chart"));
        }

        public void FingertipsNearestNeighboursLinksToLoad()
        {
            ExpectedElementToBeVisible(By.Id("nearest-neighbour-links"));
        }

        public void FingertipsAreaSearchResultsPageToLoad()
        {
            ExpectedElementToBeVisible(By.Id("area-results-box"));
            PageToFinishLoading();

            // This is necessary but not clear why
            ThreadWaitInSeconds(0.5);
        }

        public void PhofDomainsToLoad()
        {
            ExpectedElementToBeVisible(By.ClassName("phof-domains-top"));
        }

        public void PhofTrendOptionButtonToLoad()
        {
            ExpectedElementToBeVisible(By.Id("inequalities-tab-option-1"));
        }

        public void PhofCompareAreasTableToLoad()
        {
            ExpectedElementToBeVisible(By.Id("indicator-details-table"));
        }

        public void PhofInequalitiesFilters()
        {
            ExpectedElementToBeVisible(By.Id("inequalities-trend-filters"));
        }

        public void HomePageToLoad()
        {
            ExpectedElementToBeVisible(By.Id("your-account"));
        }

        public void SignInPageToLoad()
        {
            ExpectedElementToBeVisible(By.Id("frm-login-for-fingertips"));
        }

        public void SignOutPageToLoad()
        {
            ExpectedElementToBeVisible(By.ClassName("form-label-bold"));
        }

        public void IndicatorListPageToLoad()
        {
            ExpectedElementToBeVisible(By.Id("lnk-create-new-indicator-list"));
        }

        public void IndicatorListPageTableToLoad()
        {
            ExpectedElementToBeVisible(By.Id("grid-content"));
        }

        public void CreateNewIndicatorListPageToLoad()
        {
            ExpectedElementToBeVisible(By.Id("list-name"));
        }

        public void EditIndicatorListPageToLoad()
        {
            ExpectedElementToBeVisible(By.Id("save-indicator-list-button"));
        }

        public void CopyIndicatorListPopupToLoad()
        {
            ExpectedElementToBeVisible(By.Id("infoBox"));
        }

        public void DeleteIndicatorListPopupToLoad()
        {
            ExpectedElementToBeVisible(By.Id("infoBox"));
        }

        public void ViewIndicatorListPageToLoad()
        {
            ExpectedElementToBeVisible(By.Id("left-tartan-table"));
        }

        public void AreaListPageToLoadForCreateAction()
        {
            ExpectedElementToBeVisible(By.Id("create-new-area-list"));
        }

        public void CreateNewAreaListPageToLoad()
        {
            ExpectedElementToBeVisible(By.Id("btn-save-area-list"));
        }

        public void EditAreaListPageToLoad()
        {
            ExpectedElementToBeVisible(By.Id("btn-save-area-list"));
        }

        public void CopyAreaListPopupToLoad()
        {
            ExpectedElementToBeVisible(By.ClassName("info-box"));
        }

        public void DeleteAreaListPopupToLoad()
        {
            ExpectedElementToBeVisible(By.ClassName("info-box"));
        }

        public void AreaListPageTableToLoad()
        {
            ExpectedElementToBeVisible(By.Id("grid-content"));
        }

        public void AreaListAreasTableToLoad()
        {
            ExpectedElementToBeVisible(By.Id("area-list-table"));
        }

        public void IndicatorsToLoadOnIndicatorListPage()
        {
            ExpectedElementToBeVisible(By.Id("indicator-list"));
        }

        public void ProfilesToLoadOnIndicatorListPage()
        {
            ExpectedElementToBeVisible(By.XPath("//*[@id='profile-list']/option[3]"));
        }

        public void FingertipsTrendsTableToLoad()
        {
            ExpectedElementToBeVisible(By.Id("trend-chart"));
        }

        public void FingertipsEnglandTableToLoad()
        {
            ExpectedElementToBeVisible(By.Id("england-table"));
        }

        public void FingertipsDefinitionsTableToLoad()
        {
            ExpectedElementToBeVisible(By.ClassName("definition-table"));
        }

        public void FingertipsCompareAreasTableToLoad()
        {
            ExpectedElementToBeVisible(By.Id("indicator-details-table"));
        }

        public void FingertipsProfileFrontPageToLoad()
        {
            ExpectedElementToBeVisible(By.Id("introduction"));
        }

        public void FingertipsPopulationGraphToLoad()
        {
            ExpectedElementToBeVisible(By.Id("population-chart"));
        }

        public void FingertipsPopulationPopupToLoad()
        {
            ExpectedElementToBeVisible(By.Id("infoBox"));
        }

        public void FingertipsPopulationPracticeSummaryToLoad()
        {
            ExpectedElementToBeVisible(By.Id("registered-persons"));
        }

        public void FingertipsCategoryTypeDescriptionsToLoad()
        {
            ExpectedElementToBeVisible(By.ClassName(Classes.InformationToolTipWithPosition));
        }

        public void FingertipsCategoryTypeDescriptionPopupToLoad()
        {
            ExpectedElementToBeVisible(By.ClassName("info-box"));
        }

        public void FingertipsAreaTypeToLoad()
        {
            ExpectedElementToBeVisible(By.Id("areaTypes"));
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
            new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutLimitInSeconds))
                .Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(element));
        }

        /// <summary>
        /// Wait for an element to be present on the page.
        /// </summary>
        public void ExpectedElementToBeVisible(By element)
        {
            new WebDriverWait(_driver, TimeSpan.FromSeconds(TimeoutLimitInSeconds))
                .Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(element));
        }

        public void PageToFinishLoading()
        {

            const double totalSecondsToCheckFor = TimeoutLimitInSeconds;
            double secondsCheckedFor = 0;
            while (secondsCheckedFor < totalSecondsToCheckFor)
            {
                if (((IJavaScriptExecutor)_driver).ExecuteScript(
                    "return document.readyState").Equals("complete"))
                {
                    return;
                }

                // Wait until next try
                const double waitDurationInSeconds = 0.2;
                ThreadWaitInSeconds(waitDurationInSeconds);
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
        public static void ThreadWaitInSeconds(double seconds)
        {
            Thread.Sleep((int)(seconds * 1000));
        }
    }
}