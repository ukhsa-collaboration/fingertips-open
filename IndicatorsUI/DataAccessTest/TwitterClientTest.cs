using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace DataAccessTest
{
    [TestClass]
    public class TwitterClientTest
    {
        [TestMethod]
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