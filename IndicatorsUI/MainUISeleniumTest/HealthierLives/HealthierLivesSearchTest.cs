using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.HealthierLives
{
    [TestClass]
    public class HealthierLivesSearchTest : BaseUnitTest
    {
        [TestMethod]
        public void TestCanNavigateToPracticeViaAreaSearchForCcgsInDiabetes()
        {
            navigateTo.DiabetesHome();
            CheckCanNavigateToSpecificPractice();
        }

        [TestMethod]
        public void TestCanNavigateToPracticeViaAreaSearchForCountiesUAsInDiabetes()
        {
            navigateTo.DiabetesHome();
            PageDiabetesHomeTest.SelectAreaTypeAndWaitForPageToLoad(driver, LongerLivesIds.AreaTypeLinkCcgs);
            CheckCanNavigateToSpecificPractice();
        }

        private void CheckCanNavigateToSpecificPractice()
        {
            LongerLivesHelper.SearchForAreaAndChooseFirstResult(driver, "bristol");

            waitFor.HealthierLivesSearchResultsToLoad();

            // Click on link to go to practice details
            driver.FindElement(By.XPath("//*[@id=\"L81023-address\"]/td[2]/div[2]/a")).Click();

            waitFor.PracticeDetailsToLoad();

            // Check address
            var address = driver.FindElement(By.Id("area-address")).Text;
            TestHelper.AssertTextContains(address, "eastville", "Practice address was not as expected");
        }
    }
}
