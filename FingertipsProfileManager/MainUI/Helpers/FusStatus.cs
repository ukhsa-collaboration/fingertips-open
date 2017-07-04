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

            var timeMessage = GetTimeText(minutesSinceLastCheck);
            return string.Format("Upload jobs last checked {0} ago", timeMessage);
        }

        private static string GetTimeText(double minutesSinceLastCheck)
        {
            // Minutes
            if (minutesSinceLastCheck < 120)
            {
                return minutesSinceLastCheck + " mins";
            }

            // Hours
            var hours = Math.Round(minutesSinceLastCheck/60, 0);
            if (hours < 48)
            {
                return hours + " hours";
            }

            // Days
            var days = Math.Round(hours / 24, 0);
            return days + " days";
        }
    }
}