using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest
{
    [TestClass]
    public class JasmineTestRunner : BaseUnitTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            InitDriverObjects();
        }

        [TestMethod]
        public void TestFingertips()
        {
            CheckTestPage("fingertips");
        }

        [TestMethod]
        public void TestDiabetes()
        {
            CheckTestPage("diabetes");
        }

        [TestMethod]
        public void TestDiabetesRankings()
        {
            CheckTestPage("diabetes-rankings");
        }

        private void CheckTestPage(string testPage)
        {
            navigateTo.JavaScriptTestPage(testPage);
            var by = By.ClassName("bar");
            waitFor.ExpectedElementToBeVisible(by);

            var element = driver.FindElement(by);
            var html = element.Text;
            TestHelper.AssertTextContains(html, ", 0 failures", "Some Jasmine tests have failed: " + html);
        }
    }
}
