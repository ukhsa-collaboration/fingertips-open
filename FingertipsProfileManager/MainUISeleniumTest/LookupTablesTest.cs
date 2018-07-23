using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Fpm.MainUISeleniumTest
{
    public enum FrameNames
    {
        Description = 0,
        Notes = 1
    }

    [TestClass]
    public class LookupTablesTest : BaseUnitTest
    {
        private IWebElement _categoryTypeNameTextBox;
        private IWebElement _categoryTypeDescriptionTextArea;
        private IWebElement _categoryTypeNotesTextArea;

        public string CategoryTypeName = Guid.NewGuid().ToString();
        public string CategoryTypeDescription = Guid.NewGuid().ToString();
        public string CategoryTypeNotes = Guid.NewGuid().ToString();

        public IWebElement CategoryTypeNameTextBox
        {
            get
            {
                SwitchToParentWindow();
                _categoryTypeNameTextBox = Driver.FindElement(By.Id("Name"));
                return _categoryTypeNameTextBox;
            }
            set
            {
                _categoryTypeNameTextBox = value;
            }
        }

        public IWebElement CategoryTypeDescriptionTextArea
        {
            get
            {
                SwithToIFrameWindow(FrameNames.Description);
                _categoryTypeDescriptionTextArea = Driver.FindElement(By.Id("tinymce"));
                return _categoryTypeDescriptionTextArea;
            }
            set
            {
                _categoryTypeDescriptionTextArea = value;
            }
        }

        public IWebElement CategoryTypeNotesTextArea
        {
            get
            {
                SwithToIFrameWindow(FrameNames.Notes);
                _categoryTypeNotesTextArea = Driver.FindElement(By.Id("tinymce"));
                return _categoryTypeNotesTextArea;
            }
            set
            {
                _categoryTypeNotesTextArea = value;
            }
        }

        public string WindowHandle { get; set; }

        [TestMethod]
        public void Test_Edit_Category_Type()
        {
            // Navigate to the categories page
            LoadCategoriesPage();

            // Get the handle of the parent window
            // The web page to be tested uses couple of tinymce controls which are iframes
            // Having the handle of is essential to swap to and fro to access the corresponding controls
            WindowHandle = Driver.CurrentWindowHandle;

            // Load a category to edit
            LoadEditCategoryPage();

            // Edit the category and save the details
            EditCategory();

            // Load the edited category
            LoadEditCategoryPage();

            // Verify whether the edit was successful
            CheckUpdatedValues();
        }

        private void LoadCategoriesPage()
        {
            // Navigate to the lookup tables page
            navigateTo.LookupTablesPage();

            // Wait for the lookup tables page to load
            waitFor.LookupTablesPageToLoad();

            // Navigate to the categories page
            navigateTo.CategoriesPage();

            // Wait for the categories page to load
            waitFor.CategoriesPageToLoad();
        }

        private void LoadEditCategoryPage()
        {
            // Switch to the parent window
            SwitchToParentWindow();
            
            // Find the edit button on the categories page
            var editButton = Driver.FindElement(By.Id("edit"));

            // Test whether edit button is visible
            Assert.IsNotNull(editButton);

            // Click on the edit button
            editButton.Click();

            // Wait for edit category page to load
            waitFor.EditCategoryPageToLoad();
        }

        private void EditCategory()
        {
            // Set the value of the category type name
            CategoryTypeNameTextBox.Clear();
            CategoryTypeNameTextBox.SendKeys(CategoryTypeName);

            // Set the value of the category type description
            CategoryTypeDescriptionTextArea.Clear();
            CategoryTypeDescriptionTextArea.SendKeys(CategoryTypeDescription);

            // Set the value of the category type notes
            CategoryTypeNotesTextArea.Clear();
            CategoryTypeNotesTextArea.SendKeys(CategoryTypeNotes);

            // Switch back to the parent window
            SwitchToParentWindow();

            // Find the save button
            var saveButton = Driver.FindElement(By.Id("save"));
            // Click on the save button
            saveButton.Click();

            // Wait for the categories page to load
            waitFor.CategoriesPageToLoad();
        }

        private void CheckUpdatedValues()
        {
            TestHelper.AssertElementTextIsEqual(CategoryTypeDescription, CategoryTypeDescriptionTextArea);
            TestHelper.AssertElementTextIsEqual(CategoryTypeNotes, CategoryTypeNotesTextArea);
            TestHelper.AssertElementTextIsEqual(CategoryTypeName, CategoryTypeNameTextBox.GetAttribute("value"));
        }

        private void SwitchToParentWindow()
        {
            // Switch to the parent window
            Driver.SwitchTo().Window(WindowHandle);
        }

        private void SwithToIFrameWindow(FrameNames frameName)
        {
            // Switch to the parent window first
            SwitchToParentWindow();

            // Switch to the iframe
            switch (frameName)
            {
                case FrameNames.Description:
                    Driver.SwitchTo().Frame(Driver.FindElement(By.Id("Description_ifr")));
                    break;
                case FrameNames.Notes:
                    Driver.SwitchTo().Frame(Driver.FindElement(By.Id("Notes_ifr")));
                    break;
            }

            // Wait for the iframe to load
            waitFor.IFrameToLoad();
        }
    }
}
