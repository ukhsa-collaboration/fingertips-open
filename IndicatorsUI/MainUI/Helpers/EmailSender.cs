using IndicatorsUI.MainUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Notify.Client;
using Notify.Models.Responses;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.UserAccess;

namespace IndicatorsUI.MainUI.Helpers
{
    public interface IEmailSender
    {
        void SendVerificationEmail(string emailAddress, string firstName, Guid tempGuid);
        void SendResetPasswordEmail(ApplicationUser registeredUser, string token);
    }

    /// <summary>
    /// User GOV.UK Notify to send emails.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        public string _domainUrl = AppConfig.Instance.DomainUrl;
        public string _apiKey = AppConfig.Instance.NotifyApikey;

        public void SendVerificationEmail(string emailAddress, string firstName, Guid tempGuid)
        {
            var notifyClient = GetNotificationClient();

            var url = string.Format("{0}/user-account/verify-email-address/{1}",
                _domainUrl, tempGuid);
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
            {
                { "FirstName", firstName },
                { "Links", url }
            };

            EmailNotificationResponse response = notifyClient.SendEmail(emailAddress,
               NotifyEmailTemplates.VerifyEmailAddress, personalisation);
        }

        public void SendResetPasswordEmail(ApplicationUser registeredUser,string token)
        {
            var notifyClient = GetNotificationClient();

            var url = string.Format("{0}/user-account/reset-password/{1}",
                _domainUrl, registeredUser.TempGuid);
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
            {
                { "FirstName", registeredUser.FirstName },
                { "Links", url }
            };

            EmailNotificationResponse response = notifyClient.SendEmail(registeredUser.UserName,
                NotifyEmailTemplates.ResetPassword, personalisation);
        }

        private NotificationClient GetNotificationClient()
        {
            if (_apiKey == null)
                throw new Exception("Notify API key is null");
            return new NotificationClient(_apiKey);
        }
    }
}