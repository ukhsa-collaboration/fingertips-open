using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace Fpm.MainUISeleniumTest
{
    public class TestHelper
    {
        public static void AssertTextContains(string text, string contained)
        {
            AssertTextContains(text, contained, string.Empty);
        }

        public static void AssertTextContains(string text, string contained, string messageOnTestFailure)
        {
            Assert.IsNotNull(text, "Text is null but was expected to contain: " + contained);
            Assert.IsTrue(text.ToLower().Contains(contained.ToLower()), messageOnTestFailure);
        }

        public static void AssertTextDoesNotContain(string text, string notContained)
        {
            AssertTextDoesNotContain(text, notContained, string.Empty);
        }

        public static void AssertTextDoesNotContain(string text, string notContained, string messageOnTestFailure)
        {
            Assert.IsNotNull(text, "Text is null but was expected to not contain: " + notContained);
            Assert.IsFalse(text.ToLower().Contains(notContained.ToLower()), messageOnTestFailure);
        }

        public static void AssertElementTextIsEqual(string expectedText, IWebElement webElement)
        {
            Assert.AreEqual(expectedText, webElement.Text);
        }

        public static void AssertElementTextIsEqual(string expectedText, string webElementText)
        {
            Assert.AreEqual(expectedText, webElementText);
        }
    }
}