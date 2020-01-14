using IndicatorsUI.DomainObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsStateManagementTest : FingertipsBaseUnitTest
    {
        public const string UrlKey = ProfileUrlKeys.Phof;

        [TestMethod]
        public void Test_Last_Selected_Area_Is_Retained_Between_Page_Views()
        {
            navigateTo.PhofOverviewTab();
            var areaName = "Middlesbrough";

            fingertipsHelper.SwitchToAreaSearchMode();
            fingertipsHelper.SearchForAnAreaAndSelectFirstResult(areaName);
            fingertipsHelper.LeaveAreaSearchMode();

            // Leave and return to data page
            navigateTo.HomePage();
            navigateTo.PhofOverviewTab();

            // Check area menu contains searched for area
            Assert.AreEqual(areaName, fingertipsHelper.GetSelectedAreaNameFromMenu());
        }



    }
}
