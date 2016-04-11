using System;
using System.Text.RegularExpressions;

namespace Profiles.MainUI.Helpers
{
    public class ContentHelper
    {
        public static string RemoveHtmlTags(string html)
        {
            return Regex.Replace(html, "<.*?>", string.Empty);
        }
    }
}