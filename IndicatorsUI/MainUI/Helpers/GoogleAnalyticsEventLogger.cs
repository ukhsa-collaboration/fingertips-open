using System;
using System.Data.Services.Client;
using System.Threading;
using System.Threading.Tasks;
using System.Web.WebPages;
using Remotion.Linq.Utilities;
using Unity;

namespace IndicatorsUI.MainUI.Helpers
{
    public interface IGoogleAnalyticsEventLogger
    {
        /// <summary>
        /// Method to send data about downloaded files to Google Analytics
        /// </summary>
        void RegisterDocumentDownload(string fileName, string userAgent = "Not specified");
    }

    public class GoogleAnalyticsEventLogger : IGoogleAnalyticsEventLogger
    {
        /// <summary>
        /// This should only be called from DI set up
        /// </summary>
        public static IGoogleAnalyticsEventLogger Instance;

        public GoogleAnalyticsEventLogger(IMeasurementProtocolHttpClient client)
        {
            MeasurementProtocolHttpClient = client;
        }

        private IMeasurementProtocolHttpClient MeasurementProtocolHttpClient { get; set; }

        /// <summary>
        /// Method to send data about downloaded files to Google Analytics
        /// </summary>
        public void RegisterDocumentDownload(string fileName, string userAgent = "Not specified")
        {
            if (fileName.IsEmpty())
            {
                throw new ArgumentEmptyException("The downloaded file name is required");
            }

            if (fileName.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase) ||
                fileName.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) ||
                fileName.EndsWith(".git", StringComparison.InvariantCultureIgnoreCase)) return;

            var loggerDownloadFiles = MeasurementProtocolHttpClient as MeasurementProtocolDownloadFiles;

            if (loggerDownloadFiles == null)
            {
                //We are using the wrong class for this method
                throw new TypeAccessException();
            }

            loggerDownloadFiles.LogFileDownloadWithGoogleAnalytics(userAgent, fileName);
        }
    }
}