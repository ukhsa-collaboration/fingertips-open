using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace MainUISeleniumTest
{
    [TestClass]
    public class JasmineTestRunner : BaseUnitTest
    {
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

        [TestMethod]
        public void TestPracticeProfiles()
        {
            CheckTestPage("practice-profiles");
        }

        private void CheckTestPage(string testPage)
        {
            navigateTo.JavaScriptTestPage(testPage);
            var by = By.ClassName("bar");
            new WaitFor(driver).ExpectedElementToBeVisible(by);

            var element = driver.FindElement(by);
            var html = element.Text;
            TestHelper.AssertTextContains(html, ", 0 failures", "Some Jasmine tests have failed: " + html);
        }
    }
}
