using System.Linq;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabDefinitionsTest : FingertipsBaseUnitTest
    {
        private const int AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPre2019;
        private const string ButtonId = "indicator-not-in-area-ok";
        private const string ExpectedRowName = "Rationale";

        private const string RationaleCell = "//*[@id='metadata-table']/tr[5]/td[{0}]";
        private static readonly string RationaleXPathHeading = string.Format(RationaleCell,1);
        private static readonly string RationaleXPathContentText = string.Format(RationaleCell, 2);

        private string _indicatorName;

        [TestInitialize]
        public void StartUp()
        {
            navigateTo.FingertipsDataForProfile(ProfileUrlKeys.DevelopmentProfileForTesting);
            fingertipsHelper.SelectDefinitionsTab();
        }

        [TestMethod]
        public void Test_Metadata_Displayed_When_Data_Available()
        {
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

        [TestMethod]
        public void Should_Display_Only_Rationale()
        {
            _indicatorName = "(Indicator) Indicator with rationale and No specific rationale";
            const string expectedContentText = "(Rationale) Indicator with rationale and No specific rationale";

            fingertipsHelper.SelectAreaType(AreaTypeId);

            ClickOkButton();
            fingertipsHelper.SelectIndicatorByName(_indicatorName);

            var resultRowTitleNameText = GetElementTextByXPath(RationaleXPathHeading);
            var resultContentText = GetElementTextByXPath(RationaleXPathContentText);

            Assert.AreEqual(ExpectedRowName, resultRowTitleNameText);
            Assert.AreEqual(expectedContentText, resultContentText);
        }

        [TestMethod]
        public void Should_Display_Rationale_And_Specific_Rationale()
        {
            _indicatorName = "(Indicator) Indicator with rationale and specific rationale";
            const string expectedContentText = "(Rationale) Indicator with rationale and specific rationale\r\n(Specific rationale) Indicator with rationale and specific rationale";

            fingertipsHelper.SelectAreaType(AreaTypeId);

            ClickOkButton();
            fingertipsHelper.SelectIndicatorByName(_indicatorName);

            var resultRowTitleNameText = GetElementTextByXPath(RationaleXPathHeading);
            var resultContentText = GetElementTextByXPath(RationaleXPathContentText);

            Assert.AreEqual(ExpectedRowName, resultRowTitleNameText);
            Assert.AreEqual(expectedContentText, resultContentText);
        }

        [TestMethod]
        public void Should_Display_Only_Specific_Rationale()
        {
            _indicatorName = "(Indicator) Indicator with no rationale and specific rationale";
            const string expectedContentText = "(Specific rationale) Indicator with no rationale and specific rationale";

            fingertipsHelper.SelectAreaType(AreaTypeId);

            ClickOkButton();
            fingertipsHelper.SelectIndicatorByName(_indicatorName);

            var resultRowTitleNameText = GetElementTextByXPath(RationaleXPathHeading);
            var resultContentText = GetElementTextByXPath(RationaleXPathContentText);

            Assert.AreEqual(ExpectedRowName, resultRowTitleNameText);
            Assert.AreEqual(expectedContentText, resultContentText);
        }

        private void TestPropertyDisplayed(string html, string property)
        {
            TestHelper.AssertTextContains(html, property, property + " should be displayed");
        }

        private void ClickOkButton()
        {
            waitFor.ExpectedElementToBeVisible(By.Id(ButtonId));
            fingertipsHelper.ClickElementById(ButtonId);
        }

        private string GetElementTextByXPath(string xPath)
        {
            var elements = fingertipsHelper.FindElementsByXPath(xPath);
            return elements.FirstOrDefault().Text;
        }
    }
}