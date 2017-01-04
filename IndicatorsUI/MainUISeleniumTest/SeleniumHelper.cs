using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Chrome;

namespace IndicatorsUI.MainUISeleniumTest
{
    public class SeleniumHelper
    {
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

        public static List<IWebDriver> Drivers()
        {
            /* new InternetExplorerDriver(new InternetExplorerOptions{IgnoreZoomLevel = true}), 
             *  We cannot test IE as we don't have admin rights on our machines to change Zoom Level */
            const string driverDirectory = @"C:\fingertips\3rdPartyLibraries\selenium";
            IWebDriver driver = new ChromeDriver(driverDirectory);

            //            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(@"c:\Fingertips\3rdPartyLibraries\selenium");
            //            IWebDriver driver = new FirefoxDriver(service);
            return new List<IWebDriver> { driver };
        }

        public static void DisposeDrivers(List<IWebDriver> drivers)
        {
            drivers.ForEach(x => x.Quit());
            drivers.ForEach(x => x.Dispose());
        }
    }
}