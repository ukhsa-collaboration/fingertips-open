using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.DataAccess
{
    public class TwitterPullService
    {
        private readonly string _accessToken;
        private readonly string _accessTokenSecret;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _handle;

        public TwitterPullService(string consumerKey, string consumerSecret, 
            string accessToken, string accessTokenSecret, string twitterHandle)
        {
            _consumerKey = consumerKey;
            _accessToken = accessToken;
            _accessTokenSecret = accessTokenSecret;
            _consumerSecret = consumerSecret;
            _handle = twitterHandle;
        }

        public List<Tweet> ListTweetsOnUserTimeline(int noOfTweets = 3)
        {
            // oauth application keys
            string oauthToken = _accessToken;
            string oauthTokenSecret = _accessTokenSecret;
            string oauthConsumerKey = _consumerKey;
            string oauthConsumerSecret = _consumerSecret;

            // oauth implementation details
            const string oauthVersion = "1.0";
            const string oauthSignatureMethod = "HMAC-SHA1";

            // unique request details
            string oauthNonce = Convert.ToBase64String(
                new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            TimeSpan timeSpan = DateTime.UtcNow
                                - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string oauthTimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // message api details
            string resourceUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json";
            // create oauth signature
            string baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                                "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&screen_name={6}";

            string baseString = string.Format(baseFormat,
                oauthConsumerKey,
                oauthNonce,
                oauthSignatureMethod,
                oauthTimestamp,
                oauthToken,
                oauthVersion,
                Uri.EscapeDataString(_handle)
                );

            baseString = string.Concat("GET&", Uri.EscapeDataString(resourceUrl), "&", Uri.EscapeDataString(baseString));

            string compositeKey = string.Concat(Uri.EscapeDataString(oauthConsumerSecret),
                "&", Uri.EscapeDataString(oauthTokenSecret));

            string oauthSignature;
            using (var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
            {
                oauthSignature = Convert.ToBase64String(
                    hasher.ComputeHash(Encoding.ASCII.GetBytes(baseString)));
            }

            // create the request header
            string headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                                  "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                                  "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                                  "oauth_version=\"{6}\"";

            string authHeader = string.Format(headerFormat,
                Uri.EscapeDataString(oauthNonce),
                Uri.EscapeDataString(oauthSignatureMethod),
                Uri.EscapeDataString(oauthTimestamp),
                Uri.EscapeDataString(oauthConsumerKey),
                Uri.EscapeDataString(oauthToken),
                Uri.EscapeDataString(oauthSignature),
                Uri.EscapeDataString(oauthVersion)
                );

            ServicePointManager.Expect100Continue = false;

            string postBody = "screen_name=" + Uri.EscapeDataString(_handle);
            resourceUrl += "?" + postBody;
            var request = (HttpWebRequest)WebRequest.Create(resourceUrl);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Proxy = new WebProxy(AppConfig.Instance.WebProxy);

            var tweets = new List<Tweet>();
            try
            {
                WebResponse response = request.GetResponse();
                string responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                JArray jsonArray = JArray.Parse(responseData);
                if (jsonArray != null)
                {
                    for (int i = 0; i < noOfTweets; i++)
                    {
                        tweets.Add(new Tweet
                        {
                            Text = jsonArray[i]["text"].ToString(),
                            CreatedDate = TimeAgo(StringToDateTime(jsonArray[i]["created_at"].ToString()))
                        });
                    }
                }
            }
            catch (Exception error)
            {
                throw new FingertipsException("Twitter service failed to pull tweets", error);
            }

            return tweets;
        }

        private DateTime StringToDateTime(string dateAsString)
        {
            const string format = "ddd MMM dd HH:mm:ss zzzz yyyy";
            return DateTime.ParseExact(dateAsString, format, CultureInfo.InvariantCulture);
        }

        private string TimeAgo(DateTime dt)
        {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 60)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds
                        + " seconds ago";
            }
            if (delta < 120)
            {
                return "a minute ago";
            }
            if (delta < 2700) // 45 * 60
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 5400) // 90 * 60
            {
                return "an hour ago";
            }
            if (delta < 86400)
            { // 24 * 60 * 60
                return ts.Hours + " hours ago";
            }
            if (delta < 172800)
            { // 48 * 60 * 60
                return "yesterday";
            }
            if (delta < 2592000)
            { // 30 * 24 * 60 * 60
                return ts.Days + " days ago";
            }
            if (delta < 31104000)
            { // 12 * 30 * 24 * 60 * 60
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }
    }
}
