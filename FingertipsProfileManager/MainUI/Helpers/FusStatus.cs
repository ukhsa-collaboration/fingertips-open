using System;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;

namespace Fpm.MainUI.Helpers
{
    public class FusStatus
    {
        public static string Message()
        {
            var dateLastChecked = new LoggingRepository()
                .GetDatabaseLog(DatabaseLogIds.FusCheckedJobs)
                .Timestamp;

            var minutesSinceLastCheck = Math.Round((DateTime.Now - dateLastChecked).TotalMinutes, 0);
            if (minutesSinceLastCheck <= 1)
            {
                return "Upload service is running";
            }

            return string.Format("Upload jobs last checked {0} minutes ago", minutesSinceLastCheck);
        }
    }
}