using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Tobacco
{
    [TestClass]
    public class TobaccoDataTest : TobaccoBaseUnitTest
    {
        public const string UrlKey = ProfileUrlKeys.Tobacco;

        [TestMethod]
        public void TestTobaccoHomePage()
        {
            navigateTo.HomePage();
            waitFor.FingertipsProfileFrontPageToLoad();
        }

        [TestMethod]
        public void TestTobaccoAllTabsLoad()
        {
            navigateTo.FingertipsDataForProfile(UrlKey);
            FingertipsHelper.SelectEachFingertipsTabInTurnAndCheckDownloadIsLast(driver);
        }
    }
}
