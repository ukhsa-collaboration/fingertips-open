using System;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;
using IndicatorsUI.DomainObjects.EmailTemplates;
using IndicatorsUI.UserAccess;
using Newtonsoft.Json;

namespace IndicatorsUI.MainUI.Helpers
{
    public class NotifyMessageHelper
    {
        public static string GenerateNotifyTemplateParameters(ApplicationUser user, string emailNotificationType)
        {
            var firstName = user.FirstName;
            string baseUrl = string.Format("{0}/user-account/", AppConfig.Instance.DomainUrl);
            string url;

            switch (emailNotificationType)
            {
                case EmailNotificationType.VerifyEmailAddress:

                    url = string.Format("{0}verify-email-address/{1}", baseUrl, user.TempGuid);

                    VerificationEmailParameters verificationEmailParameters = new VerificationEmailParameters()
                    {
                        FirstName = firstName,
                        Links = url
                    };

                    return JsonConvert.SerializeObject(verificationEmailParameters);

                case EmailNotificationType.ResetPassword:

                    url = string.Format("{0}reset-password/{1}", baseUrl, user.TempGuid);

                    ResetPasswordParameters resetPasswordParameters = new ResetPasswordParameters()
                    {
                        FirstName = firstName,
                        Links = url
                    };

                    return JsonConvert.SerializeObject(resetPasswordParameters);

                default:
                    throw new Exception("Email notification type is not valid.");
            }
        }
    }
}