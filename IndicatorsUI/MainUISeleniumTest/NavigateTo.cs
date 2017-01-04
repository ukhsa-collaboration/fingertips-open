using OpenQA.Selenium;
using Profiles.DataAccess;
using Profiles.DomainObjects;

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

        public void FingertipsDataForPracticeProfiles(string profileUrlKey)
        {
            GoToUrl("profile/" + profileUrlKey + "/data");
            waitFor.GoogleMapToLoad();
        }

        public void JavaScriptTestPage(string testPage)
        {
            GoToUrl("test/" + testPage);
        }

        public void MortalityHome()
        {
            GoToUrl("topic/mortality");
            waitFor.GoogleMapToLoad();
        }

        public void MortalityRankings()
        {
            GoToUrl("topic/mortality/comparisons");
        }

        public void DiabetesRankings()
        {
            GoToUrl("topic/diabetes/comparisons");
        }

        public void DiabetesHome()
        {
            GoToUrl("topic/diabetes");
            waitFor.GoogleMapToLoad();
        }

        public void DiabetesPracticeDetails(string hashParameters)
        {
            GoToUrl("topic/diabetes/practice-details" + hashParameters);
        }

        public void HypertensionPracticeDetails(string hashParameters)
        {
            GoToUrl("topic/hypertension/practice-details" + hashParameters);
        }

        public void FingertipsIndicatorSearchResults(string searchText)
        {
            GoToUrl("search/" + searchText);
        }

        public void PhofHomePage()
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

        public void GoToUrl(string relativeUrl)
        {
            driver.Navigate().GoToUrl(BaseUrl + relativeUrl);
        }

        private void PhofDataTab(int tabId)
        {
            GoToUrl("profile/" + ProfileUrlKeys.Phof + "/data#page/" + tabId);
        }
    }
}
