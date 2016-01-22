using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace MainUISeleniumTest
{
    public class WaitFor
    {
        private IWebDriver driver;

        public WaitFor(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void PageWithModalPopUpToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.ClassName("modal"));
        }

        public void EditUserPageToLoad()
        {
            SeleniumHelper.WaitForExpectedElement(driver,
                By.Id("Confirm"));
        }
    }
}
