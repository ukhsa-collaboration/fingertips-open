using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace Fpm.MainUISeleniumTest
{
    [TestClass]
    public class BaseUnitTest
    {
        private IWebDriver _driver;
        public WaitFor waitFor;
        public NavigateTo navigateTo;

        public BaseUnitTest()
        {
           InitDriverObjects();
        }

        protected void InitDriverObjects()
        {
            const string driverDirectory = @"C:\fingertips\3rdPartyLibraries\selenium";
            _driver = new ChromeDriver(driverDirectory);

            waitFor = new WaitFor(_driver);
            navigateTo = new NavigateTo(_driver);
        }

        [TestInitialize]
        public virtual void CalledOnceAtStartOfTests()
        {
            _driver.Manage().Window.Maximize();
        }

        [TestCleanup]
        public void CalledOnceWhenAllTestsHaveFinished()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        public IWebDriver Driver
        {
            get { return _driver; }
        }
    }
}
