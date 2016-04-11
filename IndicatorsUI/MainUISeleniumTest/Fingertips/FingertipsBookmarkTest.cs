using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Profiles.DataAccess;


namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsBookmarkTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void TestIndicatorBookmark()
        {
            LoadUrl(driver);

            waitFor.FingertipsBarChartTableToLoad();
            var text = driver.FindElement(By.ClassName(Classes.BarchartIndicatorTitle)).Text;
            TestHelper.AssertTextContains(text, "Pupil absence");
        }

        private void LoadUrl(IWebDriver webDriver)
        {
            //Go to a specific indicator, area, sex, age etc... 
            webDriver.Navigate().GoToUrl(AppConfig.Instance.BridgeWsUrl +
                "profile/cyphof/data#gid/8000025/pat/6/ati/102/page/3/nn//par/E12000004/are/E06000015/iid/10301/age/193/sex/4");
        }
    }
}
