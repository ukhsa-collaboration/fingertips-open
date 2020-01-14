using System;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.MainUI.Helpers;
using IndicatorsUI.UserAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class NotifyMessageHelperTest
    {
        private ApplicationUser _user = GetUser();

        [TestMethod]
        public void Test_GenerateNotifyTemplateParameters_ResetPassword()
        {
            var json = NotifyMessageHelper.GenerateNotifyTemplateParameters(_user, EmailNotificationType.ResetPassword);
            Assert.IsTrue(json.Contains("/user-account/reset-password"));
            AssertFirstNameIncluded(json);
        }

        [TestMethod]
        public void Test_GenerateNotifyTemplateParameters_VerifyEmailAddress()
        {
            var json = NotifyMessageHelper.GenerateNotifyTemplateParameters(_user, EmailNotificationType.VerifyEmailAddress);
            Assert.IsTrue(json.Contains("/user-account/verify-email-address"));
            AssertFirstNameIncluded(json);
        }

        private static void AssertFirstNameIncluded(string json)
        {
            Assert.IsTrue(json.Contains("\"FirstName\":\"a\""));
        }

        private static ApplicationUser GetUser()
        {
            return new ApplicationUser
            {
                FirstName = "a",
                TempGuid = Guid.Empty
            };
        }
    }
}
