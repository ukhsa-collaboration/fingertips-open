using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUISeleniumTest.Helpers;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsGpPracticeMapTest : FingertipsBaseUnitTest
    {
        private const string PracticeProfileUrlKey = ProfileUrlKeys.PracticeProfiles;

        [TestInitialize]
        public void TestInitialize()
        {
            OpenPracticeProfilesFrontPage();
        }

        [TestMethod]
        public void Test_Search_On_Practice_Profiles_Front_Page()
        {
            // Population tab can be navigated to from search results
            PopulationTabCanBeNavigatedToFromSearchResults();
        }

        [TestMethod]
        public void Test_Results_Displayed_Searching_For_Practice()
        {
            // Navigate to profile data page
            OpenPracticeProfilesDataPage();

            // Select map tab
            SelectMapTab();
            
            //Type camb in search box
            SearchForPractice("cambr");

            // Assert expected results are displayed in autocomplete box
            var resultsHtml = GetResultsHtml();
            TestHelper.AssertTextContains(resultsHtml, "Cambridge", "Cambridge should be displayed in dropdown");
            TestHelper.AssertTextContains(resultsHtml, "Cambridge Town", "Cambridge Town should be displayed in dropdown");
        }

        [TestMethod]
        public void Test_Practices_in_CCG_Results_Displayed_Clicking_On_ShowAllPracticesinCCG()
        {
            // Navigate to profile data page
            OpenPracticeProfilesDataPage();

            // Select map tab
            SelectMapTab();

            //Get selected ccg(region Menu) name
            IWebElement ccgName = driver.FindElement(By.Id("regionMenu"));
            SelectElement selectedValue = new SelectElement(ccgName);
            string ccgNameText = selectedValue.SelectedOption.Text;

            //Click on show all practices in CCG link
            var ccgLink = driver.FindElement(By.Id("all_ccg_practices"));
            ccgLink.Click();

            //Make sure the practice count info element text contains currently selected ccg name. TThis element also displys practice count but we are not checking it.
            By element = By.Id("practice-count-info");
            new WaitFor(driver).ExpectedElementToBePresent(element);
            var countInfoText = driver.FindElement(element).Text;

            TestHelper.AssertTextContains(countInfoText, ccgNameText, "CCG Name should be present in practice count text");
        }

        [TestMethod]
        public void Test_PopulationTab_Can_Be_Navigated_To_From_Search_Results()
        {
            // Navigate to profile data page
            OpenPracticeProfilesDataPage();

            // Select map tab
            SelectMapTab();

            // Population tab can be navigated to from search results
            PopulationTabCanBeNavigatedToFromSearchResults();
        }

        private void PopulationTabCanBeNavigatedToFromSearchResults()
        {
            // Type camb in search box and select first result in autocomplete box. 
            // This will display list of practices with select link
            SearchForPracticeAndSelectFirstResult("cambr");

            //select and click first select-practice link
            var bySelectPractice = By.Id("select-practice");
            waitFor.ExpectedElementToBeVisible(bySelectPractice);
            var selectLink = driver.FindElement(bySelectPractice);
            selectLink.Click();
            waitFor.FingertipsPopulationGraphToLoad();

            //Assert that clicking on the link directs to the page-population tab
            By element = By.Id("page-population");
            new WaitFor(driver).ExpectedElementToBePresent(element);
            var populationTab = driver.FindElement(element);
            Assert.IsNotNull(populationTab);
        }

        private void OpenPracticeProfilesFrontPage()
        {
            navigateTo.FingertipsFrontPageForProfile(PracticeProfileUrlKey);
        }

        private void OpenPracticeProfilesDataPage()
        {
            navigateTo.FingertipsDataForProfile(PracticeProfileUrlKey);
        }

        private void SelectMapTab()
        {
            fingertipsHelper.SelectTab(FingertipsIds.TabMap);
            waitFor.ExpectedElementToBePresent(By.TagName("ft-practice-search"));
        }

        private void SearchForPractice(string place)
        {
            fingertipsHelper.SearchForPractice(place);
        }

        private void SearchForPracticeAndSelectFirstResult(string place)
        {
            var id = By.Id(FingertipsIds.GpPracticeSearchText);
            waitFor.ExpectedElementToBeVisible(id);
            var searchText = driver.FindElement(id);
            searchText.SendKeys(place);
            waitFor.ExpectedElementToBeVisible(By.TagName(FingertipsIds.GpPracticeAutoComplete));
            searchText.SendKeys(Keys.Return);
        }

        private string GetResultsHtml()
        {
            var resultsHtml = driver.FindElement(By.TagName(FingertipsIds.GpPracticeAutoComplete)).Text;
            return resultsHtml;
        }

    }
}
