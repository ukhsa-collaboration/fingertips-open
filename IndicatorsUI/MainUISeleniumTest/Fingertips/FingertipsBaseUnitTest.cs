using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Profiles.MainUI.Skins;
using System.Collections.Generic;
using System.Linq;

namespace IndicatorsUI.MainUISeleniumTest.Fingertips
{
    [TestClass]
    public class FingertipsBaseUnitTest : BaseUnitTest
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            SetSkin(SkinNames.Core);
        }

        public static void CheckTartanRugHasLoaded(IWebDriver driver)
        {
            IList<IWebElement> firstRow = driver.FindElements(By.Id(LongerLivesIds.TartanRugIndicatorNameOnFirstRow));
            Assert.IsTrue(firstRow.Any());
        }
    }
}
