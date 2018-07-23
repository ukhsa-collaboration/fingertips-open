using System.Collections.Generic;
using Fpm.ProfileData.Entities.Job;

namespace Fpm.MainUI.Helpers
{
    public class UploadHelper
    {
        public static bool AnyDisallowedIndicators(List<string> indicatorMessages)
        {
            return indicatorMessages != null && indicatorMessages.Count > 0;
        }

        public static string GetTextFromStatusCodeForActiveJobs(UploadJobStatus statusCode)
        {
            var status = "Unknown";
            switch (statusCode)
            {
                case UploadJobStatus.NotStarted:
                case UploadJobStatus.OverrideDatabaseDuplicatesConfirmationGiven:
                case UploadJobStatus.SmallNumberWarningConfirmationGiven:
                    status = "In queue";
                    break;
                case UploadJobStatus.InProgress:
                    status = "In progress";
                    break;
            }
            return status;
        }

    }
}