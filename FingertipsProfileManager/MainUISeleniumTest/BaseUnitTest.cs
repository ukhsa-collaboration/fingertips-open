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

        public BaseUnitTest()
        {
            _driver = new FirefoxDriver();
            //new ChromeDriver(ConfigurationManager.AppSettings["ChromeDriver"]);
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
