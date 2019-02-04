using OpenQA.Selenium;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest
{
    public class NavigateTo
    {
        public static string BaseUrl = AppConfig.Instance.BridgeWsUrl;

        private IWebDriver driver;
        private WaitFor waitFor;

        public NavigateTo(IWebDriver driver)
        {
            this.driver = driver;
            waitFor = new WaitFor(driver);
        }

        public void FingertipsDataForProfile(string profileUrlKey)
        {
            GoToUrl("profile/" + profileUrlKey + "/data");
            waitFor.FingertipsTartanRugToLoad();
        }

        public void FingertipsFrontPageForProfile(string profileUrlKey)
        {
            GoToUrl("profile/" + profileUrlKey);
            waitFor.FingertipsProfileFrontPageToLoad();
        }

        public void JavaScriptTestPage(string testPage)
        {
            GoToUrl("test/" + testPage);
        }

        public void PublicHealthDashboardMap()
        {
            GoToUrl("topic/public-health-dashboard/map-with-data");
            waitFor.GoogleMapToLoad();
        }

        public void PublicHealthDashboardRankings()
        {
            GoToUrl("topic/public-health-dashboard/comparisons");
        }

        public void PublicHealthDashboardHome()
        {
            GoToUrl("topic/public-health-dashboard");
            waitFor.GoogleMapToLoad();
        }

        public void FingertipsIndicatorSearchResults(string searchText)
        {
            GoToUrl("search/" + searchText);
        }

        public void HomePage()
        {
            GoToUrl("/");
        }

        public void PhofInequalities()
        {
            PhofDataTab(TabIds.Inequalities);
            waitFor.InequalitiesTabToLoad();
        }

        public void PhofCompareAreas()
        {
            PhofDataTab(TabIds.CompareAreas);
            waitFor.FingertipsBarChartTableToLoad();
        }

        public void PhofTartanRug()
        {
            PhofDataTab(TabIds.TartanRug);
            waitFor.FingertipsTartanRugToLoad();
        }

        public void PhofAreaProfile()
        {
            PhofDataTab(TabIds.AreaProfile);
            waitFor.FingertipsSpineChartToLoad();
        }

        public void FingertipsTartanRug()
        {
            DeveloperTestProfile(TabIds.TartanRug);
            waitFor.FingertipsAreaTypeToLoad();
        }

        public void FingertipsScatterplot()
        {
            DeveloperTestProfile(TabIds.ScatterPlot);
            waitFor.FingertipsAreaTypeToLoad();
        }

        public void FingertipsMap()
        {
            DeveloperTestProfile(TabIds.Map);
            waitFor.FingertipsAreaTypeToLoad();
        }

        public void FingertipsTrends()
        {
            DeveloperTestProfile(TabIds.Trends);
            waitFor.FingertipsAreaTypeToLoad();
        }

        public void FingertipsCompareAreas()
        {
            DeveloperTestProfile(TabIds.CompareAreas);
            waitFor.FingertipsAreaTypeToLoad();
        }

        public void FingertipsAreaProfiles()
        {
            DeveloperTestProfile(TabIds.AreaProfile);
            waitFor.FingertipsAreaTypeToLoad();
        }

        public void FingertipsPopulation()
        {
            DeveloperTestProfile(TabIds.Population);
            waitFor.FingertipsAreaTypeToLoad();
        }

        public void FingertipsInequalities()
        {
            DeveloperTestProfile(TabIds.Inequalities);
            waitFor.FingertipsAreaTypeToLoad();
        }

        public void FingertipsEngland()
        {
            DeveloperTestProfile(TabIds.England);
            waitFor.FingertipsAreaTypeToLoad();
        }

        public void GoToUrl(string relativeUrl)
        {
            driver.Navigate().GoToUrl(BaseUrl + relativeUrl);
        }

        private void PhofDataTab(int tabId)
        {
            GoToUrl("profile/" + ProfileUrlKeys.Phof + "/data#page/" + tabId);
        }

        private void DeveloperTestProfile(int tabId)
        {
            GoToUrl("profile/" + ProfileUrlKeys.DeveloperTestProfile + "/data#page/" + tabId);
        }
    }
}
