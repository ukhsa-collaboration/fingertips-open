using System;
using System.Collections.Generic;
using System.Linq;
using IndicatorsUI.MainUISeleniumTest.HealthierLives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest
{
    [TestClass]
    public class PageHypertensionPracticeDetailsTest : BaseUnitTest
    {
        [TestMethod]
        public void TestPageLoadsOk()
        {
            var driver = LoadPage();

            PageDiabetesPracticeDetailsTest.CheckPracticeNameAndCcgName(driver);

            // Info boxes
            TestHelper.AssertTextContains(driver.FindElement(By.Id(LongerLivesIds.InfoBox1)).Text, "Practice list size");
            TestHelper.AssertTextContains(driver.FindElement(By.Id(LongerLivesIds.InfoBox2)).Text, "hypertension");
        }

        private IWebDriver LoadPage()
        {
            navigateTo.HypertensionPracticeDetails(
                PageDiabetesPracticeDetailsTest.GetPracticeDetailsHashParametersString());
            new WaitFor(driver).PracticeDetailsToLoad();
            return driver;
        }
    }
}
