using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace IndicatorsUI.MainUISeleniumTest
{
    public class LongerLivesHelper
    {

        public static void CheckNavigationHeaders(IWebDriver driver)
        {
            TestHelper.AssertElementTextIsEqual("Home",
                driver.FindElement(By.XPath(XPaths.NavHome)));
            TestHelper.AssertElementTextIsEqual("National comparisons",
                driver.FindElement(By.XPath(XPaths.NavNationalComparisons)));
            TestHelper.AssertElementTextIsEqual("About the project",
                driver.FindElement(By.XPath(XPaths.NavAboutTheProject)));
            TestHelper.AssertElementTextIsEqual("About the data",
                driver.FindElement(By.XPath(XPaths.NavAboutTheData)));
        }

        public static void ClickOnMapAndWaitForPopUp(IWebDriver webDriver, int x, int y)
        {
            var builder = new Actions(webDriver);
            builder.MoveToElement(webDriver.FindElement(By.Id(LongerLivesIds.Map)), x, y)
                .Click()
                .Build()
                .Perform();

            var popUp = By.ClassName(Classes.MapPopUp);
            new WaitFor(webDriver).ExpectedElementToBePresent(popUp);
        }

        public static void SearchForAreaAndChooseFirstResult(IWebDriver driver, string text)
        {
            var waitFor = new WaitFor(driver);
            waitFor.PageToFinishLoading();

            var searchBox = driver.FindElement(By.Id("search_text"));
            searchBox.Click();
            searchBox.SendKeys(text);
            searchBox.Click();

            waitFor.AutoCompleteSearchResultsToBeDisplayed();

            searchBox.SendKeys(Keys.Return);
        }


    }
}