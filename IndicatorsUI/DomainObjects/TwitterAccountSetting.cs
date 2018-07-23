using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndicatorsUI.DomainObjects
{
    public class TwitterAccountSetting
    {
        public string Handle { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
    }
}