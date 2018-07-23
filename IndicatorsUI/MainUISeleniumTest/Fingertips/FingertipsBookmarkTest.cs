using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsBookmarkTest : FingertipsBaseUnitTest
    {

        [TestMethod]
        public void Test_Area_Code_Can_Be_Bookmarked()
        {
            var parameters = new HashParameters();
            parameters.AddAreaCode(AreaCodes.Hartlepool);
            parameters.AddAreaTypeId(AreaTypeIds.CountyAndUnitaryAuthority);
            navigateTo.GoToUrl(ProfileUrlKeys.Phof + parameters.HashParameterString);
            waitFor.FingertipsTartanRugToLoad();

            // Check area menu contains searched for area
            Assert.AreEqual("Hartlepool", FingertipsHelper.GetSelectedAreaNameFromMenu(driver));
        }

        [TestMethod]
        public void Test_Indicator_And_Sex_And_Age_Can_Be_Bookmarked()
        {
            var parameters = new HashParameters();
            parameters.AddAreaTypeId(AreaTypeIds.CountyAndUnitaryAuthority);
            parameters.AddIndicatorId(IndicatorIds.GapInLifeExpectancyAtBirth);
            parameters.AddSexId(SexIds.Female);
            parameters.AddAgeId(AgeIds.AllAges);
            parameters.AddTabId(TabIds.CompareAreas);

            navigateTo.GoToUrl(ProfileUrlKeys.Phof + parameters.HashParameterString);
            waitFor.FingertipsBarChartTableToLoad();

            // Check area menu contains searched for area
            var text = driver.FindElement(By.Id("indicator-details-header")).Text;
            TestHelper.AssertTextContains(text, "gap in life expectancy at birth");
            TestHelper.AssertTextContains(text, "(Female)");
        }
    }
}
