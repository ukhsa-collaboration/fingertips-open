using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Fpm.MainUISeleniumTest
{
    [TestClass]
    public class ReportTest : BaseUnitTest
    {
        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void Test_1_Report_Add()
        {
            var driver = Driver;
            NavigateToReportsIndexPage(driver);

            driver.FindElement(By.Id("newReportButton")).Click();
            new WaitFor(driver).ReportsEditPageToLoad();

            driver.FindElement(By.Id("reportName")).SendKeys("Test report");
            driver.FindElement(By.Id("fileName")).SendKeys("TestReportFile");
            driver.FindElement(By.Id("notes")).SendKeys("Test report notes");

            // Profiles dropdown
            driver.FindElement(By.Name("profiles")).Click();
            driver.FindElements(By.ClassName("select2-search__field"))[0].SendKeys("Unassigned Indicators");
            driver.FindElement(By.ClassName("select2-results__option--highlighted")).Click();
            driver.FindElement(By.Id("reportName")).Click();

            // Parameters dropdown
            driver.FindElement(By.Name("parameters")).Click();
            driver.FindElements(By.ClassName("select2-search__field"))[1].SendKeys("areaCode");
            driver.FindElement(By.ClassName("select2-results__option--highlighted")).Click();
            driver.FindElement(By.Id("reportName")).Click();

            // AreaTypes dropdown
            driver.FindElement(By.Name("areaTypes")).Click();
            driver.FindElements(By.ClassName("select2-search__field"))[2].SendKeys("Acute Trusts");
            driver.FindElement(By.ClassName("select2-results__option--highlighted")).Click();
            driver.FindElement(By.Id("reportName")).Click();

            driver.FindElement(By.Id("saveReportButton")).Click();
            new WaitFor(driver).ReportsIndexPageToLoad();

            var reportsTable = driver.FindElement(By.Id("reportListTable"));
            var text = reportsTable.Text;

            TestHelper.AssertTextContains(text, "Test report");
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void Test_2_Report_Edit()
        {
            var driver = Driver;
            NavigateToReportsIndexPage(driver);

            var reportLinks = driver.FindElements(By.ClassName("btn-link"));
            reportLinks.FirstOrDefault(x => x.Text == "Test report").Click();
            new WaitFor(driver).ReportsEditPageToLoad();

            driver.FindElement(By.Id("reportName")).SendKeys(" modified");

            driver.FindElement(By.Id("saveReportButton")).Click();
            new WaitFor(driver).ReportsIndexPageToLoad();

            var reportsTable = driver.FindElement(By.Id("reportListTable"));
            var text = reportsTable.Text;

            TestHelper.AssertTextContains(text, "Test report modified");
        }

        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void Test_3_Report_Delete()
        {
            var driver = Driver;
            NavigateToReportsIndexPage(driver);

            var reportLinks = driver.FindElements(By.ClassName("btn-link"));
            reportLinks.FirstOrDefault(x => x.Text == "Test report modified").Click();
            new WaitFor(driver).ReportsEditPageToLoad();

            driver.FindElement(By.Id("deleteReportButton")).Click();
            SeleniumHelper.WaitForExpectedElement(driver, By.ClassName("ok"));

            driver.FindElement(By.ClassName("ok")).Click();
            new WaitFor(driver).ReportsIndexPageToLoad();

            var reportsTable = driver.FindElement(By.Id("reportListTable"));
            var text = reportsTable.Text;

            TestHelper.AssertTextDoesNotContain(text, "Test report modified");
        }

        private void NavigateToReportsIndexPage(IWebDriver driver)
        {
            new NavigateTo(driver).ReportsIndexPage();
            new WaitFor(driver).ReportsIndexPageToLoad();

            Assert.IsTrue(driver.FindElement(By.Id("reportListTable")).Displayed);
        }
    }
}
