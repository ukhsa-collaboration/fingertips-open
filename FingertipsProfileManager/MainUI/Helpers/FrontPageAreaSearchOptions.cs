using Fpm.ProfileData;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Fpm.MainUI.Helpers
{
    public class FrontPageAreaSearchOptions
    {
        public const string NoSearchText = "NONE";

        public IList<SelectListItem> GetOptions(string selectedValue)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem {Text = NoSearchText, Value = null},

                // CCsGs
                new SelectListItem {Text = "CCGs (pre 4/2017)", Value = AreaTypeIds.CcgsPreApr2017.ToString()},
                new SelectListItem {Text = "CCGs (post 4/2017) [Place and postcode search not available]",
                    Value = AreaTypeIds.CcgsPostApr2017.ToString()},
                new SelectListItem {Text = "CCGs (since 4/2018) [Place and postcode search not available]",
                    Value = AreaTypeIds.CcgsSinceApr2018.ToString()},

                // Local authorities pre April 2019
                new SelectListItem {Text = "County/UA (pre 4/19)", Value = AreaTypeIds.CountyAndUnitaryAuthorityPre2019.ToString()},
                new SelectListItem
                {
                    Text = "County/UA/District (pre 4/19)",
                    Value = AreaTypeIds.CountyAndUnitaryAuthorityPre2019 + "," + AreaTypeIds.DistrictAndUnitaryAuthorityPre2019
                },

                // Local authorities from April 2019
                new SelectListItem {Text = "County/UA [Place and postcode search not available]", Value = AreaTypeIds.CountyAndUnitaryAuthority.ToString()},
                new SelectListItem
                {
                    Text = "County/UA/District [Place and postcode search not available]",
                    Value = AreaTypeIds.CountyAndUnitaryAuthority + "," + AreaTypeIds.DistrictAndUnitaryAuthority
                },
            };

            foreach (var selectListItem in list)
            {
                if (selectListItem.Value == selectedValue)
                {
                    selectListItem.Selected = true;
                    break;
                }
            }

            return list;
        }
    }
}