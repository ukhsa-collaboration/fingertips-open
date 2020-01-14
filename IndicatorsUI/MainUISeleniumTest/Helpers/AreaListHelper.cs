using IndicatorsUI.DomainObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace IndicatorsUI.MainUISeleniumTest.Helpers
{
    public class AreaListHelper
    {
        private readonly WaitFor _waitFor;
        private readonly IWebDriver _driver;
        private readonly NavigateTo _navigateTo;

        public AreaListHelper(IWebDriver driver)
        {
            _driver = driver;
            _waitFor = new WaitFor(driver);
            _navigateTo = new NavigateTo(driver);
        }

        public void NavigateToSignInAreaListPage()
        {
            var menu = NavigateToYourDataMenu();

            // Click sign in
            var signIn = menu.FindElement(By.ClassName("dropdown-item"));
            TestHelper.AssertElementTextIsEqual("Sign in", signIn);
            signIn.Click();

            _waitFor.SignInPageToLoad();

            // Assert sign in page loaded
            var heading = _driver.FindElement(By.Id("frm-login-for-fingertips")).FindElement(By.TagName("h2"));
            TestHelper.AssertElementTextIsEqual("Sign in", heading);
        }

        public void SignInAreaListPage()
        {
            _driver.FindElement(By.Id("email")).SendKeys(SeleniumUser.EmailAddress);

            _driver.FindElement(By.Id("pwd")).SendKeys(SeleniumUser.Password);

            _driver.FindElement(By.Id("btn-login")).Click();

            _waitFor.IndicatorListPageToLoad();

            // Assert indicator list page loaded
            var createNewIndicatorListButton = _driver.FindElement(By.Id("lnk-create-new-indicator-list"));
            TestHelper.AssertElementTextIsEqual("Create new indicator list", createNewIndicatorListButton);
        }

        public void SignOutAreaListPage()
        {
            var menu = NavigateToYourDataMenu();

            menu.FindElement(By.LinkText("Sign out")).Click();

            _waitFor.SignOutPageToLoad();

            var signInLinkText = _driver.FindElement(By.ClassName("form-label-bold"));
            TestHelper.AssertElementTextIsEqual("Sign in", signInLinkText);
        }

        public void NavigateToCreateNewAreaListPage()
        {
            _driver.FindElement(By.Id("area-list-button")).Click();
            _waitFor.AreaListPageToLoadForCreateAction();

            _driver.FindElement(By.Id("create-new-area-list")).Click();
            _waitFor.CreateNewAreaListPageToLoad();
        }

        public void NavigateToEditAreaListPage()
        {
            NavigateToAreaListPage();

            // Click edit button
            _driver.FindElements(By.Id("edit-area-list"))[0].Click();

            _waitFor.EditAreaListPageToLoad();
        }

        public void CreateNewAreaList()
        {
            var areaTypeDropDown = _driver.FindElement(By.Id("areaTypeList"));
            var areaTypeDropDownElement = new SelectElement(areaTypeDropDown);
            areaTypeDropDownElement.SelectByValue(AreaTypeIds.AcuteTrusts.ToString());

            _waitFor.AreaListAreasTableToLoad();

            Assert.IsTrue(GetAreas().Count > 0);

            // Select some areas
            var areas = GetAreas();
            areas[0].Click();
            areas[1].Click();

            // Search for some areas
            _driver.FindElement(By.Id("areaSearchText")).SendKeys("bar");
            var searchedAreas = GetSearchedAreas();

            Assert.IsTrue(searchedAreas.Any());
            searchedAreas[0].Click();
            Assert.IsTrue(_driver.FindElements(By.ClassName("selected-areas")).Any());

            // Set list name
            var areaListName = Guid.NewGuid().ToString();
            _driver.FindElement(By.Id("areaListName")).SendKeys(areaListName);

            SaveList();

            // Check that list is available on index page
            _waitFor.AreaListPageTableToLoad();
            var nameElement = GetNameElementOfFirstList();
            TestHelper.AssertElementTextIsEqual(areaListName, nameElement);
        }

        public void EditAreaList()
        {
            _waitFor.AreaListAreasTableToLoad();

            Assert.IsTrue(GetAreas().Count > 0);

            // Select some areas
            GetAreas()[2].Click();
            GetAreas()[3].Click();

            _driver.FindElement(By.Id("areaSearchText")).SendKeys("ac");
            Assert.IsTrue(GetSearchedAreas().Any());
            GetSearchedAreas()[0].Click();

            Assert.IsTrue(_driver.FindElements(By.ClassName("selected-areas")).Any());

            var areaListName = _driver.FindElement(By.Id("areaListName")).GetAttribute("value");
            SaveList();

            // Check list name as expected
            _waitFor.AreaListPageTableToLoad();
            var nameElement = GetNameElementOfFirstList();
            TestHelper.AssertElementTextIsEqual(areaListName, nameElement);
        }

        public void CopyAreaList()
        {
            // Click copy
            _driver.FindElements(By.Id("copy-area-list")).First().Click();
            _waitFor.CopyAreaListPopupToLoad();

            // Copy list
            var areaListName = _driver.FindElement(By.Id("info-box-text-input"))
                .GetAttribute("value");
            _driver.FindElement(By.ClassName("ok")).Click();

            // Wait for page to refresh
            Thread.Sleep(1000);
            _waitFor.PageToFinishLoading();
            _waitFor.AreaListPageTableToLoad();

            // Check list copy name
            var nameElement = GetNameElementOfFirstList();
            var count = 0;
            while (count < WaitFor.TimeoutLimitInSeconds &&
                   nameElement.Text.Equals(areaListName) == false)
            {
                // Need to wait until page has refreshed with new list
                Thread.Sleep(1000);
                count++;
                _waitFor.AreaListPageTableToLoad();
                nameElement = GetNameElementOfFirstList();
            }

            // Check list has been copied
            TestHelper.AssertElementTextIsEqual(areaListName, nameElement);
        }

        /// <summary>
        /// This is a recursive method calls itself
        /// to loop through the area lists and
        /// deletes them one by one
        /// </summary>
        public void DeleteAreaLists()
        {
            IWebElement firstAreaListElement;

            // Check whether at least one area list record is present
            // If not then exit this method
            try
            {
                firstAreaListElement = _driver.FindElements(By.Id("delete-area-list")).First();
            }
            catch (Exception)
            {
                // No area list to delete, exit this method
                return;
            }
            
            // No area list to delete,exit this method
            if (firstAreaListElement == null)
            {
                return;
            }

            // Delete the area list
            DeleteAreaList(firstAreaListElement);

            // Call this method recursively
            DeleteAreaLists();
        }

        private void DeleteAreaList(IWebElement areaListElement)
        {
            // Click delete
            areaListElement.Click();
            _waitFor.DeleteAreaListPopupToLoad();

            // Click on Ok button in the info box
            _driver.FindElement(By.ClassName("ok")).Click();

            // Wait for page to refresh
            Thread.Sleep(1000);
            _waitFor.PageToFinishLoading();
        }

        private IWebElement NavigateToYourDataMenu()
        {
            _navigateTo.HomePage();

            _waitFor.HomePageToLoad();

            var menu = _driver.FindElement(By.Id("your-account"));

            // Click menu to display options
            var accountButton = menu.FindElement(By.Id("your-account-button"));
            TestHelper.AssertElementTextIsEqual("Your data", accountButton);
            accountButton.Click();

            return menu;
        }

        public void NavigateToAreaListPage()
        {
            _driver.FindElement(By.Id("area-list-button")).Click();
            _waitFor.AreaListPageTableToLoad();
        }

        private ReadOnlyCollection<IWebElement> GetSearchedAreas()
        {
            return _driver.FindElements(By.ClassName("searched-areas"));
        }
        private ReadOnlyCollection<IWebElement> GetAreas()
        {
            return _driver.FindElements(By.ClassName("areas"));
        }

        private void SaveList()
        {
            _driver.FindElement(By.Id("btn-save-area-list")).Click();
        }

        private IWebElement GetNameElementOfFirstList()
        {
            IWebElement nameElement = null;

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
            row = _driver.FindElements(By.ClassName("webgrid-row-style"))
                .First();
            nameElement = row.FindElements(By.TagName("td")).First();
            return nameElement;
        }
    }
}
