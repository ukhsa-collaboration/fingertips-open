using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Profiles.MainUI.Skins;
using System.Collections.Generic;
using System.Linq;

namespace IndicatorsUI.MainUISeleniumTest
{
    [TestClass]
    public class BaseUnitTest
    {
        protected List<IWebDriver> drivers;
        public IWebDriver driver;
        public WaitFor waitFor;
        public NavigateTo navigateTo;

        public BaseUnitTest()
        {
            drivers = SeleniumHelper.Drivers();
        }

        [TestInitialize]
        public virtual void CalledOnceAtStartOfTests()
        {
            SetSkin(SkinNames.Mortality);
        }

        protected void InitDriverObjects()
        {
            driver = FirstDriver;
            waitFor = new WaitFor(driver);
            navigateTo = new NavigateTo(driver);
        }

        protected void SetSkin(string skinName)
        {
            SeleniumHelper.SetSkin(skinName);
            drivers.ForEach(x => x.Manage().Window.Maximize());

            InitDriverObjects();
        }

        [TestCleanup]
        public void CalledOnceWhenAllTestsHaveFinished()
        {
            SeleniumHelper.DisposeDrivers(drivers);
        }

        private IWebDriver FirstDriver
        {
            get { return drivers.First(); }
        }
    }
}
