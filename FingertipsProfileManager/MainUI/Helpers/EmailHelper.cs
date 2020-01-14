using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Email;
using Fpm.ProfileData.Entities.User;
using Fpm.ProfileData.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fpm.MainUI.Helpers
{
    public class EmailHelper
    {
        public const string NewLine = "%0A";

        private readonly IEmailRepository _emailRepository;
        private readonly string _notifyEmailTemplate;
        private readonly int _indicatorId;
        private readonly string _indicatorName;
        private readonly string _indicatorUrl;

        public EmailHelper() { }

        public EmailHelper(IEmailRepository emailRepository, string notifyEmailTemplate, int indicatorId,
            string indicatorName, string indicatorUrl)
        {
            _emailRepository = emailRepository;
            _notifyEmailTemplate = notifyEmailTemplate;
            _indicatorId = indicatorId;
            _indicatorName = indicatorName;
            _indicatorUrl = indicatorUrl;
        }

        public static string GetMessageBody(params string[] lines)
        {
            return string.Join(NewLine, lines.Select(Uri.EscapeDataString));
        }

        public bool SendEmail()
        {
            try
            {
                Email email = new Email()
                {
                    To = GetRecipientEmailAddress(),
                    TemplateId = _notifyEmailTemplate,
                    TemplateParameters = GenerateNotifyTemplateParameters(),
                    CreatedTimestamp = DateTime.Now
                };

                _emailRepository.CreateEmail(email);
                return true;
            }

            catch (Exception exception)
            {
                ExceptionLogger.LogException(exception, "Global.asax");
            }

            return false;
        }

        private string GetRecipientEmailAddress()
        {
            switch (_notifyEmailTemplate)
            {
                case NotifyEmailTemplates.IndicatorCreated:
                    return UserDetails.CurrentUser().FpmUser.EmailAddress;

                case NotifyEmailTemplates.IndicatorSubmittedForReview:
                case NotifyEmailTemplates.IndicatorApprovedNotificationToIMRG:
                case NotifyEmailTemplates.IndicatorSubmittedForReviewAfterRevision:
                    return AppConfig.ImrgEnquiriesEmailAddress();

                case NotifyEmailTemplates.IndicatorAwaitingRevision:
                case NotifyEmailTemplates.IndicatorApprovedNotificationToCreatedUser:
                    var user = GetDetailsOfUserWhoCreatedTheIndicator();
                    return user == null ? string.Empty : user.EmailAddress;

                default:
                    throw new Exception("Invalid notify email template option");
            }
        }

        private string GenerateNotifyTemplateParameters()
        {
            switch (_notifyEmailTemplate)
            {
                case NotifyEmailTemplates.IndicatorCreated:
                    return GetNotifyTemplateParametersForIndicatorCreated();

                case NotifyEmailTemplates.IndicatorSubmittedForReview:
                case NotifyEmailTemplates.IndicatorSubmittedForReviewAfterRevision:
                    return GetNotifyTemplateParametersForIndicatorSubmittedForReview();

                case NotifyEmailTemplates.IndicatorApprovedNotificationToCreatedUser:
                case NotifyEmailTemplates.IndicatorApprovedNotificationToIMRG:
                    return GetNotifyTemplateParametersForIndicatorApproved();

                case NotifyEmailTemplates.IndicatorAwaitingRevision:
                    return GetNotifyTemplateParametersForIndicatorAwaitingRevision();

                default:
                    throw new Exception("Invalid notify email template option");
            }
        }

        private string GetNotifyTemplateParametersForIndicatorCreated()
        {
            IndicatorCreated indicatorCreated = new IndicatorCreated()
            {
                IndicatorId = _indicatorId,
                IndicatorName = _indicatorName,
                IndicatorLink = _indicatorUrl,
                PersonCreated = UserDetails.CurrentUser().FpmUser.DisplayName
            };

            return JsonConvert.SerializeObject(indicatorCreated);
        }

        private string GetNotifyTemplateParametersForIndicatorSubmittedForReview()
        {
            IndicatorSubmittedForReview indicatorSubmittedForReview = new IndicatorSubmittedForReview()
            {
                IndicatorId = _indicatorId,
                IndicatorName = _indicatorName,
                IndicatorLink = _indicatorUrl,
                PersonSubmittedForReview = UserDetails.CurrentUser().FpmUser.DisplayName
            };

            return JsonConvert.SerializeObject(indicatorSubmittedForReview);
        }

        private string GetNotifyTemplateParametersForIndicatorApproved()
        {
            IndicatorApproved indicatorApproved = new IndicatorApproved()
            {
                IndicatorId = _indicatorId,
                IndicatorName = _indicatorName,
                IndicatorLink = _indicatorUrl,
                PersonCreated = GetDetailsOfUserWhoCreatedTheIndicator().DisplayName,
                PersonApproved = UserDetails.CurrentUser().FpmUser.DisplayName
            };

            return JsonConvert.SerializeObject(indicatorApproved);
        }

        private string GetNotifyTemplateParametersForIndicatorAwaitingRevision()
        {
            IndicatorAwaitingRevision indicatorAwaitingRevision = new IndicatorAwaitingRevision()
            {
                IndicatorId = _indicatorId,
                IndicatorName = _indicatorName,
                IndicatorLink = _indicatorUrl,
                PersonCreated = GetDetailsOfUserWhoCreatedTheIndicator().DisplayName,
                PersonMovedIndicatorToAwaitingRevision = UserDetails.CurrentUser().FpmUser.DisplayName
            };

            return JsonConvert.SerializeObject(indicatorAwaitingRevision);
        }

        private FpmUser GetDetailsOfUserWhoCreatedTheIndicator()
        {
            var indicatorAudit = CommonUtilities.GetIndicatorAudit(new List<int>() { _indicatorId });
            var indicatorCreatedAudit = indicatorAudit.FirstOrDefault(x => x.AuditType.ToLower() == "create");

            var fpmUsers = CommonUtilities.GetAllFpmUsers();
            var user = fpmUsers.FirstOrDefault(x => x.UserName.ToLower() == indicatorCreatedAudit.User);

            if (user == null)
            {
                return new FpmUser();
            }

            return user;
        }
    }
}