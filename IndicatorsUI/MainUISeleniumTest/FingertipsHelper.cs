using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IndicatorsUI.MainUISeleniumTest
{
    public class FingertipsHelper
    {
        public static void SelectEachFingertipsTabInTurnAndCheckDownloadIsLast(IWebDriver driver)
        {
            var lastText = SelectEachFingertipsTabInTurn(driver);

            Assert.IsTrue(lastText.Contains("Download"),
                "Download should be last tab");
        }

        /// <summary>
        /// Selects each Fingertips tab in order from left to right.
        /// </summary>
        /// <returns>The HTML of the last selected tab.</returns>
        public static string SelectEachFingertipsTabInTurn(IWebDriver driver)
        {
            var waitFor = new WaitFor(driver);

            // Click through each domain
            var tabs = driver.FindElements(By.ClassName("page"));
            string lastText = string.Empty;
            foreach (var tab in tabs)
            {
                tab.Click();
                WaitFor.ThreadWait(0.1);
                waitFor.AjaxLockToBeUnlocked();

                // Check tab
                var text = tab.Text;
                Assert.AreNotEqual(lastText, text, "Tab clicked but was not selected");
                lastText = text;
            }

            return lastText;
        }

        public static void SelectAreaType(IWebDriver driver, int areaTypeId)
        {
            var areasDropdown = driver.FindElement(By.Id("areaTypes"));
            var selectElements = new SelectElement(areasDropdown);
            selectElements.SelectByValue(areaTypeId.ToString());
            WaitForAjaxLock(driver);
        }

        public static void SelectDomain(IWebDriver driver, int groupId)
        {
            ClickElement(driver, "domain" + groupId);
        }

        public static void SelectFingertipsTab(IWebDriver driver, string pageId)
        {
            ClickElement(driver, pageId);
        }

        public static string GetSelectedAreaNameFromMenu(IWebDriver driver)
        {
            var areasDropdown = driver.FindElement(By.Id(FingertipsIds.AreaMenu));
            var selectElements = new SelectElement(areasDropdown);
            var selectArea = selectElements.SelectedOption.Text;
            return selectArea;
        }

        public static void SearchForAnAreaAndSelectFirstResult(IWebDriver driver, string text)
        {
            var id = By.Id(FingertipsIds.AreaSearchText);
            var searchText = driver.FindElement(id);
            searchText.SendKeys(text);
            new WaitFor(driver).ExpectedElementToBeVisible(By.Id(PublicHealthDashboardIds.AreaSearchAutocompleteOptions));
            searchText.SendKeys(Keys.Return);
            WaitForAjaxLock(driver);
        }

        public static void SearchForPractice(IWebDriver driver, string text)
        {
            var id = By.Id(FingertipsIds.GpPracticeSearchText);
            var searchText = driver.FindElement(id);
            searchText.SendKeys(text);
            new WaitFor(driver).ExpectedElementToBeVisible(By.TagName(FingertipsIds.GpPracticeAutoComplete));
        }

        public static void LeaveAreaSearchMode(IWebDriver driver)
        {
            var searchLink = driver.FindElement(By.Id(FingertipsIds.AreaSearchLink));
            searchLink.Click();
            new WaitFor(driver).ExpectedElementToBeVisible(By.Id(FingertipsIds.AreaMenu));
        }

        public static void SwitchToAreaSearchMode(IWebDriver driver)
        {
            var searchLink = driver.FindElement(By.Id(FingertipsIds.AreaSearchLink));
            searchLink.Click();
            new WaitFor(driver).ExpectedElementToBeVisible(By.Id(FingertipsIds.AreaSearchText));
        }

        public static void SelectNextIndicator(IWebElement nextIndicatorButton, WaitFor waitFor)
        {
            nextIndicatorButton.Click();
            WaitFor.ThreadWait(0.1);
            waitFor.AjaxLockToBeUnlocked();
        }

        public static void SelectInequalityTrends(IWebDriver driver)
        {
            var trendsButton = driver.FindElement(By.Id(FingertipsIds.InequalitiesTrends));
            trendsButton.Click();
        }

        public static void SelectInequalitiesLatestValues(IWebDriver driver)
        {
            var trendsButton = driver.FindElement(By.Id(FingertipsIds.InequalitiesLatestValues));
            trendsButton.Click();
        }

        public static void SelectTrendsOnTartanRug(IWebDriver driver)
        {
            var trendsButton = driver.FindElement(By.Id("tab-option-1"));
            trendsButton.Click();
        }

        public static void ClickElement(IWebDriver driver, string pageId)
        {
            var tab = driver.FindElement(By.Id(pageId));
            tab.Click();
            WaitForAjaxLock(driver);
        }

        public static void checkElementDisplayed(IWebDriver driver, string elementId, bool shouldBeDisplayed, string errorMessage)
        {
            var element = FindElementById(driver, elementId);

            if (shouldBeDisplayed && !element.Displayed)
            {
                Assert.Fail(errorMessage);
            }

            if (!shouldBeDisplayed && element.Displayed)
            {
                Assert.Fail(errorMessage);
            }
        }

        public static void checkElementNumbersIsMinorOfMax(IWebDriver driver, string elementClassName, int maximunElement, string errorMessage = "The class is contained more times than specified")
        {
            var elementList = FindElementsByClass(driver, elementClassName);

            if (elementList.Count > maximunElement)
            {
                Assert.Fail(errorMessage);
            }
        }

        public static void checkTextIsNumberTimesById(IWebDriver driver, string elementId, string text, int timesNumber, string errorMessage = "The class is contained more times than specified")
        {
            var elementList = FindElementsTextById(driver, elementId, text).ToList();

            if (elementList.Count != timesNumber)
            {
                Assert.Fail(errorMessage);
            }
        }

        public static void checkTextIsNumberTimesbyTagName(IWebDriver driver,string tagName, string text, int timesNumber, string errorMessage = "The class is contained more times than specified")
        {
            var elementList = FindElementsTextByTagName(driver, tagName, text).ToList();

            if (elementList.Count != timesNumber)
            {
                Assert.Fail(errorMessage);
            }
        }

        public static IWebElement FindElementById(IWebDriver driver, string elementId)
        {
            var byId = getById(elementId);
            new WaitFor(driver).ExpectedElementToBePresent(byId);

            var element = driver.FindElement(byId);
            return element;
        }

        public static ReadOnlyCollection<IWebElement> FindElementsByClass(IWebDriver driver, string elementClassName)
        {
            var byClass = getByClass(elementClassName);
            new WaitFor(driver).ExpectedElementToBePresent(byClass);

            var element = driver.FindElements(byClass);
            return element;
        }

        public static IEnumerable<IWebElement> FindElementsTextByTagName(IWebDriver driver, string tagName, string text)
        {
            var byTagName = getByTagName(tagName);

            var element = driver.FindElements(byTagName).Where(x=> x.Text == text);
            return element;
        }

        public static IEnumerable<IWebElement> FindElementsTextById(IWebDriver driver, string elementId, string text)
        {
            var byId = getById(elementId);

            var element = driver.FindElements(byId).Where(x => x.Text == text);
            return element;
        }


        private static void WaitForAjaxLock(IWebDriver driver)
        {
            WaitFor.ThreadWait(0.1);
            new WaitFor(driver).AjaxLockToBeUnlocked();
        }

        private static By getById(string elementId)
        {
            return By.Id(elementId);
        }

        private static By getByClass(string elementClassName)
        {
            return By.ClassName(elementClassName);
        }

        private static By getByTagName(string tagName)
        {
            return By.TagName(tagName);
        }

        //***************************************************************
        // Helpers about csv files
        //***************************************************************
        public static bool isFileDownloaded(string path = null)
        {
            var downloadsPath = string.Empty;

            if (path == null)
            {
                downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
                Console.WriteLine("The downloaded path is:" + downloadsPath);
            }
            else
            {
                downloadsPath = path;
            }

            var d = new DirectoryInfo(downloadsPath);
            var files = d.GetFiles("*.csv");

            return files.Length == 1;

        }

        public static bool BackupCsvFiles(string sourcePath = null, string targetPath = null)
        {
            string backupDir;
            var sourceDir = SetPathOrDefault(sourcePath, targetPath, out backupDir);

            return copyCsvFiles(sourceDir, backupDir);
        }

        public static bool RestoreCsvFiles(string tempBackUpPath = null, string originPath = null)
        {
            string backupDir;
            var sourceDir = SetPathOrDefault(originPath, tempBackUpPath, out backupDir);

            return copyCsvFiles(backupDir, sourceDir);
        }

        public static bool DeleteCsvFiles(string originPath = null)
        {
            string backupDir;
            var sourceDir = SetPathOrDefault(originPath, null, out backupDir);

            return eraseCsvFiles(sourceDir);
        }

        private static string SetPathOrDefault(string sourcePath, string targetPath, out string backupDir)
        {
            var sourceDir = string.Empty;
            backupDir = string.Empty;

            // Default Download folder
            if (sourcePath == null)
            {
                sourceDir = KnownFolders.GetPath(KnownFolder.Downloads);
            }

            // Default Download/previousCsvTemp folder
            if (targetPath == null)
            {
                backupDir = KnownFolders.GetPath(KnownFolder.Downloads) + "\\previousCsvTemp";
            }

            return sourceDir;
        }

        private static bool copyCsvFiles(string sourceDir, string backupDir)
        {
            var copied = false;
            try
            {
                // Create a temporal directory if doesn't exist
                Directory.CreateDirectory(backupDir);
                
                // Get all files from source
                var files = Directory.GetFiles(sourceDir);

                foreach (var f in files)
                {
                    var fileName = f.Substring(sourceDir.Length + 1);

                    if (Path.GetExtension(f).Equals(".csv"))
                    {
                        // Copy backup of file
                        File.Copy(f, Path.Combine(backupDir, fileName), true);
                        copied = true;

                        // Delete file
                        File.Delete(f);
                    }
                }
            }
            catch (Exception e)
            {
                throw new IOException(e.Message);
            }

            return copied;
        }

        private static bool eraseCsvFiles(string sourceDir)
        {
            var deleted = false;
            try
            {
                var files = Directory.GetFiles(sourceDir);

                foreach (var f in files)
                {
                    if (Path.GetExtension(f).Equals(".csv"))
                    {
                        // Delete file
                        File.Delete(f);
                        deleted = true;
                    }
                }
            }
            catch (Exception e)
            {
                throw new IOException(e.Message);
            }

            return deleted;
        }
    }
}