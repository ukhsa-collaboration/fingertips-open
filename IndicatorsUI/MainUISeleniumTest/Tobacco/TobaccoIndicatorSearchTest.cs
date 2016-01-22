using MainUISeleniumTest.Fingertips;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace MainUISeleniumTest.Tobacco
{
    [TestClass]
    public class TobaccoIndicatorSearchTest : TobaccoBaseUnitTest
    {
        [TestMethod]
        public void CheckSearchFindsIndicators()
        {
            FingertipsIndicatorSearchTest.CheckSearchFindsSomeIndicators(driver, "smoking");
        }

        [TestMethod]
        public void CheckSearchIndicatorsPerProfilesPopUpDisplays()
        {
            FingertipsIndicatorSearchTest
                .CheckSearchIndicatorsPerProfilesPopUpDisplays(driver, "smoking");
        }

        [TestMethod]
        public void CheckOnlyTobaccoIndicatorsAreSearched()
        {
            var searchTerm = "cocaine";
            try
            {
                FingertipsIndicatorSearchTest.CheckSearchFindsNoMatchingIndicators(driver, searchTerm);
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail(searchTerm +
                    "' may appear in the metadata of an indicator in the tobacco profile. If it does then you should choose another search term that does not.");
            }
        }
    }
}
