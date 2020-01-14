using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUI.Controllers
{
    public class TwitterController : Controller
    {
        [Route("tweets/{twitterHandle}")]
        [OutputCache(Duration = 300/*300 = 5 minutes, 60s x 5m */)]
        public ActionResult Tweets(string twitterHandle)
        {
            //NOTE: need to update twitter credentials in UI_Twitter before we can pull tweets again
//            if (twitterHandle != "")
//            {
//                TwitterAccountSetting twitterAccount = ReaderFactory.GetTwitterSettingReadr().GetSettings(twitterHandle);
//                return new JsonpResult
//                {
//                    Data = new TwitterClient(twitterAccount).GetTweets(),
//                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
//                };
//            }

            // In case we don't get anything due to callback failure
            // just return empty tweet list json object.
            return new JsonpResult
            {
                Data = new List<Tweet>(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
