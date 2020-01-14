using System;
using System.Linq;
using System.Threading;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsIndicatorListTest : FingertipsBaseUnitTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            NavigateToSignInPage();
            SignIn();
        }

        [TestCleanup]
        public void CleanUp()
        {
            SignOut();
        }

        [TestMethod]
        public void Test_1_Can_Create_New_Indicator_List()
        {
            NavigateToCreateNewIndicatorListPage();
            CreateNewIndicatorList();
        }

        [TestMethod]
        public void Test_2_Can_Edit_Indicator_List()
        {
            NavigateToEditIndicatorListPage();
            EditIndicatorList();
        }

        [TestMethod]
        public void Test_3_Can_View_Indicator_List()
        {
            NavigateToViewIndicatorListPage();
        }

        [TestMethod]
        public void Test_4_Redirect_To_Search_Page_After_Edit()
        {
            NavigateToViewIndicatorListPage();

            var editIndicatorListLink = driver.FindElement(By.Id("edit-indicator-list-link"));
            Assert.IsTrue(editIndicatorListLink.Displayed);

            editIndicatorListLink.Click();
            waitFor.EditIndicatorListPageToLoad();

            driver.FindElement(By.Id("save-indicator-list-button")).Click();
            waitFor.ViewIndicatorListPageToLoad();
            Assert.IsTrue(driver.FindElement(By.Id("left-tartan-table")).Displayed);
        }


        [TestMethod]
        public void Test_5_Can_Copy_Indicator_List()
        {
            CopyIndicatorList();
        }

        [TestMethod]
        public void Test_6_Can_Delete_Indicator_Lists()
        {
            DeleteIndicatorLists();
        }

        private void NavigateToSignInPage()
        {
            var menu = NavigateToYourDataMenu();

            // Click sign in
            var signIn = menu.FindElement(By.ClassName("dropdown-item"));
            TestHelper.AssertElementTextIsEqual("Sign in", signIn);
            signIn.Click();

            waitFor.SignInPageToLoad();

            // Assert sign in page loaded
            var heading = driver.FindElement(By.Id("frm-login-for-fingertips")).FindElement(By.TagName("h2"));
            TestHelper.AssertElementTextIsEqual("Sign in", heading);
        }

        private void SignIn()
        {
            driver.FindElement(By.Id("email")).SendKeys(SeleniumUser.EmailAddress);

            driver.FindElement(By.Id("pwd")).SendKeys(SeleniumUser.Password);

            driver.FindElement(By.Id("btn-login")).Click();

            waitFor.IndicatorListPageToLoad();

            // Assert indicator list page loaded
            var createNewIndicatorListButton = driver.FindElement(By.Id("lnk-create-new-indicator-list"));
            TestHelper.AssertElementTextIsEqual("Create new indicator list", createNewIndicatorListButton);
        }

        private void SignOut()
        {
            var menu = NavigateToYourDataMenu();

            menu.FindElement(By.LinkText("Sign out")).Click();

            waitFor.SignOutPageToLoad();

            var signedOutTextElement = driver.FindElement(By.Id("div-content"))
                .FindElement(By.TagName("h3"));

            TestHelper.AssertElementTextIsEqual("You have successfully signed out", signedOutTextElement);
        }

        private IWebElement NavigateToYourDataMenu()
        {
            navigateTo.HomePage();

            waitFor.HomePageToLoad();

            var menu = driver.FindElement(By.Id("your-account"));

            // Click menu to display options
            var accountButton = menu.FindElement(By.Id("your-account-button"));
            TestHelper.AssertElementTextIsEqual("Your data", accountButton);
            accountButton.Click();

            return menu;
        }

        private void NavigateToCreateNewIndicatorListPage()
        {
            driver.FindElement((By.Id("lnk-create-new-indicator-list"))).Click();

            waitFor.CreateNewIndicatorListPageToLoad();

            // Assert create new indicator list page loaded
            Assert.IsTrue(driver.FindElement(By.Id("list-name")).Displayed);
        }

        private void NavigateToEditIndicatorListPage()
        {
            driver.FindElement(By.Id("lnk-edit-indicator-list")).Click();

            waitFor.EditIndicatorListPageToLoad();

            // Assert edit indicator list page loaded
            Assert.IsTrue(driver.FindElement(By.Id("save-indicator-list-button")).Displayed);
        }

        private void NavigateToViewIndicatorListPage()
        {
            driver.FindElement(By.Id("lnk-view-indicator-list")).Click();

            waitFor.ViewIndicatorListPageToLoad();

            // Assert indicator list page loaded
            Assert.IsTrue(driver.FindElement(By.Id("left-tartan-table")).Displayed);
        }

        private void CreateNewIndicatorList()
        {
            SearchIndicators();
            SelectIndicators();
            SaveIndicatorList(true);
        }

        private void EditIndicatorList()
        {
            ClearSelectedIndicators();
            SearchIndicators();
            SelectIndicators();
            SaveIndicatorList(false);
        }

        private void ClearSelectedIndicators()
        {
            var clearListButon = driver.FindElement(By.Id("clear-list-button"));

            // Assert clear indicator list button displayed
            Assert.IsTrue(clearListButon.Displayed);

            clearListButon.Click();
        }

        private void SearchIndicators()
        {
            waitFor.ProfilesToLoadOnIndicatorListPage();

            driver.FindElement(By.Id("search-indicator")).SendKeys("diabetes");

            var profileDropDown = driver.FindElement(By.Id("profile-list"));
            var profileDropDownElement = new SelectElement(profileDropDown);
            profileDropDownElement.SelectByValue("139");

            driver.FindElement(By.Id("search-indicator-button")).Click();

            waitFor.IndicatorsToLoadOnIndicatorListPage();

            var indicators = driver.FindElement(By.Id("indicator-list"));

            var sexId = indicators.FindElement(By.TagName("div")).GetAttribute("sex");

            // Assert sex id matches
            Assert.AreEqual(SexIds.Persons, Convert.ToInt32(sexId));
        }

        private void SelectIndicators()
        {
            var firstElement = driver.FindElement(By.Id("indicator-list")).FindElement(By.TagName("div"));

            Actions actions = new Actions(driver);
            actions.MoveToElement(firstElement).Perform();
            
            var innerElement = firstElement.FindElement(By.TagName("div"));
            innerElement.Click();
        }

        private void SaveIndicatorList(bool updateName)
        {
            if (updateName)
            {
                driver.FindElement(By.Id("list-name")).SendKeys(Guid.NewGuid().ToString());
            }

            driver.FindElement(By.Id("save-indicator-list-button")).Click();

            waitFor.IndicatorListPageToLoad();

            // Assert save indicator list
            var createNewIndicatorListButton = driver.FindElement(By.Id("lnk-create-new-indicator-list"));
            TestHelper.AssertElementTextIsEqual("Create new indicator list", createNewIndicatorListButton);
        }

        private void CopyIndicatorList()
        {
            driver.FindElement(By.Id("lnk-copy-indicator-list")).Click();
            waitFor.CopyIndicatorListPopupToLoad();

            var indicatorListName = driver.FindElement(By.Id("indicator-list-name")).GetAttribute("value");
            driver.FindElement(By.Id("btn-copy-indicator-list")).Click();
            waitFor.IndicatorListPageTableToLoad();

            var copiedIndicatorElement = driver.FindElement(By.ClassName("webgrid-table"))
                .FindElement(By.TagName("tbody"))
                .FindElements(By.TagName("tr"))[0]
                .FindElements(By.TagName("td"))[0];

            TestHelper.AssertElementTextIsEqual(indicatorListName, copiedIndicatorElement);
        }

        private void DeleteIndicatorLists()
        {
            IWebElement firstIndicatorListElement;

            // Check whether at least one indicator list record is present
            // If not then exit this method
            try
            {
                firstIndicatorListElement = driver.FindElements(By.Id("lnk-delete-indicator-list")).First();
            }
            catch (Exception)
            {
                // No indicator list to delete, exit this method
                return;
            }

            // No indicator list to delete, exit this method
            if (firstIndicatorListElement == null)
            {
                return;
            }

            // Delete the indicator list
            DeleteIndicatorList(firstIndicatorListElement);

            // Call this method recursively
            DeleteIndicatorLists();
        }

        private void DeleteIndicatorList(IWebElement indicatorListElement)
        {
            indicatorListElement.Click();
            waitFor.DeleteIndicatorListPopupToLoad();

            // Click on Ok button in the info box
            driver.FindElement(By.Id("btn-delete-confirm-indicator-list")).Click();

            // Wait for page to refresh
            Thread.Sleep(1000);
        }
    }
}
