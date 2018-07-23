using System;
using System.Text.RegularExpressions;

namespace IndicatorsUI.MainUI.Helpers
{
    public class ContentHelper
    {
        public static string RemoveHtmlTags(string html)
        {
            return Regex.Replace(html, "<.*?>", string.Empty);
        }
    }
}