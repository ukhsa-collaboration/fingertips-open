using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Profiles.DomainObjects;

namespace Profiles.MainUI.Models
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
                return "font-size:" + Math.Round(95.0 / length, 1) + "em";
            }

            return string.Empty;
        }
    }
}