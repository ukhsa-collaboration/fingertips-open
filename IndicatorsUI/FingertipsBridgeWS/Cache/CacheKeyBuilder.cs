using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace FingertipsBridgeWS.Cache
{
    public class CacheKeyBuilder
    {
        /// <summary>
        /// Identifies the service which determines the data of the response.
        /// </summary>
        public const string ParameterService = "s";

        /// <summary>
        /// _ is used by JQuery to identify each individual AJAX request made by the browser.
        /// </summary>
        public const string ParameterAjaxCallId = "_";

        /// <summary>
        /// Parameter is used by JQuery to allow json to be encoded as jsonp.
        /// </summary>
        public const string ParameterJsonp = "callback";

        /// <summary>
        /// Parameter used to indicate the json response of a call should not be cached.
        /// </summary>
        public const string ParameterNoCache = "nocache";

        public string JsonPValue { get; set; }
        public string ServiceId { get; set; }

        public bool CanJsonBeCached = true;
        public bool IsJsonP;

        private NameValueCollection parameters;

        private StringBuilder sb = new StringBuilder();

        public CacheKeyBuilder(NameValueCollection parameters, string serviceId)
        {
            this.parameters = parameters;

            ServiceId = serviceId ?? ParseString(ParameterService);

            List<string> keysToUse = new List<string>();
            foreach (string key in parameters.AllKeys)
            {
                switch (key)
                {
                    case ParameterService:
                    case ParameterAjaxCallId:
                        // Ignore these parameters
                        break;
                    case ParameterNoCache:
                        CanJsonBeCached = false;
                        break;
                    case ParameterJsonp:
                        IsJsonP = true;
                        JsonPValue = parameters[ParameterJsonp];
                        break;
                    default:
                        keysToUse.Add(key);
                        break;
                }
            }

            SetKey(parameters, keysToUse);
        }

        private void SetKey(NameValueCollection parameters, List<string> keys)
        {
            keys.Sort();
            foreach (string name in keys)
            {
                sb.Append(name);
                sb.Append(parameters[name]);
            }
        }

        /// <summary>
        /// varchar(MAX)
        /// </summary>
        public string CacheKey
        {
            get
            {
                return sb.ToString();
            }
        }

        protected string ParseString(string name)
        {
            string label = parameters[name];
            if (string.IsNullOrEmpty(label) == false)
            {
                return label;
            }
            return string.Empty;
        }

    }
}
