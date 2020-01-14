using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabOverviewTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Overview_Initial_State()
        {
            navigateTo.PhofOverviewTab();

            // Check display buttons
            CheckDisplayOptionText(0, "Values");
            CheckDisplayOptionText(1, "Trends");
            CheckDisplayOptionText(2, "Values & Trends");
        }

        [TestMethod]
        public void Test_All_Domains_Load_For_Phof()
        {
            navigateTo.PhofOverviewTab();
            CheckOverviewLoadsForAllDomains();
        }

        [TestMethod]
        public void Test_All_Domains_Load_For_Health_Profiles()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.HealthProfiles);
            CheckOverviewLoadsForAllDomains();
        }

        [TestMethod]
        public void Test_All_Domains_Load_For_Sexual_Health()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.SexualHealth);
            CheckOverviewLoadsForAllDomains();
        }

        [TestMethod]
        public void Test_All_Domains_Load_For_Practice_Profiles()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.PracticeProfiles);
            CheckOverviewLoadsForAllDomains();
        }

        [TestMethod]
        public void Clicking_Recent_Trends_Icon_Navigates_To_Trends_Tab()
        {
            navigateTo.PhofOverviewTab();
            waitFor.FingertipsOverviewTabToLoad();
            fingertipsHelper.SelectTrendsOnOverviewTab();
            waitFor.FingertipsOverviewTabToLoad();

            // Click on first column of tartan rug            
            driver.FindElement(By.Id("tc-1-0")).Click();

            // Success if trends table loads
            waitFor.FingertipsTrendsTableToLoad();
        }

        private void CheckDisplayOptionText(int i, string text)
        {
            var element = driver.FindElement(By.Id("tab-option-" + i));

            Assert.AreEqual(text, element.Text);
            Assert.IsTrue(element.Displayed);
        }

        public void CheckOverviewLoadsForAllDomains()
        {
            var previousIndicatorNames = string.Empty;

            // Click through each domain
            var domains = fingertipsHelper.GetDomainOptions();
            foreach (var domain in domains)
            {
                // Select domain
                domain.Click();
                waitFor.AjaxLockToBeUnlocked();

                // Check indicators are different on new domain
                var indicatorNames = GetIndicatorNames();
                Assert.AreNotEqual(previousIndicatorNames, indicatorNames);

                previousIndicatorNames = indicatorNames;
            }
        }

        private string GetIndicatorNames()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                sb.Append(GetIndicatorName(i));
            }

            var indicatorNames = sb.ToString();
            return indicatorNames;
        }

        private string GetIndicatorName(int tartanRugRowNumber)
        {
            var idPrefix = "rug-indicator-";

            var element = driver.FindElements(By.Id(idPrefix + tartanRugRowNumber)).FirstOrDefault();

            if (element == null)
            {
                return string.Empty;
            }

            return element.Text;
        }

    }
}