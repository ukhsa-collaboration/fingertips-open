using System;
using System.Linq;
using IndicatorsUI.DomainObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsContentTest : FingertipsBaseUnitTest
    {
        [TestMethod]
        public void Test_Single_Profile_Front_Page_Can_Be_Viewed()
        {
            navigateTo.GoToUrl("/profile/" + ProfileUrlKeys.HealthProfiles);
            var introduction = By.Id("introduction");
            waitFor.ExpectedElementToBeVisible(introduction);

            // Assert: Area search is on front page
            var headings = driver.FindElements(By.TagName("h2"));
            TestHelper.AssertElementTextIsEqual("Find your Health Profile", headings.First());
        }

        [TestMethod]
        public void Test_Profile_Group_Front_Page_Can_Be_Viewed()
        {
            navigateTo.GoToUrl("/profile-group/" + ProfileUrlKeys.MentalHealth);
            var introduction = By.Id("profile-collection-with-front-page");
            waitFor.ExpectedElementToBeVisible(introduction);
        }

        [TestMethod]
        public void Test_Supporting_Information_Page_Can_Be_Viewed()
        {
            navigateTo.GoToUrl("/profile/" + ProfileUrlKeys.HealthProfiles + "/supporting-information/faqs");
            waitFor.ExpectedElementToBeVisible(By.ClassName("supporting-information"));
        }
    }
}
