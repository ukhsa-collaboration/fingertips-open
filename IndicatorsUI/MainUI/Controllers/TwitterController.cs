using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace Profiles.MainUI.Controllers
{
    public class TwitterController : Controller
    {
        [OutputCache(Duration = 300/*300 = 5 minutes, 60s x 5m */)]
        public ActionResult Tweets(string twitterHandle)
        {
            if (twitterHandle != "")
            {
                TwitterAccountSetting twitterAccount = null;
                twitterAccount = ReaderFactory.GetTwitterSettingReadr().GetSettings(twitterHandle);
                return new JsonpResult
                {
                    Data = new TwitterClient(twitterAccount).GetTweets(),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {    // In case we don't get anything due to callback failure
                 // just return empty tweet list json object.
                return new JsonpResult
                {
                    Data =   new List<Tweet>(),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }                                          
        }
    }
}
