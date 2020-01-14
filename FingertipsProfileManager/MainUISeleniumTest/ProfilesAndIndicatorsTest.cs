using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Fpm.MainUISeleniumTest
{
    [TestClass]
    public class ProfilesAndIndicatorsTest : BaseUnitTest
    {
        private IWebDriver _driver;
        private NavigateTo _navigateTo;
        private WaitFor _waitFor;
        private ProfilesAndIndicatorsTestHelper _helper;

        [TestInitialize]
        public void TestInitialize()
        {
            _driver = Driver;
            _navigateTo = new NavigateTo(_driver);
            _waitFor = new WaitFor(_driver);
            _helper = new ProfilesAndIndicatorsTestHelper(_driver, _navigateTo, _waitFor);

            // Navigate profiles & indicators page
            _helper.NavigateToProfilesAndIndicatorsPage();
        }

        [TestMethod]
        public void Test_ProfilesAndIndicatorsPageLoads()
        {
            // Is one table of indicators
            var indicatorTable = _driver.FindElements(By.ClassName("grid"));
            Assert.AreEqual(1, indicatorTable.Count);
        }

        [TestMethod]
        public void Test_DisplayProfilesOwnersAsUserInformation()
        {
            const string warningText = @"This indicator is owned by 'Cardiovascular Disease'
For metadata changes to Specific rationale and Indicator number use Override
For changes to other fields contact the owner profile team: Andrew Hughes, James Hollinshead";
            
            _helper.SelectProfile(UrlKeys.Diabetes);
            _helper.SelectDomain(1);
            _helper.SelectAreaType(AreaTypeIds.CountyAndUnitaryAuthorityPre2019);
            _helper.ClickElement("//*[@id='tbl-profiles-and-indicators']/tbody/tr[1]/td[3]/a", FindElement.ByXPath);
            WaitFor.ThreadWait(0.5);

            // Is one table of indicators
            var indicatorWarning = _driver.FindElements(By.ClassName("warning"));
            Assert.AreEqual(warningText, indicatorWarning.First().Text);
        }

        /// <summary>
        /// Ignored because will not run in either dev or Jenkins
        /// </summary>
        [TestMethod]
        [Ignore]
        public void Test_ReorderIndicators()
        {
            // Select profile, domain and area type combination
            _helper.SelectProfile(UrlKeys.DevelopmentProfileForTesting);
            _helper.SelectDomain(1);
            _helper.SelectAreaType(AreaTypeIds.Country);

            // Navigate to reorder indicators page
            _helper.ClickElement("reorder-indicators", FindElement.ById);
            _waitFor.ReorderIndicatorsPageToLoad();

            // Open add subheading pop up
            _helper.ClickElement("reorder-add-subheading", FindElement.ById);
            _waitFor.AddSubheadingPopupToLoad();

            // Add subheading
            var guid = Guid.NewGuid().ToString();
            _helper.InputText("txt-light-box", guid);
            _helper.ClickElement("btn-save-subheading", FindElement.ById);
            _waitFor.ReorderIndicatorsTableToLoad();
            
            // Check subheading was not created
            TestHelper.AssertTextContains(_driver.FindElement(By.XPath("//*[@id='table']/tbody")).Text, guid,
                "Subheading " + guid + " not created");
        }

        [TestMethod]
        public void Test_1_Create_Indicator()
        {
            _helper.ClickNewIndicatorButton();
            _helper.FillInScreenInput();
            _helper.SaveIndicator();

            Assert.IsTrue(_helper.GetNewlyCreatedIndicatorId() > -1, "The new indicator hasn't been created");
        }

        [TestMethod]
        public void Test_2_Create_New_Indicator_By_Copying_This_Metadata()
        {
            _helper.SelectProfile(UrlKeys.DevelopmentProfileForTesting);
            _helper.SelectDomain(6);
            _helper.SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);

            _helper.ClickElement("//*[@id='tbl-profiles-and-indicators']/tbody/tr[1]/td[2]/a", FindElement.ByXPath);
            _waitFor.EditIndicatorPageToLoad();

            _helper.ClickElement("copy-indicator", FindElement.ById);
            _waitFor.InfoBoxPopupToLoad();

            var profileSelect = _driver.FindElement(By.Id("ProfileSelectedToCopy"));
            var selectElement = new SelectElement(profileSelect);
            selectElement.SelectByValue(UrlKeys.DevelopmentProfileForTesting);

            _helper.ClickElement("confirm-copy-indicator", FindElement.ById);
            _waitFor.InfoBoxPopupToLoad();

            var indicatorId = _driver.FindElement(By.Id("new-indicator-id")).Text;
            _helper.ClickElement("indicator-created-confirmation", FindElement.ById);
            _waitFor.ProfilesAndIndicatorsPageToLoad();

            _helper.SelectProfile(UrlKeys.IndicatorsForReview);
            _helper.SelectDomain(1);
            _helper.SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);

            var tableContent = _driver.FindElement(By.Id("tbl-profiles-and-indicators")).Text;
            Assert.IsTrue(tableContent.Contains(indicatorId));
        }

        [TestMethod]
        public void Test_3_Edit_Indicator()
        {
            _helper.ClickElement("//*[@id='tbl-profiles-and-indicators']/tbody/tr[1]/td[2]/a", FindElement.ByXPath);
            _waitFor.ExpectedElementToBePresent(By.XPath("//a[@id='ui-id-1']"));

            _helper.ClickElement("show-empty", FindElement.ById);
            _waitFor.ExpectedElementToBePresent(By.Id("override-specific-rationale"));

            _helper.ClickElement("override-specific-rationale", FindElement.ById);
            SeleniumHelper.WaitForExpectedElement(_driver, By.XPath("//iframe[@id='metaDataText_ifr']"));
            _driver.SwitchTo().Frame("metaDataText_ifr");
            var editable = _driver.SwitchTo().ActiveElement();
            editable.SendKeys(" modified");
            _driver.SwitchTo().DefaultContent();
            _helper.ClickElement("//input[@id='editor-done']", FindElement.ByXPath);

            _helper.ClickElement("update-indicator", FindElement.ById);
            _waitFor.ProfilesAndIndicatorsPageToLoad();
        }

        [TestMethod]
        public void Test_4_Add_Review_Note()
        {
            _helper.SelectProfile(UrlKeys.DevelopmentProfileForTesting);
            _helper.SelectDomain(6);
            _helper.SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);

            var trElementBeforeReviewNote = _driver.FindElement(By.XPath("//*[@id='tbl-profiles-and-indicators']/tbody/tr[1]/td[2]/a"));
            var indicatorId = trElementBeforeReviewNote.Text;
            trElementBeforeReviewNote.Click();
            _waitFor.EditIndicatorPageToLoad();

            _helper.ClickElement("//a[@id='ui-id-5']", FindElement.ByXPath);
            _waitFor.EditIndicatorMetadataReviewTabToLoad();

            var currentDate = DateTime.Now.ToString("dd-MM-yyyy");
            var reviewAuditNote = string.Format("Review audit note entered on {0}", currentDate);

            _helper.InputText("IndicatorMetadataReviewAudit_Notes", reviewAuditNote);
            _helper.InputText("NextReviewTimestamp", currentDate);
            _helper.ClickElement("update-indicator", FindElement.ById);
            _waitFor.ProfilesAndIndicatorsPageToLoad();

            var trElementAfterReviewNote = _driver.FindElement(By.XPath("//*[@id='tbl-profiles-and-indicators']/tbody/tr[1]/td[2]/a"));
            Assert.AreEqual(indicatorId, trElementAfterReviewNote.Text);

            trElementAfterReviewNote.Click();
            _waitFor.EditIndicatorPageToLoad();

            _helper.ClickElement("//a[@id='ui-id-5']", FindElement.ByXPath);
            _waitFor.EditIndicatorMetadataReviewTabToLoad();

            var tdReviewAuditNote = _driver.FindElement(By.XPath("//*[@id='tbl-review-history']/tbody/tr[1]/td[2]"));
            Assert.AreEqual(tdReviewAuditNote.Text, reviewAuditNote);
        }

        [TestMethod]
        public void Test_5_Copy_Indicator()
        {
            _helper.ClickElement("indicator-check-box", FindElement.ByClassName);
            _helper.ClickElement("copy-indicators-button", FindElement.ById);
            _waitFor.InfoBoxPopupToLoad();

            _helper.ClickElement("ConfirmCopy", FindElement.ById);
            _waitFor.ProfilesAndIndicatorsPageToLoad();
        }

        /// <summary>
        /// Will fail if user (Doris) is not a Reviewer
        /// </summary>
        [TestMethod]
        public void Test_6_Submit_Indicator_For_Review()
        {
            _helper.SelectProfile(UrlKeys.IndicatorsForReview);
            _helper.SelectDomain(1);
            _helper.SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);

            var indicatorId = _driver.FindElement(By.XPath("//*[@id='tbl-profiles-and-indicators']/tbody/tr[1]/td[2]/a")).Text;
            _helper.ClickElement(indicatorId + "_selected", FindElement.ByName);

            _helper.ClickElement("submit-indicators-for-review-button", FindElement.ById);
            _waitFor.InfoBoxPopupToLoad();

            _helper.ClickElement("submit-indicators-for-review-confirm-button", FindElement.ById);
            _waitFor.ProfilesAndIndicatorsPageToLoad();

            _helper.SelectDomain(2);
            _helper.SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);

            var tableContent = _driver.FindElement(By.Id("tbl-profiles-and-indicators")).Text;
            Assert.IsTrue(tableContent.Contains(indicatorId));
        }

        [TestMethod]
        public void Test_7_Buttons_Visibility_For_UnderReview()
        {
            _helper.SelectProfile(UrlKeys.IndicatorsForReview);
            _helper.SelectDomain(2);
            _helper.SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);

            // Awaiting revision button must be displayed
            Assert.IsTrue(_driver.FindElement(By.Id("indicators-awaiting-revision-button")).Displayed);

            // Approve indicators button must be displayed
            Assert.IsTrue(_driver.FindElement(By.Id("approve-indicators-button")).Displayed);

            // Reject indicators button must be displayed
            Assert.IsTrue(_driver.FindElement(By.Id("reject-indicators-button")).Displayed);

            // Submit for review button must not be visible
            try
            {
                _driver.FindElement(By.Id("submit-indicators-for-review-button"));
                Assert.Fail("The submit for review button must not be visible");
            }
            catch (NoSuchElementException)
            {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        /// Will fail if user (Doris) is not a Reviewer
        /// </summary>
        [TestMethod]
        public void Test_8_Indicator_Awaiting_Revision()
        {
            _helper.SelectProfile(UrlKeys.IndicatorsForReview);
            _helper.SelectDomain(2);
            _helper.SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);

            var indicatorId = _driver.FindElement(By.XPath("//*[@id='tbl-profiles-and-indicators']/tbody/tr[1]/td[2]/a")).Text;
            _helper.ClickElement(indicatorId + "_selected", FindElement.ByName);

            _helper.ClickElement("indicators-awaiting-revision-button", FindElement.ById);
            _waitFor.InfoBoxPopupToLoad();

            _helper.ClickElement("indicators-awaiting-revision-confirm-button", FindElement.ById);
            _waitFor.ProfilesAndIndicatorsPageToLoad();

            _helper.SelectDomain(3);
            _helper.SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);

            var tableContent = _driver.FindElement(By.Id("tbl-profiles-and-indicators")).Text;
            Assert.IsTrue(tableContent.Contains(indicatorId));
        }

        [TestMethod]
        public void Test_9_Buttons_Visibility_For_InDevelopment()
        {
            _helper.SelectProfile(UrlKeys.IndicatorsForReview);
            _helper.SelectDomain(1);
            _helper.SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);

            // Submit for review button must be visible
            Assert.IsTrue(_driver.FindElement(By.Id("submit-indicators-for-review-button")).Displayed);

            // Withdraw indicators button must be visible
            Assert.IsTrue(_driver.FindElement(By.Id("withdraw-indicators-button")).Displayed);

            // Indicators awaiting revision button must not be visible
            try
            {
                _driver.FindElement(By.Id("indicators-awaiting-revision-confirm-button"));
                Assert.Fail("The indicators awaiting revision button must not be visible");
            }
            catch (NoSuchElementException)
            {
                Assert.IsTrue(true);
            }

            // Approve indicators button must not be visible
            try
            {
                _driver.FindElement(By.Id("approve-indicators-button"));
                Assert.Fail("The approve indicators button must not be visible");
            }
            catch (NoSuchElementException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Test_10_Approve_Indicator()
        {
            // Submit indicators for review, so that it can be approved
            Test_6_Submit_Indicator_For_Review();

            _helper.SelectProfile(UrlKeys.IndicatorsForReview);
            _helper.SelectDomain(2);
            _helper.SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);

            var indicatorId = _driver.FindElement(By.XPath("//*[@id='tbl-profiles-and-indicators']/tbody/tr[1]/td[2]/a")).Text;
            _helper.ClickElement(indicatorId + "_selected", FindElement.ByName);

            var reader = ReaderFactory.GetProfilesReader();
            var indicatorMetadata = reader.GetIndicatorMetadata(int.Parse(indicatorId));
            var groupings = reader.GetGroupingByIndicatorId(new List<int>() { int.Parse(indicatorId) });
            var profileUrlKey = reader.GetProfileUrlKeyFromId(indicatorMetadata.DestinationProfileId);

            _helper.ClickElement("approve-indicators-button", FindElement.ById);
            _waitFor.InfoBoxPopupToLoad();

            _helper.ClickElement("indicators-to-approve-confirm-button", FindElement.ById);
            _waitFor.ProfilesAndIndicatorsPageToLoad();

            _helper.SelectProfile(profileUrlKey);
            _helper.SelectDomain(1);
            _helper.SelectAreaType(groupings[0].AreaTypeId);

            var tableContent = _driver.FindElement(By.Id("tbl-profiles-and-indicators")).Text;
            Assert.IsTrue(tableContent.Contains(indicatorId));
        }

        [TestMethod]
        public void Test_Reject_Indicator()
        {
            // Create indicator and submit it for review
            // so that it can be rejected
            Test_1_Create_Indicator();
            Test_6_Submit_Indicator_For_Review();

            _helper.SelectProfile(UrlKeys.IndicatorsForReview);
            _helper.SelectDomain(2);
            _helper.SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);

            var indicatorId = _driver.FindElement(By.XPath("//*[@id='tbl-profiles-and-indicators']/tbody/tr[1]/td[2]/a")).Text;
            _helper.ClickElement(indicatorId + "_selected", FindElement.ByName);

            _helper.ClickElement("reject-indicators-button", FindElement.ById);
            _waitFor.InfoBoxPopupToLoad();

            _helper.ClickElement("ConfirmDelete", FindElement.ById);
            _waitFor.ProfilesAndIndicatorsPageToLoad();

            var tableContent = _driver.FindElement(By.Id("tbl-profiles-and-indicators")).Text;
            Assert.IsFalse(tableContent.Contains(indicatorId));
        }

        [TestMethod]
        public void Test_Withdraw_Indicator()
        {
            // Create indicator so that it can be withdrawn
            Test_1_Create_Indicator();

            _helper.SelectProfile(UrlKeys.IndicatorsForReview);
            _helper.SelectDomain(1);
            _helper.SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);

            var indicatorId = _driver.FindElement(By.XPath("//*[@id='tbl-profiles-and-indicators']/tbody/tr[1]/td[2]/a")).Text;
            _helper.ClickElement(indicatorId + "_selected", FindElement.ByName);

            _helper.ClickElement("withdraw-indicators-button", FindElement.ById);
            _waitFor.InfoBoxPopupToLoad();

            _helper.ClickElement("ConfirmDelete", FindElement.ById);
            _waitFor.ProfilesAndIndicatorsPageToLoad();

            var tableContent = _driver.FindElement(By.Id("tbl-profiles-and-indicators")).Text;
            Assert.IsFalse(tableContent.Contains(indicatorId));
        }
    }
}
