using IndicatorsUI.MainUISeleniumTest.Fingertips;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUI.MainUISeleniumTest.Phof
{
    [TestClass]
    public class PhofIndicatorSearchTest : PhofBaseUnitTest
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
        public void CheckOnlyPhofIndicatorsAreSearched()
        {
            FingertipsIndicatorSearchTest.CheckSearchFindsNoMatchingIndicators(driver, "underweight");
        }

        [TestMethod]
        public void TestAllTabsLoadForSearchResults()
        {
            navigateTo.FingertipsIndicatorSearchResults("alcohol");
            waitFor.FingertipsTartanRugToLoad();

            FingertipsHelper.SelectEachFingertipsTabInTurnAndCheckDownloadIsLast(driver);
        }
    }
}
