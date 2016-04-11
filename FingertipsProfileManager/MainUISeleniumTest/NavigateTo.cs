using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Fpm.MainUI.Helpers;

namespace Fpm.MainUISeleniumTest
{
    public class NavigateTo
    {
        public static string BaseUrl = ApplicationConfiguration.SiteUrlForTesting;

        private IWebDriver driver;

        public NavigateTo(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void ProfilesAndIndicatorsPage()
        {
            GoToUrl(string.Empty);
        }

        public void ContentIndexPage()
        {
            GoToUrl("content");
        }

        public void UserIndexPage()
        {
            GoToUrl("user");
        }

        public void GoToUrl(string relativeUrl)
        {
            driver.Navigate().GoToUrl(BaseUrl + relativeUrl);
        }
    }
}
