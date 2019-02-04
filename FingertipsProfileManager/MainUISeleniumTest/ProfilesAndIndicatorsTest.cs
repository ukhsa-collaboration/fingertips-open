using System;
using System.Linq;
using System.Threading;
using Fpm.ProfileData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Fpm.MainUISeleniumTest
{
    [TestClass]
    public class ProfilesAndIndicatorsTest : BaseUnitTest
    {
        private IWebDriver _driver;
        private NavigateTo _navigateTo;
        private WaitFor _waitFor;

        [TestInitialize]
        public void TestInitialize()
        {
            _driver = Driver;
            _navigateTo = new NavigateTo(_driver);
            _waitFor = new WaitFor(_driver);
        }

        [TestMethod]
        public void Test_ProfilesAndIndicatorsPageLoads()
        {
            _navigateTo.ProfilesAndIndicatorsPage();
            _waitFor.PageWithModalPopUpToLoad();

            // Is one table of indicators
            var indicatorTable = _driver.FindElements(By.ClassName("grid"));
            Assert.AreEqual(1, indicatorTable.Count);
        }

        [TestMethod]
        public void Test_CopyIndicators()
        {
            _navigateTo.ProfilesAndIndicatorsPage();
            _waitFor.PageWithModalPopUpToLoad();

            // Select profile
            var profileSelect = _driver.FindElement(By.Id("selectedProfile"));
            var selectElement = new SelectElement(profileSelect);
            selectElement.SelectByValue(UrlKeys.Hypertension);

            // Tick an indicator to copy
            var tickBox = _driver.FindElements(By.Name(IndicatorIds.HypertensionPrevalence + "_selected"));
            tickBox.First().Click();

            // Click copy
            var copyButton = _driver.FindElement(By.Id("copy-indicators-button"));
            copyButton.Click();
            SeleniumHelper.WaitForExpectedElementToBeVisible(_driver, By.Id("copyIndicators"));

            // Check the correct profile is selected in the profile menu
            var copyProfileSelect = _driver.FindElement(By.Id("TargetProfileUrlKey"));
            var copySelectElement = new SelectElement(copyProfileSelect);
            var selectedProfileName = copySelectElement.SelectedOption.Text;
            Assert.AreEqual("Hypertension", selectedProfileName, "Incorrect profile selected");
        }

        [TestMethod]
        public void Test_ReorderIndicators()
        {
            _navigateTo.ProfilesAndIndicatorsPage();
            _waitFor.PageWithModalPopUpToLoad();

            // Select reorder indicators
            _driver.FindElement(By.Id("reorder-indicators")).Click();
            SeleniumHelper.WaitForExpectedElementToBeClickable(_driver, By.Id("reorder-add-subheading"));

            // Open add subheading pop up
            _driver.FindElement(By.Id("reorder-add-subheading")).Click();
            SeleniumHelper.WaitForExpectedElementToBeVisible(_driver, By.ClassName("info-box"));

            // Add subheading
            var guid = Guid.NewGuid().ToString();
            _driver.FindElement(By.Id("txt-light-box")).SendKeys(guid);
            _driver.FindElement(By.ClassName("ok")).Click();
            SeleniumHelper.WaitForExpectedElementToBeVisible(_driver,
                By.XPath("//*[@id='table']/tbody"));

            // Check subheading was not created
            TestHelper.AssertTextContains(_driver.FindElement(By.XPath("//*[@id='table']/tbody")).Text, guid,
                "Subheading " + guid + " not created");
        }
    }
}
