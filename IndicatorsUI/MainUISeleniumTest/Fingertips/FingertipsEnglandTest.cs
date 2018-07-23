using System;
using System.Collections.Generic;
using System.Linq;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Fingertips;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsEnglandTest : FingertipsBaseUnitTest
    {

        [TestMethod]
        public void Test_England_Data_Displayed()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.HealthProfiles);
            SelectEnglandTab();
            waitFor.FingertipsEnglandTableToLoad();

            // Assert expected properties are displayed
            var rows = driver.FindElement(By.Id("england-table")).FindElements(By.TagName("tr"));
            Assert.IsTrue(rows.Any());
        }

        private void SelectEnglandTab()
        {
            FingertipsHelper.SelectFingertipsTab(driver, "page-england");
        }


    }
}
