using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.HealthierLives
{
    [TestClass]
    public class PageDiabetesPracticeDetailsTest : BaseUnitTest
    {
        [TestMethod]
        public void TestPageLoadsOk()
        {
            var driver = LoadPage();

            CheckPracticeNameAndCcgName(driver);

            // Info boxes
            TestHelper.AssertTextContains(driver.FindElement(By.Id(LongerLivesIds.InfoBox1)).Text, "Practice list size");
            TestHelper.AssertTextContains(driver.FindElement(By.Id(LongerLivesIds.InfoBox2)).Text, "diabetes");

            // Deprivation label
            TestHelper.AssertTextDoesNotContain(driver.FindElement(By.Id("diabetes-rankings-table")).Text,
                "undefined", "Deprivation label is not displayed correctly");
        }

        public static void CheckPracticeNameAndCcgName(IWebDriver driver)
        {
            // Check area name
            Assert.AreEqual("Thatched House Medical Centre",
                driver.FindElement(By.ClassName("area_name")).Text,
                "Practice name not as expected");

            // Check parent name
            Assert.AreEqual(AreaNames.CcgWalthamForest,
                driver.FindElement(By.ClassName("practice_in_area")).Text,
                "Parent name not as expected");
        }

        private IWebDriver LoadPage()
        {
            navigateTo.DiabetesPracticeDetails(GetPracticeDetailsHashParametersString());
            new WaitFor(driver).PracticeDetailsToLoad();
            return driver;
        }

        public static string GetPracticeDetailsHashParametersString()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.GpPracticeThatchedHouseMedicalCentre);
            parameters.AddParentAreaCode(AreaCodes.CcgWalthamForest);
            parameters.AddParentAreaTypeId(AreaTypeIds.CcgPreApr2017);
            parameters.AddAreaTypeId(AreaTypeIds.CcgPreApr2017);
            return parameters.HashParameterString;
        }
    }
}