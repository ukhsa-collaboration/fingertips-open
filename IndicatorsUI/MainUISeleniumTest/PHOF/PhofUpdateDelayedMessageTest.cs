﻿using IndicatorsUI.MainUISeleniumTest.Fingertips;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUISeleniumTest.Phof
{
    [TestClass]
    public class PhofUpdateDelayedMessageTest : FingertipsBaseUnitTest
    {
        const string Key = "ShowUpdateDelayedMessage";

        [TestMethod]
        public void TestHoldingMessageCanBeDisplayed()
        {           
            SeleniumHelper.SetValueInWebConfig(Key, "true");
            LoadPhofPage();
            Assert.IsTrue(IsUpdateDelayedPresent());
        }

        [TestMethod]
        public void TestHoldingMessageDisabled()
        {            
            SeleniumHelper.SetValueInWebConfig(Key, "false");           
            LoadPhofPage();
            Assert.IsFalse(IsUpdateDelayedPresent());            
        }

        private void LoadPhofPage()
        {
            navigateTo.FingertipsFrontPageForProfile(ProfileUrlKeys.Phof);
            waitFor.PhofDomainsToLoad();
        }

        private bool IsUpdateDelayedPresent()
        {
            try
            {
                return driver.FindElement(By.Id("update-delayed")).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}