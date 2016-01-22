using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace MainUISeleniumTest.HealthierLives
{
    [TestClass]
    public class PageMortalityHomeTest : BaseUnitTest
    {
        [TestMethod]
        public void NavigationHeaderLinksAsExpected()
        {
            GoToMortalityHomeAndWaitToLoad();

            TestHelper.AssertElementTextIsEqual("Home",
                driver.FindElement(By.XPath(XPaths.NavHome)));
            TestHelper.AssertElementTextIsEqual("Mortality rankings",
                driver.FindElement(By.XPath(XPaths.NavNationalComparisons)));
            TestHelper.AssertElementTextIsEqual("About the project",
                driver.FindElement(By.XPath(XPaths.NavAboutTheProject)));
            TestHelper.AssertElementTextIsEqual("About the data",
                driver.FindElement(By.XPath(XPaths.NavAboutTheData)));
            TestHelper.AssertElementTextIsEqual("Connect",
                driver.FindElement(By.XPath(XPaths.NavConnect)));

            // Check home navigation is selected
            string cssClass = driver.FindElement(By.Id("home-nav")).GetAttribute("class");
            Assert.AreEqual("selected", cssClass);
        }

        [TestMethod]
        public void CheckMapPopUpContainsExpectedInformation()
        {
            GoToMortalityHomeAndWaitToLoad();
            ClickOnNorthumberland();

            Assert.AreEqual("Northumberland", GetAreaName());
            Assert.IsNotNull(GetPopulation());
            Assert.IsNotNull(GetTotalPrematureDeaths());
        }

        [TestMethod]
        public void CheckMapPopUpContainsExpectedLink()
        {
            GoToMortalityHomeAndWaitToLoad();
            ClickOnNorthumberland();

            // Click on view local authority details link
            driver.FindElement(By.ClassName("map-info-footer")).FindElement(By.TagName("a")).Click();

            waitFor.MortalityAreaDetailsToLoad();
        }

        [TestMethod]
        public void NavigatingToAreaDetailsUsingSearch()
        {
            GoToMortalityHomeAndWaitToLoad();

            LongerLivesHelper.SearchForAreaAndChooseFirstResult(driver, "durham");

            waitFor.MortalityAreaDetailsToLoad();
        }

        [TestMethod]
        public void ClickEachIndicatorInTurnAndCheckPopUpTextChanges()
        {
            GoToMortalityHomeAndWaitToLoad();
            ClickOnNorthumberland();

            // Overall mortality
            var overallPrematureDeathInfoBox = GetPopUpText();

            // Cancer
            driver.FindElement(By.Id("Overall-Cancer")).Click();
            WaitFor.ThreadWait(1);
            var cancerInfoBox = GetPopUpText();
            Assert.IsFalse(cancerInfoBox.Equals(overallPrematureDeathInfoBox));

            // CVD
            driver.FindElement(By.Id("Overall-CardioVascular")).Click();
            WaitFor.ThreadWait(1);
            var heartDiseaseAndStrokeInfoBox = GetPopUpText();
            Assert.IsFalse(heartDiseaseAndStrokeInfoBox.Equals(cancerInfoBox));

            // Lung
            driver.FindElement(By.Id("Overall-Lung")).Click();
            WaitFor.ThreadWait(1);
            var lungDiseaseInfoBox = GetPopUpText();
            Assert.IsFalse(lungDiseaseInfoBox.Equals(heartDiseaseAndStrokeInfoBox));

            // Liver
            driver.FindElement(By.Id("Overall-Liver")).Click();
            WaitFor.ThreadWait(1);
            var liverDiseaseInfoBox = GetPopUpText();
            Assert.IsFalse(liverDiseaseInfoBox.Equals(lungDiseaseInfoBox));

            // Injury
            driver.FindElement(By.Id("Overall-Injury")).Click();
            WaitFor.ThreadWait(1);
            var injuryInfoBox = GetPopUpText();
            Assert.IsFalse(injuryInfoBox.Equals(liverDiseaseInfoBox));

            // Lung Cancer
            driver.FindElement(By.Id("Overall-Lung-Cancer")).Click();
            WaitFor.ThreadWait(1);
            var lungCancerInfoBox = GetPopUpText();
            Assert.IsFalse(lungCancerInfoBox.Equals(injuryInfoBox));

            // Colorectal Cancer
            driver.FindElement(By.Id("Overall-Colorectal-Cancer")).Click();
            WaitFor.ThreadWait(1);
            var colorectalCancerInfoBox = GetPopUpText();
            Assert.IsFalse(colorectalCancerInfoBox.Equals(lungCancerInfoBox));

            // Breast Cancer
            driver.FindElement(By.Id("Overall-Breast-Cancer")).Click();
            WaitFor.ThreadWait(1);
            var breastCancerInfoBox = GetPopUpText();
            Assert.IsFalse(breastCancerInfoBox.Equals(colorectalCancerInfoBox));

            // Heart Disease
            driver.FindElement(By.Id("Overall-Heart-Disease")).Click();
            WaitFor.ThreadWait(1);
            var heartDiseaseInfoBox = GetPopUpText();
            Assert.IsFalse(heartDiseaseInfoBox.Equals(breastCancerInfoBox));

            // Stroke
            driver.FindElement(By.Id("Overall-Stroke")).Click();
            WaitFor.ThreadWait(1);
            var strokeInfoBox = GetPopUpText();
            Assert.IsFalse(strokeInfoBox.Equals(heartDiseaseInfoBox));

            // Deprivation
            driver.FindElement(By.Id("Deprivation")).Click();
            WaitFor.ThreadWait(1);
            var deprivationInfoBox = GetPopUpText();
            Assert.IsFalse(deprivationInfoBox.Equals(liverDiseaseInfoBox));

        }

        private void GoToMortalityHomeAndWaitToLoad()
        {
            navigateTo.MortalityHome();
            waitFor.GoogleMapToLoad();
        }

        private void ClickOnNorthumberland()
        {
            LongerLivesHelper.ClickOnMapAndWaitForPopUp(driver, 570, 100);
        }

        private string GetPopUpText()
        {
            return driver.FindElement(By.ClassName(Classes.MapPopUp)).Text;
        }

        private string GetAreaName()
        {
            return driver.FindElement(By.ClassName("place-name")).Text;
        }

        private string GetPopulation()
        {
            return driver.FindElement(By.ClassName("population_stat")).FindElement(By.TagName("strong")).Text;
        }

        private string GetTotalPrematureDeaths()
        {
            return driver.FindElement(By.ClassName("premature_death_stat")).FindElement(By.TagName("strong")).Text;
        }
    }
}