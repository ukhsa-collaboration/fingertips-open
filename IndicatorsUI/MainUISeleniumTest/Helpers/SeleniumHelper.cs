using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace IndicatorsUI.MainUISeleniumTest.Helpers
{
    public class SeleniumHelper
    {
        private const string driverDirectory = @"C:\fingertips\3rdPartyLibraries\selenium";

        /// <summary>
        /// Changes the skin in web.config.
        /// </summary>
        public static void SetSkin(string skin)
        {
            SetValueInWebConfig("SkinOverride", skin);
        }

        public static void SetValueInWebConfig(string propertyName, string val)
        {
            string path = ConfigurationManager.AppSettings["WebConfig"];
            var text = File.ReadAllText(path);
            text = new Regex(propertyName + "[^>]+")
                .Replace(text, propertyName + "\" value=\"" + val + "\"/");
            File.WriteAllText(path, text);
        }

        public static IWebDriver GetDriver()
        {
            /* new InternetExplorerDriver(new InternetExplorerOptions{IgnoreZoomLevel = true}), 
             *  We cannot test IE as we don't have admin rights on our machines to change Zoom Level */

            var driver = GetFirefoxDriver();
            return driver;
        }

        public static IWebDriver GetChromeDriver()
        {
            IWebDriver driver = new ChromeDriver(driverDirectory);
            return driver;
        }

        public static IWebDriver GetFirefoxDriver()
        {
            FirefoxOptions options = new FirefoxOptions();

            // Make sure CSV file are saved straight to disc
            // about:config in Firefox to see options
            options.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/octet-stream;image/png;image/gif;text/csv");
            options.SetPreference("browser.download.folderList", 2);
            options.SetPreference("browser.download.dir", DownloadFileHelper.DownloadFolder);

            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(driverDirectory);
            IWebDriver driver = new FirefoxDriver(service, options);
            return driver;
        }

        public static void DisposeDriver(IWebDriver driver)
        {
            driver.Quit();
            driver.Dispose();
        }

    }
}