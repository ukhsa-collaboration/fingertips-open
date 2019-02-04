using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUI.Helpers
{
    public interface IMeasurementProtocolHttpClient
    {
        Task SendDataToGoogleAnalytics();
    }

    public class MeasurementProtocolDownloadFiles : MeasurementProtocolHttpClientBase
    {
        protected const int ClientId = GoogleAnalytics.CodeFromAnonymousClient;
        protected const string HitType = "event";
        private const string Category = "Download";
        private const string Action = "Document";
        private string _eventLabel;
        private string _userAgent;

        public MeasurementProtocolDownloadFiles() : base(ClientId, HitType)
        {
            tid = AppConfig.Instance.IsEnvironmentLive ? GoogleAnalytics.TrackingLiveId : GoogleAnalytics.TrackingDevelopmentId;
        }

        protected override async Task SendDataAsync()
        {
            if (client.DefaultRequestHeaders.UserAgent.Count == 0)
            {
                client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            }

            var values = new Dictionary<string, string>
            {
                { "v", version.ToString() },
                { "tid", tid },
                { "cid", cid.ToString() },
                { "t", t },
                { "ec", Category },
                { "ea", Action },
                { "el", _eventLabel }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(GoogleAnalytics.EventCollectionUrl, content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new DataServiceRequestException(response.Content.ReadAsStringAsync().Result);
            }
        }

        public void LogFileDownloadWithGoogleAnalytics(string userAgent, string eventLabel)
        {
            _eventLabel = eventLabel;
            _userAgent = userAgent;

            SendDataToGoogleAnalytics();
        }
    }

    public class MeasurementProtocolHttpClientBase : IMeasurementProtocolHttpClient
    {
        protected readonly int version = GoogleAnalytics.GoogleAnalyticsVersion;
        protected HttpClient client;
        protected string tid;
        protected int cid;
        protected string t;

        public MeasurementProtocolHttpClientBase(int ClientId, string HitType,
            string TrackingId = GoogleAnalytics.TrackingDevelopmentId, HttpClient httpClient = null)
        {
            client = httpClient ?? new HttpClient();
            tid = TrackingId;
            cid = ClientId;
            t = HitType;
        }

        public Task SendDataToGoogleAnalytics()
        {
            return Task.Run(async () =>
                    {
                        try
                        {
                            await SendDataAsync();
                        }
                        catch (Exception e)
                        {
                            ExceptionLogger.LogException(e, "Google Analytics sending data failed:");
                        }
                    });
        }

        protected virtual Task SendDataAsync()
        {
            // This exception is intentionally created to be overridden
            throw new NotImplementedException(); 
        }

        public HttpClient Client
        {
            set
            {
                client = value;
            }
        }
    }
}