using MainUISeleniumTest.Fingertips;
using OpenQA.Selenium;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace MainUISeleniumTest
{
    public class NavigateTo
    {
        public static string BaseUrl = AppConfig.Instance.BridgeWsUrl;

        private IWebDriver driver;

        public NavigateTo(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void FingertipsDataForProfile(string profileUrlKey)
        {
            GoToUrl("profile/" + profileUrlKey + "/data");
            new WaitFor(driver).FingertipsTartanRugToLoad();
        }

        public void FingertipsDataForPracticeProfiles(string profileUrlKey)
        {
            GoToUrl("profile/" + profileUrlKey + "/data");
            new WaitFor(driver).GoogleMapToLoad();
        }

        public void JavaScriptTestPage(string testPage)
        {
            GoToUrl("test/" + testPage);
        }

        public void MortalityHome()
        {
            GoToUrl("topic/mortality");
            new WaitFor(driver).GoogleMapToLoad();
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
            new WaitFor(driver).GoogleMapToLoad();
        }

        public void DiabetesPracticeDetails(string hashParameters)
        {
            GoToUrl("topic/diabetes/practice-details" + hashParameters);
        }

        public void FingertipsIndicatorSearchResults(string searchText)
        {
            GoToUrl("search/" + searchText);
        }

        public void PhofHomePage()
        {
            GoToUrl("/");
        }

        public void PhofDataPage()
        {
            FingertipsDataForProfile(ProfileUrlKeys.Phof);
        }

        public void GoToUrl(string relativeUrl)
        {
            driver.Navigate().GoToUrl(BaseUrl + relativeUrl);
        }
    }
}
