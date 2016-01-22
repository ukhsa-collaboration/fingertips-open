using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace MainUISeleniumTest
{
    [TestClass]
    public class BaseUnitTest
    {
        private IWebDriver _driver;

        public BaseUnitTest()
        {
            _driver = new ChromeDriver();
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
