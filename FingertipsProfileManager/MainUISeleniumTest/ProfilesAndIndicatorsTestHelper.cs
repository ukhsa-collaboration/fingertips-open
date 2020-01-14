using Fpm.ProfileData;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace Fpm.MainUISeleniumTest
{
    class ProfilesAndIndicatorsTestHelper
    {
        private readonly IWebDriver _driver;
        private NavigateTo _navigateTo;
        private WaitFor _waitFor;

        public ProfilesAndIndicatorsTestHelper(IWebDriver driver, NavigateTo navigateTo, WaitFor waitFor)
        {
            this._driver = driver;
            this._navigateTo = navigateTo;
            this._waitFor = waitFor;
        }

        public void ClickElement(string elementId, int findElement)
        {
            IWebElement element;

            switch (findElement)
            {
                case FindElement.ByName:
                    element = _driver.FindElement(By.Name(elementId));
                    break;
                case FindElement.ByClassName:
                    element = _driver.FindElement(By.ClassName(elementId));
                    break;
                case FindElement.ByXPath:
                    element = _driver.FindElement(By.XPath(elementId));
                    break;
                default:
                    element = _driver.FindElement(By.Id(elementId));
                    break;
            }

            element.Click();
        }

        public void InputText(string elementId, string text)
        {
            var element = _driver.FindElement(By.Id(elementId));
            element.SendKeys(text);
        }

        public void NavigateToProfilesAndIndicatorsPage()
        {
            _navigateTo.ProfilesAndIndicatorsPage();
            _waitFor.ProfilesAndIndicatorsPageToLoad();

            SelectProfile(UrlKeys.DevelopmentProfileForTesting);
            SelectDomain(6);
            SelectAreaType(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts);
        }

        public void SelectProfile(string profileUrlKey)
        {
            var profileSelect = _driver.FindElement(By.Id("selectedProfile"));
            var selectElement = new SelectElement(profileSelect);
            selectElement.SelectByValue(profileUrlKey);

            _waitFor.ProfilesAndIndicatorsPageToLoad();
        }

        public void SelectDomain(int domainSequence)
        {
            var domainSelect = _driver.FindElement(By.Id("selectedDomain"));
            var selectElement = new SelectElement(domainSelect);
            selectElement.SelectByValue(domainSequence.ToString());

            _waitFor.ProfilesAndIndicatorsPageToLoad();
        }

        public void SelectAreaType(int areaTypeId)
        {
            var areaTypeSelect = _driver.FindElement(By.Id("SelectedAreaTypeId"));
            var selectElement = new SelectElement(areaTypeSelect);
            selectElement.SelectByValue(areaTypeId.ToString());

            _waitFor.ProfilesAndIndicatorsPageToLoad();
        }

        public void SelectIndicator(int indicatorId)
        {
            var tickBox = _driver.FindElements(By.Name(indicatorId + "_selected"));
            tickBox.First().Click();
        }

        public void ClickNewIndicatorButton()
        {
            ClickElement("//input[@value='New indicator']", FindElement.ByXPath);
            _waitFor.ExpectedElementToBePresent(By.XPath("//a[@id='ui-id-1']"));
        }

        public void FillInScreenInput()
        {
            // Tab 1
            _driver.FindElement(By.Id("IndicatorMetadataTextValue_Name")).SendKeys("Indicator test");

            _driver.FindElement(By.Id("IndicatorMetadataTextValue_Definition")).Click();
            SeleniumHelper.WaitForExpectedElement(_driver, By.XPath("//iframe[@id='metaDataText_ifr']"));
            _driver.SwitchTo().Frame("metaDataText_ifr");
            var editable = _driver.SwitchTo().ActiveElement();
            editable.SendKeys("text for test");
            _driver.SwitchTo().DefaultContent();
            _driver.FindElement(By.XPath("//input[@id='editor-done']")).Click();

            _driver.FindElement(By.Id("IndicatorMetadataTextValue_DataSource")).SendKeys("text for test");

            // Tab 2
            _driver.FindElement(By.XPath("//a[@id='ui-id-2']")).Click();
            new SelectElement(_driver.FindElement(By.Id("IndicatorMetadata_ValueTypeId"))).SelectByValue("7");
            new SelectElement(_driver.FindElement(By.Id("IndicatorMetadata_CIMethodId"))).SelectByValue("2");
            new SelectElement(_driver.FindElement(By.Id("IndicatorMetadata_UnitId"))).SelectByValue("56");
            new SelectElement(_driver.FindElement(By.Id("IndicatorMetadata_DenominatorTypeId"))).SelectByValue("24");
            new SelectElement(_driver.FindElement(By.Id("IndicatorMetadata_YearTypeId"))).SelectByValue("3");
            _driver.FindElement(By.Id("IndicatorMetadata_AlwaysShowSexWithIndicatorName")).Click();
            _driver.FindElement(By.Id("IndicatorMetadata_AlwaysShowAgeWithIndicatorName")).Click();

            // Tab 3
            _driver.FindElement(By.XPath("//a[@id='ui-id-3']")).Click();
            new SelectElement(_driver.FindElement(By.Id("UrlKey"))).SelectByValue("development-profile-for-testing");
            new SelectElement(_driver.FindElement(By.Id("Grouping_AreaTypeId"))).SelectByValue(AreaTypeIds.AcuteTrustsIncludingCombinedMentalHealthTrusts.ToString());
            new SelectElement(_driver.FindElement(By.Id("Grouping_SexId"))).SelectByValue("2");
            _driver.FindElement(By.Id("Grouping_AgeId_chosen")).Click();
            _driver.FindElement(By.XPath("/html/body/div[3]/div/div[3]/form/div[4]/div[6]/div/div/ul/li[3]")).Click();
            new SelectElement(_driver.FindElement(By.Id("Grouping_ComparatorMethodId"))).SelectByValue("17");
            new SelectElement(_driver.FindElement(By.Id("Grouping_PolarityId"))).SelectByValue("99");
            new SelectElement(_driver.FindElement(By.Id("Grouping_YearRange"))).SelectByValue("1");
            new SelectElement(_driver.FindElement(By.Id("Grouping_TimeSeries"))).SelectByValue("1");
            _driver.FindElement(By.Id("Grouping_BaselineYear")).SendKeys("2019");
            _driver.FindElement(By.Id("Grouping_DataPointYear")).SendKeys("2040");

            // Tab 4
            _driver.FindElement(By.XPath("//a[@id='ui-id-4']")).Click();
            _driver.FindElement(By.Id("IndicatorMetadataTextValue_Keywords")).SendKeys("text for test");
            _driver.FindElement(By.Id("IndicatorMetadataTextValue_EvidOfVariability")).SendKeys("text for test");
            _driver.FindElement(By.Id("IndicatorMetadataTextValue_JustifConfIntMeth")).SendKeys("text for test");
            _driver.FindElement(By.Id("IndicatorMetadataTextValue_QualityAssurance")).SendKeys("text for test");
            _driver.FindElement(By.Id("IndicatorMetadataTextValue_QualityImprPlan")).SendKeys("text for test");
            _driver.FindElement(By.Id("IndicatorMetadataTextValue_JustiOfExclusions")).SendKeys("text for test");
            _driver.FindElement(By.Id("IndicatorMetadataTextValue_JustifOfDataSources")).SendKeys("text for test");
            _driver.FindElement(By.Id("IndicatorMetadataTextValue_SponsorStakeholders")).SendKeys("text for test");
            _driver.FindElement(By.Id("IndicatorMetadataTextValue_IndOwnerContDet")).SendKeys("text for test");
            _driver.FindElement(By.Id("IndicatorMetadataTextValue_Comments")).SendKeys("text for test");

            // TODO: Selenium to work with Select2
            //_driver.FindElement(By.Id("PartitionAgeIds")).Click();
            //_driver.FindElement(By.Id("PartitionSexIds")).Click();
            //_driver.FindElement(By.Id("PartitionAreaTypeIds")).Click();
        }

        public void SaveIndicator()
        {
            ClickElement("add-indicator", FindElement.ById);
            _waitFor.ExpectedElementToBePresent(By.XPath("//div[@id='infoBox']"));

            ClickElement("confirm-create-indicator", FindElement.ById);
            _waitFor.InfoBoxPopupToLoad();
        }

        public int GetNewlyCreatedIndicatorId()
        {
            var newIndicatorIdText = _driver.FindElement(By.Id("new-indicator-id")).Text;

            ClickElement("indicator-created-confirmation", FindElement.ById);

            NavigateToProfilesAndIndicatorsPage();

            return int.Parse(newIndicatorIdText);
        }

    }
}
