using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUI.Models
{
    public static class TitleFont
    {

        public static string GetSize(string s, int profileId)
        {
            if (profileId == ProfileIds.PracticeProfiles)
            {
                return string.Empty;
            }

            var length = s.Length;
            if (length > 25)
            {
                return "font-size:" + Math.Round(95.0 / length, 1) + "em !important;";
            }

            return string.Empty;
        }
    }
}