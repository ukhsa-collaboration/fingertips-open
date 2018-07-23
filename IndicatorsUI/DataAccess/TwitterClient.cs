using System;
using System.Collections.Generic;
using System.Linq;
using IndicatorsUI.DomainObjects;


namespace IndicatorsUI.DataAccess
{
    public class TwitterClient
    {
        private TwitterAccountSetting account = new TwitterAccountSetting();

        public TwitterClient(TwitterAccountSetting accountSetting)
        {
            this.account = accountSetting;
        }

        public List<Tweet> GetTweets()
        {           
            var service = new TwitterPullService(account.ConsumerKey, account.ConsumerSecret, account.AccessToken,
                account.AccessTokenSecret, account.Handle);
            var tweetsToDisplay = service.ListTweetsOnUserTimeline();
            return tweetsToDisplay;
        }       
    }
}