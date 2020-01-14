using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using IndicatorsUI.MainUISeleniumTest.Helpers;

namespace IndicatorsUI.MainUISeleniumTest
{
    [TestClass]
    public class BaseUnitTest
    {
        public IWebDriver driver;
        public WaitFor waitFor;
        public NavigateTo navigateTo;
        public FingertipsHelper fingertipsHelper;

        /// <summary>
        /// Constructor that uses the default web driver
        /// </summary>
        public BaseUnitTest()
        {
            driver = SeleniumHelper.GetDriver();
        }

        [TestCleanup]
        public void TestCleanup_Base()
        {
            SeleniumHelper.DisposeDriver(driver);
            driver = null;
        }

        protected void SetSkin(string skinName)
        {
            SeleniumHelper.SetSkin(skinName);
            InitDriverObjects();
        }

        protected void InitDriverObjects()
        {
            driver.Manage().Window.Maximize();
            waitFor = new WaitFor(driver);
            navigateTo = new NavigateTo(driver);
            fingertipsHelper = new FingertipsHelper(driver);
        }
    }
}
