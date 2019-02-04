using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.PublicHealthDashboard
{
    [TestClass]
    public class PublicHealthDashboardTest : BaseUnitTest
    {
        [TestMethod]
        public void Test_Public_Health_Dashboard_Landing_Page()
        {
            navigateTo.HomePage();

            // Check home link is present
            TestHelper.AssertElementTextIsEqual("Home",
                driver.FindElement(By.XPath(XPaths.NavHome)));
        }

        [TestMethod]
        public void TestPublicHealthDashboardHomeNavigationHeaders()
        {
            navigateTo.PublicHealthDashboardHome();
            PublicHealthDashboardHelper.CheckNavigationHeaders(driver);
        }
    }
}
