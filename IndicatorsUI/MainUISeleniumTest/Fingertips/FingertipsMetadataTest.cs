using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsMetadataTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Metadata_Displayed_When_Data_Available()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.HealthProfiles);
            SelectMetadataTab();
            waitFor.FingertipsMetadataTableToLoad();

            // Assert expected properties are displayed
            var html = driver.FindElement(By.ClassName("definition-table")).Text;
            TestPropertyDisplayed(html, "Indicator ID");
            TestPropertyDisplayed(html, "Definition");
            TestPropertyDisplayed(html, "Data source");
            TestPropertyDisplayed(html, "Value type");
            TestPropertyDisplayed(html, "Age");
            TestPropertyDisplayed(html, "Sex");
            TestPropertyDisplayed(html, "Year type");
            TestPropertyDisplayed(html, "Benchmarking method");
        }

        private void TestPropertyDisplayed(string html, string property)
        {
            TestHelper.AssertTextContains(html, property, property + " should be displayed");
        }

        private void SelectMetadataTab()
        {
            FingertipsHelper.SelectFingertipsTab(driver, "page-metadata");
        }
    }
}
