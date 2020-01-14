using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsTabDownloadReportsTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Where_Pdf_Link_Is_Displayed()
        {
            // Navigate to data page for district & UA
            var parameters = new HashParameters();
            parameters.AddAreaTypeId(AreaTypeIds.CountyAndUnitaryAuthorityPre2019);
            navigateTo.GoToUrl(ProfileUrlKeys.HealthProfiles + parameters.HashParameterString);
            waitFor.FingertipsOverviewTabToLoad();

            // Select download tab
            fingertipsHelper.SelectTab(FingertipsIds.TabDownload);
            waitFor.ExpectedElementToBeVisible(By.Id("pdf-download-text"));

            // Assert PDF message is correct
            var text = driver.FindElement(By.Id("pdf-download-text")).Text;
            TestHelper.AssertTextContains(text, "Download a detailed report", "");

            // Assert PDF link is visible
            waitFor.ExpectedElementToBeVisible(By.ClassName("pdf"));
        }

        [TestMethod]
        public void Test_Where_Pdfs_Not_Available_Message_Is_Displayed()
        {
            // Navigate to data page 
            var parameters = new HashParameters();
            parameters.AddAreaTypeId(AreaTypeIds.Region);
            navigateTo.GoToUrl("profile/" + ProfileUrlKeys.HealthProfiles +"/data" + parameters.HashParameterString);
            waitFor.FingertipsOverviewTabToLoad();

            // Select download tab
            fingertipsHelper.SelectTab(FingertipsIds.TabDownload);
            waitFor.ExpectedElementToBeVisible(By.Id("pdf-not-available-text"));

            // Assert no PDF message is displayed
            var text = driver.FindElement(By.Id("pdf-not-available-text")).Text;
            TestHelper.AssertTextContains(text,
                "PDF profiles are not currently available for Region");
        }

    }
}
