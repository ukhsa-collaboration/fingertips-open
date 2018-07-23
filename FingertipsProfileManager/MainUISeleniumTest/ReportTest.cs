using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Fpm.MainUISeleniumTest
{
    [TestClass]
    public class ReportTest : BaseUnitTest
    {
        [TestMethod]
        public void ReportsIndex()
        {
            var driver = Driver;
            new NavigateTo(driver).ReportsIndexPage();
            new WaitFor(driver).ReportsIndexPageToLoad();

            var reportsTable = driver.FindElement(By.Name("reportListTable"));
            var text = reportsTable.Text;

            TestHelper.AssertTextContains(text, "Parameters");
        }

    }
}
