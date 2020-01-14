using Fpm.MainUI.Helpers;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class EmailHelperTest
    {
        private EmailRepository _emailRepository;
        private const int IndicatorId = IndicatorIds.BackPainPrevalence;
        private const string IndicatorName = "Back pain prevalence in people of all ages";

        private const string IndicatorLink =
            "http://localhost:52501/profile/indicators/specific?ProfileKey=indicators-for-review&DomainSequence=1&selectedAreaTypeId=118";

        [TestInitialize]
        public void TestInitialize()
        {
            _emailRepository = new EmailRepository(NHibernateSessionFactory.GetSession());
        }

        [TestMethod]
        public void TestSendEmailSucceeds()
        {
            // Assign
            EmailHelper emailHelper = new EmailHelper(_emailRepository, NotifyEmailTemplates.IndicatorCreated, IndicatorId, IndicatorName, IndicatorLink);

            // Act
            var status = emailHelper.SendEmail();

            // Assert
            Assert.IsTrue(status);
        }

        [TestMethod]
        public void TestSendEmailFails()
        {
            // Assign
            EmailHelper emailHelper = new EmailHelper(_emailRepository, "InvalidNotificationTemplate", IndicatorId, IndicatorName, IndicatorLink);

            //Act
            var status = emailHelper.SendEmail();

            //Assert
            Assert.IsFalse(status);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _emailRepository.Dispose();
        }
    }
}
