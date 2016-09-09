using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Profiles.MainUI.Skins;
using System.Collections.Generic;
using System.Linq;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsBaseUnitTest : BaseUnitTest
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            SetSkin(SkinNames.Core);
        }

        public static void CheckTartanRugHasLoaded(IWebDriver driver)
        {
            IList<IWebElement> firstRow = driver.FindElements(By.Id(LongerLivesIds.TartanRugIndicatorNameOnFirstRow));
            Assert.IsTrue(firstRow.Any());
        }

        public static void CheckProfilesPerIndicatorLinkExists(IWebDriver driver)
        {
            var profilePerIndicator = driver.FindElement(By.Id(FingertipsIds.ProfilePerIndicator));
            Assert.IsTrue(profilePerIndicator != null);
        }

        public static void CheckProfilesPerIndicatorPopup(IWebDriver driver)
        {
            // Click show me which profiles these indicators are in
            var profilePerIndicator = driver.FindElement(By.Id(FingertipsIds.ProfilePerIndicator));
            profilePerIndicator.Click();

            // Wait for indicator menu to be visible in pop up
            var byIndicatorMenu = By.Id(FingertipsIds.ListOfIndicators);
            new WaitFor(driver).ExpectedElementToBeVisible(byIndicatorMenu);

            // Select 2nd indicator in list
            var listOfIndicators = driver.FindElement(byIndicatorMenu);
            var selectMenu = new SelectElement(listOfIndicators);
            selectMenu.SelectByIndex(1);

            // Check list of profiles is displayed
            var listOfProfiles = driver.FindElements(By.XPath(XPaths.ListOfProfilesInPopup));
            Assert.IsTrue(listOfProfiles.Count > 0);            
        }
    }
}
