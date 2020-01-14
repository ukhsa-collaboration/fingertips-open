using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Fpm.MainUISeleniumTest
{
    [TestClass]
    public class UploadTest : BaseUnitTest
    {
        private readonly string testUploadFile = "upload-tests.xlsx";

        [TestMethod]
        public void UploadFileTest()
        {
            LoadUploadPage();

            var browseForFile = Driver.FindElements(By.Id("BrowseForFileButton")).First();
            browseForFile.Click();

            // find the input element
            var elem = Driver.FindElement(By.XPath("//input[@type='file']"));

            var filePathPartial = ConfigurationManager.AppSettings.Get("TestUploadFilesPath");
            
            var filePath = Path.GetFullPath(filePathPartial + "\\"+ testUploadFile);
            elem.SendKeys(filePath);

            Driver.FindElement(By.Id("uploadButton")).Click();
        }

        [TestMethod]
        public void CancelUploadFileTest()
        {
            LoadUploadPage();

            var browseForFile = Driver.FindElements(By.Id("BrowseForFileButton")).First();
            browseForFile.Click();

            // find the input element
            var elem = Driver.FindElement(By.XPath("//input[@type='file']"));
            
            var filePathPartial = ConfigurationManager.AppSettings.Get("TestUploadFilesPath");

            var filePath = Path.GetFullPath(filePathPartial + "\\"+ testUploadFile);
            elem.SendKeys(filePath);

            Driver.FindElement(By.Id("cancelButton")).Click();
        }

        private void LoadUploadPage()
        {
            navigateTo.UploadFilePage();
            waitFor.BatchUploadForm();
        }
    }
}
