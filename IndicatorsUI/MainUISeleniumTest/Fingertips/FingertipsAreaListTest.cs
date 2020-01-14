using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsAreaListTest : FingertipsBaseUnitTest
    {
        private AreaListHelper _areaListHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            _areaListHelper = new AreaListHelper(driver);
            _areaListHelper.NavigateToSignInAreaListPage();
            _areaListHelper.SignInAreaListPage();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _areaListHelper.SignOutAreaListPage();
        }

        [TestMethod]
        public void Test_1_Can_Create_New_Area_List()
        {
            _areaListHelper.NavigateToCreateNewAreaListPage();
            _areaListHelper.CreateNewAreaList();
        }

        [TestMethod]
        public void Test_2_Can_Edit_Area_List()
        {
            _areaListHelper.NavigateToEditAreaListPage();
            _areaListHelper.EditAreaList();
        }

        [TestMethod]
        public void Test_3_Can_Copy_Area_List()
        {
            _areaListHelper.NavigateToAreaListPage();
            _areaListHelper.CopyAreaList();
        }

        [TestMethod]
        public void Test_4_Can_Delete_Area_Lists()
        {
            _areaListHelper.NavigateToAreaListPage();
            _areaListHelper.DeleteAreaLists();
        }
    }
}
