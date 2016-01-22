using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DomainObjects;

namespace MainUISeleniumTest.Tobacco
{
    [TestClass]
    public class TobaccoDataTest : TobaccoBaseUnitTest
    {
        public const string UrlKey = ProfileUrlKeys.Tobacco;

        [TestMethod]
        public void TestAllTabsLoad()
        {
            navigateTo.FingertipsDataForProfile(UrlKey);
            FingertipsHelper.SelectEachFingertipsTabInTurnAndCheckDownloadIsLast(driver);
        }
    }
}
