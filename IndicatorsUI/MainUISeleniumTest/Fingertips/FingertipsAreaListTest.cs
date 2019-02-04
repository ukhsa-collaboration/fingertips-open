using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using IndicatorsUI.DomainObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsAreaListTest : FingertipsBaseUnitTest
    {
        private IList<string> _listNamesToDelete = new List<string>();

        [TestInitialize]
        public void Initialize()
        {
            _listNamesToDelete = new List<string>();
            NavigateToSignInPage();
            SignIn();
        }

        [TestCleanup]
        public void CleanUp()
        {
            // Delete area lists
            foreach (var listName in _listNamesToDelete)
            {
                //TODO Figure out how to delete these lists!
            }

            SignOut();
        }

        [TestMethod]
        public void Test_Can_Create_New_Area_List()
        {
            NavigateToCreateNewAreaListPage();
            CreateNewAreaList();
        }

        [TestMethod]
        public void Test_Can_Edit_Area_List()
        {
            NavigateToEditAreaListPage();
            EditAreaList();
        }

        [TestMethod]
        public void Test_Can_Copy_Area_List()
        {
            NavigateToAreaListPage();
            CopyAreaList();
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

            var signInLinkText = driver.FindElement(By.LinkText("Sign in"));
            TestHelper.AssertElementTextIsEqual("Sign in", signInLinkText);
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

        private void NavigateToAreaListPage()
        {
            driver.FindElement(By.Id("area-list-button")).Click();
            waitFor.AreaListPageTableToLoad();
        }

        private void NavigateToCreateNewAreaListPage()
        {
            NavigateToAreaListPage();

            driver.FindElement(By.Id("create-new-area-list")).Click();
            waitFor.CreateNewAreaListPageToLoad();
        }

        private void NavigateToEditAreaListPage()
        {
            NavigateToAreaListPage();

            // Click edit button
            driver.FindElements(By.ClassName("edit-link-button"))[0].Click();

            waitFor.EditAreaListPageToLoad();
        }

        private void CreateNewAreaList()
        {
            var areaTypeDropDown = driver.FindElement(By.Id("areaTypeList"));
            var areaTypeDropDownElement = new SelectElement(areaTypeDropDown);
            areaTypeDropDownElement.SelectByValue(AreaTypeIds.AcuteTrusts.ToString());

            waitFor.AreaListAreasTableToLoad();

            Assert.IsTrue(GetAreas().Count > 0);

            // Select some areas
            GetAreas()[0].Click();
            GetAreas()[1].Click();

            // Search for some areas
            driver.FindElement(By.Id("areaSearchText")).SendKeys("bar");
            Assert.IsTrue(GetSearchedAreas().Any());
            GetSearchedAreas()[0].Click();
            Assert.IsTrue(driver.FindElements(By.ClassName("selected-areas")).Any());

            // Set list name
            var areaListName = Guid.NewGuid().ToString();
            _listNamesToDelete.Add(areaListName);
            driver.FindElement(By.Id("areaListName")).SendKeys(areaListName);

            SaveList();

            // Check that list is available on index page
            waitFor.AreaListPageToLoad();
            var nameElement = GetNameElementOfFirstList();
            TestHelper.AssertElementTextIsEqual(areaListName, nameElement);
        }

        private ReadOnlyCollection<IWebElement> GetSearchedAreas()
        {
            return driver.FindElements(By.ClassName("searched-areas"));
        }

        private void EditAreaList()
        {
            Assert.IsTrue(GetAreas().Count > 0);
            GetAreas()[2].Click();
            GetAreas()[3].Click();

            driver.FindElement(By.Id("areaSearchText")).SendKeys("ac");
            Assert.IsTrue(GetSearchedAreas().Any());
            GetSearchedAreas()[0].Click();

            Assert.IsTrue(driver.FindElements(By.ClassName("selected-areas")).Any());

            var areaListName = driver.FindElement(By.Id("areaListName")).GetAttribute("value");
            SaveList();

            // Check list name as expected
            waitFor.AreaListPageToLoad();
            var nameElement = GetNameElementOfFirstList();
            TestHelper.AssertElementTextIsEqual(areaListName, nameElement);
        }

        private ReadOnlyCollection<IWebElement> GetAreas()
        {
            return driver.FindElements(By.ClassName("areas"));
        }

        private void CopyAreaList()
        {
            // Click copy
            driver.FindElements(By.Id("copy-area-list")).First().Click();
            waitFor.CopyAreaListPopupToLoad();

            // Copy list
            var areaListName = driver.FindElement(By.Id("info-box-text-input"))
                .GetAttribute("value");
            _listNamesToDelete.Add(areaListName);
            driver.FindElement(By.ClassName("ok")).Click();

            // Wait for page to refresh
            Thread.Sleep(1000);
            waitFor.PageToFinishLoading();
            waitFor.AreaListPageTableToLoad();

            // Check list copy name
            var nameElement = GetNameElementOfFirstList();
            var count = 0;
            while (count < WaitFor.TimeoutLimitInSeconds && 
                   nameElement.Text.Equals(areaListName) == false)
            {
                // Need to wait until page has refreshed with new list
                Thread.Sleep(1000);
                count++;
                waitFor.AreaListPageTableToLoad();
                nameElement = GetNameElementOfFirstList();
            }

            // Check list has been copied
            TestHelper.AssertElementTextIsEqual(areaListName, nameElement);
        }

        private IWebElement GetNameElementOfFirstList()
        {
            IWebElement nameElement =null;

            var count = 0;
            while (nameElement == null && count < WaitFor.TimeoutLimitInSeconds)
            {
                try
                {
                    nameElement = GetNameElement();
                }
                catch (StaleElementReferenceException)
                {
                    // For case when page has refreshed since getting table rows
                    Thread.Sleep(1000);
                }
            }

            return nameElement;
        }

        private IWebElement GetNameElement()
        {
            IWebElement row;
            IWebElement nameElement;
            row = driver.FindElements(By.ClassName("webgrid-row-style"))
                .First();
            nameElement = row.FindElements(By.TagName("td")).First();
            return nameElement;
        }

        private void SaveList()
        {
            driver.FindElement(By.Id("btn-save-area-list")).Click();
        }
    }
}
