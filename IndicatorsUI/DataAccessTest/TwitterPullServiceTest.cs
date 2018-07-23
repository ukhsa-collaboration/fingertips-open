using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.DataAccessTest
{
    [TestClass]
    public class TwitterPullServiceTest
    {
        [TestMethod]
        public void ListTweetsOnUserTimelineTest()
        {
            // We are using PHE_Uk twitter details
            const string consumerKey = "5uI0XQZNAVn5M3NSLNLxA";
            const string consumerSecret = "yF21fii3r8mJxwlUSAESFMFgo0gplwBsNCc5sf8UqPU";
            const string accessToken = "41822696-7JJXbB2oF6Xpc8IPIPh38QvCPU0Ol50UPiCtFcESK";
            const string accessTokenSecret = "xkSkeIlcRU1vadFaY4FSSDLeGbpXsb7rE5F4HHs70";
            const string handle = "PHE_uk";

            var twitterPuller = new TwitterPullService(consumerKey, consumerSecret, accessToken, accessTokenSecret,
                handle);
            var tweets = twitterPuller.ListTweetsOnUserTimeline();

            Assert.IsTrue(tweets.Count == 3);
        }
    }
}
