using System.Threading;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsEnglandParentAreaTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Parent_Menu_Hidden_If_England_Parent_Type()
        {
            // Navigate to the developer profile for testing overview tab
            navigateTo.OverviewTab();
            waitFor.FingertipsOverviewTabToLoad();

            // Select region area type
            SelectElement areaTypeDropdown = new SelectElement(driver.FindElement(By.Id("areaTypes")));
            areaTypeDropdown.SelectByValue("6");

            // Wait for the page to refresh
            Thread.Sleep(500);
            waitFor.AjaxLockToBeUnlocked();

            // Areas grouped by drop down selected value must be England
            SelectElement areasGroupedByDropdown = new SelectElement(driver.FindElement(By.Id("parentAreaTypesMenu")));
            TestHelper.AssertElementTextIsEqual("England", areasGroupedByDropdown.SelectedOption);

            // Area type drop down must not be visible
            Assert.IsTrue(!driver.FindElement(By.Id("regionMenu")).Displayed);
        }
    }
}
