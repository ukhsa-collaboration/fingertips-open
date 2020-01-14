using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace Fpm.MainUISeleniumTest
{
    public class WaitFor
    {
        private IWebDriver driver;
        public const int TimeoutLimitInSeconds = 30;
        const double ShortWaitDurationInSeconds = 0.2;


        public WaitFor(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void PageWithModalPopUpToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.ClassName("a-modal"));
        }

        public void UserIndexPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver, By.Id("create-new-user"));
        }

        public void CreateUserPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver, By.Id("Confirm"));
        }

        public void EditUserPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("Confirm"));
        }

        public void ProfilesPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElementToBeVisible(driver,
                By.Id("tbl-profiles"));
        }

        public void ProfilesForNonAdminToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("profile-menu"));
        }

        public void EditProfilePageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("main"));
        }

        public void ProfilesAndIndicatorsPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElementToBeVisible(driver, By.Id("tbl-profiles-and-indicators"));
        }

        public void EditIndicatorPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElementToBeVisible(driver, By.XPath("//a[@id='ui-id-1']"));
        }

        public void EditIndicatorMetadataReviewTabToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver, By.Id("IndicatorMetadataReviewAudit_Notes"));
        }

        public void ContentIndexPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("profileId"));
        }

        public void ReportsIndexPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver, By.Id("newReportButton"));
        }

        public void ReportsEditPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver, By.Id("saveReportButton"));
        }

        public void LookupTablesPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.ClassName("link-options"));
        }

        public void CategoriesPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("edit"));
        }

        public void EditCategoryPageToLoad()
        {
            SeleniumHelper.ThreadWait(3);
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("save"));
        }

        public void ReorderIndicatorsPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("reorder-add-subheading"));
        }

        public void ReorderIndicatorsTableToLoad()
        {
            SeleniumHelper.WaitForExpectedElementToBeVisible(driver,
                By.XPath("//*[@id='table']/tbody"));
        }

        public void AddSubheadingPopupToLoad()
        {
            ThreadWait(3);
            SeleniumHelper.WaitForExpectedElementToBeVisible(driver,
                By.Id("btn-save-subheading"));
        }

        public void IFrameToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("tinymce"));
        }

        public void BatchUploadForm()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("batchUploadForm"));
        }

        public void CopyIndicatorsPopupToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("copyIndicators"));
        }

        public void IndicatorRowToLoad(int indicatorId)
        {
            SeleniumHelper.WaitForExpectedElementToBeVisible(driver,
                By.Name(indicatorId + "_selected"));
        }

        public void InfoBoxPopupToLoad()
        {
            SeleniumHelper.WaitForExpectedElementToBeVisible(driver,
                By.Id("infoBox"));
        }

        public void SubmitIndicatorsForReviewPopupToLoad()
        {
            SeleniumHelper.WaitForExpectedElementToBeVisible(driver,
                By.Id("submit-indicators-for-review-confirm-button"));
        }

        public void IndicatorsAwaitingRevisionPopupToLoad()
        {
            SeleniumHelper.WaitForExpectedElementToBeVisible(driver,
                By.Id("indicators-awaiting-revision-confirm-button"));
        }

        /// <summary>
        /// Wait for an element to be present on the page.
        /// </summary>
        public void ExpectedElementToBePresent(By element)
        {
            SeleniumHelper.WaitForExpectedElementToBeVisible(driver, element);
        }

        public void ElementToContainText(IWebElement element, string text)
        {
            const double totalSecondsToCheckFor = TimeoutLimitInSeconds;
            double secondsCheckedFor = 0;
            while (secondsCheckedFor < totalSecondsToCheckFor)
            {
                if (element.Text.ToLower().Contains(text.ToLower()))
                {
                    return;
                }

                // Wait until next try
                ThreadWait(ShortWaitDurationInSeconds);
                secondsCheckedFor += ShortWaitDurationInSeconds;
            }

            Assert.Fail("Element did not contain expected text after waiting");
        }

        public void ElementToNotContainText(IWebElement element, string text)
        {
            const double totalSecondsToCheckFor = TimeoutLimitInSeconds;
            double secondsCheckedFor = 0;
            while (secondsCheckedFor < totalSecondsToCheckFor)
            {
                if (element.Text.ToLower().Contains(text.ToLower()) == false)
                {
                    return;
                }

                // Wait until next try
                ThreadWait(ShortWaitDurationInSeconds);
                secondsCheckedFor += ShortWaitDurationInSeconds;
            }

            Assert.Fail("Element did not contain expected text after waiting");
        }

        /// <summary>
        /// DO NOT USE unless you really have to!!
        /// Use ExpectedElementToBeVisible to be visible instead.
        /// </summary>
        public static void ThreadWait(double seconds)
        {
            Thread.Sleep((int)(seconds * 1000));
        }
    }
}
