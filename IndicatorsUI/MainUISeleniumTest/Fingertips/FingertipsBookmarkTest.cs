using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;

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
            parameters.AddAreaTypeId(AreaTypeIds.CountyAndUnitaryAuthorityPre2019);
            navigateTo.GoToUrl(ProfileUrlKeys.Phof + parameters.HashParameterString);
            waitFor.FingertipsOverviewTabToLoad();

            // Check area menu contains searched for area
            Assert.AreEqual("Hartlepool", fingertipsHelper.GetSelectedAreaNameFromMenu());
        }

        [TestMethod]
        public void Test_Indicator_And_Sex_And_Age_Can_Be_Bookmarked()
        {
            var parameters = new HashParameters();
            parameters.AddAreaTypeId(AreaTypeIds.DistrictAndUAPreApr2019);
            parameters.AddIndicatorId(IndicatorIds.BackPainPrevalence);
            parameters.AddSexId(SexIds.Persons);
            parameters.AddAgeId(AgeIds.AllAges);
            parameters.AddTabId(TabIds.CompareAreas);

            navigateTo.GoToUrl(ProfileUrlKeys.DevelopmentProfileForTesting + parameters.HashParameterString);
            waitFor.FingertipsBarChartTableToLoad();

            // Check area menu contains searched for area
            var text = driver.FindElement(By.Id("indicator-header-link")).Text;
            TestHelper.AssertTextContains(text, "Back pain prevalence");

        }
    }
}
