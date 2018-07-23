using IndicatorsUI.MainUISeleniumTest.Fingertips;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.Phof
{
    [TestClass]
    public class PhofNearestNeighboursTest : FingertipsBaseUnitTest
    {
        const int ExtraColumns = 3;
        /// <summary>
        ///     Checks if NN link loaded correctly
        /// </summary>
        [TestMethod]
        public void CheckNearestNeighboursLink()
        {
            navigateTo.PhofTartanRug();

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
            navigateTo.PhofTartanRug();
            var selectedAreaName = FingertipsHelper.GetSelectedAreaNameFromMenu(driver);
            ClickNearestNeighbourLink();

            new WaitFor(driver).FingertipsNearestNeighboursMenuToLoad();

            // Check the back button text
            var exitNearestNeighboursText = driver.FindElement(By.Id("nearest-neighbour-links")).Text;
            TestHelper.AssertTextContains(exitNearestNeighboursText, "Exit nearest neighbours");
            TestHelper.AssertTextContains(exitNearestNeighboursText, "More information");

            // Check NN text loaded correctly
            var selectAreaText = driver.FindElement(By.Id("nearest-neighbour-header")).Text;
            Assert.IsTrue(selectAreaText.StartsWith(selectedAreaName + " and its CIPFA nearest neighbours"));
        }

        /// <summary>
        ///     Right hand table should only have 16 columns
        /// </summary>
        [TestMethod]
        public void TestNearestNeighboursForAnAreaLoadedCorrectly()
        {
            navigateTo.PhofTartanRug();
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
        public void TestExitNearestNeighboursLink()
        {
            navigateTo.PhofTartanRug();
            ClickNearestNeighbourLink();
            new WaitFor(driver).AjaxLockToBeUnlocked();
            ClickExitNearestNeighboursLink();
            new WaitFor(driver).AjaxLockToBeUnlocked();

            var columns = driver.FindElements(By.XPath("//*[@id='rightTartanTable']/thead/tr/th"));
            
            // Assert: nearest neighbours is no longer displayed
            Assert.AreEqual(9, columns.Count - ExtraColumns);     
        }

        private void ClickNearestNeighbourLink()
        {
            driver.FindElement(By.Id("nearest-neighbour-link")).Click();
        }

        private void ClickExitNearestNeighboursLink()
        {
            driver.FindElement(By.Id("exit-nearest-neighbours")).Click();
        }
    }
}