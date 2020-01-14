using OpenQA.Selenium;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest
{
    public class NavigateTo
    {
        public static string BaseUrl = AppConfig.Instance.BridgeWsUrl;

        private readonly IWebDriver _driver;
        private readonly WaitFor _waitFor;

        public NavigateTo(IWebDriver driver)
        {
            _driver = driver;
            _waitFor = new WaitFor(driver);
        }

        public void FingertipsDataForProfile(string profileUrlKey)
        {
            GoToUrl("profile/" + profileUrlKey + "/data");
            _waitFor.FingertipsOverviewTabToLoad();
            _waitFor.AjaxLockToBeUnlocked();
        }

        public void FingertipsFrontPageForProfile(string profileUrlKey)
        {
            GoToUrl("profile/" + profileUrlKey);
            _waitFor.FingertipsProfileFrontPageToLoad();
        }

        public void JavaScriptTestPage(string testPage)
        {
            GoToUrl("test/" + testPage);
        }

        public void PublicHealthDashboardMap()
        {
            GoToUrl("topic/public-health-dashboard/map-with-data");
            _waitFor.GoogleMapToLoad();
        }

        public void PublicHealthDashboardRankings()
        {
            GoToUrl("topic/public-health-dashboard/comparisons");
        }

        public void PublicHealthDashboardHome()
        {
            GoToUrl("topic/public-health-dashboard");
            _waitFor.GoogleMapToLoad();
        }

        public void FingertipsIndicatorSearchResults(string searchText)
        {
            GoToUrl("search/" + searchText);
        }

        public void HomePage()
        {
            GoToUrl("/");
        }

        public void PhofHealthImprovementDomain()
        {
            GoToUrl("profile/" + ProfileUrlKeys.Phof + "/data#page/0/gid/" + GroupIds.Phof_HealthImprovement);
        }

        public void PhofInequalitiesTab()
        {
            NavigateToDataTab(ProfileUrlKeys.Phof, TabIds.Inequalities);
            _waitFor.InequalitiesTabToLoad();
        }

        public void PhofCompareAreasTab()
        {
            NavigateToDataTab(ProfileUrlKeys.Phof, TabIds.CompareAreas);
            _waitFor.FingertipsBarChartTableToLoad();
        }

        public void PhofOverviewTab()
        {
            NavigateToDataTab(ProfileUrlKeys.Phof, TabIds.Overview);
            _waitFor.FingertipsOverviewTabToLoad();
        }

        public void OverviewTab()
        {
            NavigateToDataTab(ProfileUrlKeys.DevelopmentProfileForTesting, TabIds.Overview);
            _waitFor.FingertipsAreaTypeToLoad();
            _waitFor.AjaxLockToBeUnlocked();
        }

        public void CompareIndicatorsTab()
        {
            NavigateToDataTab(ProfileUrlKeys.DevelopmentProfileForTesting, TabIds.CompareIndicators);
            _waitFor.FingertipsAreaTypeToLoad();
            _waitFor.AjaxLockToBeUnlocked();
        }

        public void MapTab()
        {
            NavigateToDataTab(ProfileUrlKeys.DevelopmentProfileForTesting, TabIds.Map);
            _waitFor.FingertipsAreaTypeToLoad();
            _waitFor.AjaxLockToBeUnlocked();
        }

        public void TrendsTab()
        {
            NavigateToDataTab(ProfileUrlKeys.DevelopmentProfileForTesting, TabIds.Trends);
            _waitFor.FingertipsAreaTypeToLoad();
            _waitFor.AjaxLockToBeUnlocked();
        }

        public void CompareAreasTab()
        {
            NavigateToDataTab(ProfileUrlKeys.DevelopmentProfileForTesting, TabIds.CompareAreas);
            _waitFor.FingertipsAreaTypeToLoad();
            _waitFor.AjaxLockToBeUnlocked();
        }

        public void AreaProfilesTab()
        {
            NavigateToDataTab(ProfileUrlKeys.DevelopmentProfileForTesting, TabIds.AreaProfile);
            _waitFor.FingertipsAreaTypeToLoad();
            _waitFor.AjaxLockToBeUnlocked();
        }

        public void PopulationTab()
        {
            NavigateToDataTab(ProfileUrlKeys.DevelopmentProfileForTesting, TabIds.Population);
            _waitFor.FingertipsAreaTypeToLoad();
            _waitFor.AjaxLockToBeUnlocked();
        }

        public void InequalitiesTab()
        {
            NavigateToDataTab(ProfileUrlKeys.DevelopmentProfileForTesting, TabIds.Inequalities);
            _waitFor.FingertipsAreaTypeToLoad();
            _waitFor.AjaxLockToBeUnlocked();
        }

        public void EnglandTab()
        {
            NavigateToDataTab(ProfileUrlKeys.DevelopmentProfileForTesting, TabIds.England);
            _waitFor.FingertipsAreaTypeToLoad();
            _waitFor.AjaxLockToBeUnlocked();
        }

        public void PracticeProfilesDefinitionsTab()
        {
            NavigateToDataTab(ProfileUrlKeys.PracticeProfiles, TabIds.Definitions);
            _waitFor.FingertipsDefinitionsTableToLoad();
            _waitFor.AjaxLockToBeUnlocked();
        }

        public void PracticeProfilesCompareAreasTab()
        {
            NavigateToDataTab(ProfileUrlKeys.PracticeProfiles, TabIds.CompareAreas);
            _waitFor.FingertipsCompareAreasTableToLoad();
            _waitFor.AjaxLockToBeUnlocked();
        }

        private void NavigateToDataTab(string profileUrlKey, int tabId)
        {
            var url = string.Format("profile/{0}/data#page/{1}", profileUrlKey, tabId);
            GoToUrl(url);
        }

        public void GoToUrl(string relativeUrl)
        {
            _driver.Navigate().GoToUrl(BaseUrl + relativeUrl);
        }
    }
}
