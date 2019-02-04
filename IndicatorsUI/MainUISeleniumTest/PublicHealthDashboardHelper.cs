using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace IndicatorsUI.MainUISeleniumTest
{
    public class PublicHealthDashboardHelper
    {

        public static void CheckNavigationHeaders(IWebDriver driver)
        {
            TestHelper.AssertElementTextIsEqual("Home",
                driver.FindElement(By.XPath(XPaths.NavHome)));
            TestHelper.AssertElementTextIsEqual("Map",
                driver.FindElement(By.XPath(XPaths.NavMap)));
            TestHelper.AssertElementTextIsEqual("National comparisons",
                driver.FindElement(By.XPath(XPaths.NavNationalComparisons)));
            TestHelper.AssertElementTextIsEqual("About the data",
                driver.FindElement(By.XPath(XPaths.NavAboutTheData)));
        }

        public static void ClickOnMapAndWaitForPopUp(IWebDriver webDriver, int x, int y)
        {
            var builder = new Actions(webDriver);
            builder.MoveToElement(webDriver.FindElement(By.Id(PublicHealthDashboardIds.Map)), x, y)
                .Click()
                .Build()
                .Perform();

            var popUp = By.ClassName(Classes.MapPopUp);
            new WaitFor(webDriver).ExpectedElementToBePresent(popUp);
        }
    }
}