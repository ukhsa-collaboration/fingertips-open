using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.DataAccessTest
{
    [TestClass]
    public class TwitterClientTest
    {
        // Tweet not pulled anymore
        [TestMethod, TestCategory("ExcludeFromJenkins")]
        public void GetTweetsTest()
        {
            TwitterAccountSetting account = ReaderFactory.GetTwitterSettingReadr().GetSettings("PHoutcomes");

            var client = new TwitterClient(account);
            List<Tweet> tweetList = client.GetTweets();
            Assert.AreEqual(3, tweetList.Count);
            Assert.IsNotNull(tweetList[0].Text);
        }
    }
}