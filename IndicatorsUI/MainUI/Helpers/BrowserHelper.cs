using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Profiles.MainUI.Helpers
{
    public class BrowserHelper
    {
        public static bool IsIe8(HttpRequestBase request )
        {
            return IsIe(request) && request.Browser.MajorVersion == 8;
        }

        public static bool IsIe(HttpRequestBase request)
        {
            string browser = request.Browser.Browser.ToLower();

            if (string.IsNullOrEmpty(browser))
            {
                // Cannot identify browser
                return false;
            }

            return browser.Equals("ie") || browser.Equals("internetexplorer");
        }
    }
}