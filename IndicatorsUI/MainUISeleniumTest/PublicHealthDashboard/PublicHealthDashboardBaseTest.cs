using IndicatorsUI.MainUI.Skins;
using IndicatorsUI.MainUISeleniumTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace IndicatorsUI.MainUISeleniumTest.PublicHealthDashboard
{
    [TestClass]
    public class PublicHealthDashboardBaseTest : BaseUnitTest
    {
        [TestInitialize]
        public void TestInitialize_PublicHealthDashboardBase()
        {
            SetSkin(SkinNames.Mortality);
        }
    }
}
