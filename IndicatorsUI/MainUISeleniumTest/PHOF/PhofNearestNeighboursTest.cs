using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace MainUISeleniumTest.Phof
{
    [TestClass]
    public class PhofNearestNeighboursTest : PhofBaseUnitTest
    {
        const int ExtraColumns = 3;
        /// <summary>
        ///     Checks if NN link loaded correctly
        /// </summary>
        [TestMethod]
        public void CheckNearestNeighboursLink()
        {
            LoadPhofPage();

            var selectArea = FingertipsHelper.GetSelectedAreaNameFromMenu(driver);
            var selectedNearestNeighbourArea = driver.FindElement(By.Id("nearest-neighbour-link")).Text;

            Assert.AreEqual("CIPFA nearest neighbours to " + selectArea, selectedNearestNeighbourArea);
        }

        /// <summary>
        ///     Check if NN menu loaded correctly after NN link click
        /// </summary>
        [TestMethod]
        public void CheckNearestNeighboursMenuLoadedCorrectly()
        {
            LoadPhofPage();
            var selectedAreaName = FingertipsHelper.GetSelectedAreaNameFromMenu(driver);
            ClickNearestNeighbourLink();

            new WaitFor(driver).FingertipsNearestNeighboursMenuToLoad();

            // Check the back button text
            var backToMenuText = driver.FindElement(By.Id("goBack")).Text;
            TestHelper.AssertTextContains(backToMenuText, "Back to menus");
            TestHelper.AssertTextContains(backToMenuText, "More information");

            // Check NN text loaded correctly
            var selectAreaText = driver.FindElement(By.Id("selected-nearest-neighbour")).Text;
            Assert.IsTrue(selectAreaText.StartsWith(selectedAreaName + " and its CIPFA nearest neighbours"));
        }

        /// <summary>
        ///     Right hand table should only have 16 columns
        /// </summary>
        [TestMethod]
        public void TestNearestNeighboursForAnAreaLoadedCorrectly()
        {
            LoadPhofPage();
            ClickNearestNeighbourLink();
            new WaitFor(driver).AjaxLockToBeUnlocked();

            var rightTartanTable = driver.FindElement(By.XPath("//*[@id='rightTartanTable']"));
            var columns = rightTartanTable.FindElements(By.XPath("//*[@id='rightTartanTable']/thead/tr/th"));
           
            Assert.AreEqual(16, columns.Count - ExtraColumns);
        }

        /// <summary>
        ///     Test the back to menu link
        /// </summary>
        [TestMethod]
        public void TestBackToMenuLink()
        {
            LoadPhofPage();
            ClickNearestNeighbourLink();
            new WaitFor(driver).AjaxLockToBeUnlocked();
            ClickBackToMenuLink();
            new WaitFor(driver).AjaxLockToBeUnlocked();

            var rightTartanTable = driver.FindElement(By.XPath("//*[@id='rightTartanTable']"));
            var columns = rightTartanTable.FindElements(By.XPath("//*[@id='rightTartanTable']/thead/tr/th"));
            
            Assert.AreEqual(9, columns.Count - ExtraColumns);     
        }

        private void LoadPhofPage()
        {
            navigateTo.PhofDataPage();
        }

        private void ClickNearestNeighbourLink()
        {
            driver.FindElement(By.Id("nearest-neighbour-link")).Click();
        }

        private void ClickBackToMenuLink()
        {
            driver.FindElement(By.Id("goBack")).Click();
        }
    }
}