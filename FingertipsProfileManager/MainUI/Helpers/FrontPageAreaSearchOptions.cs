using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Fpm.ProfileData;

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
                new SelectListItem {Text = "CCG", Value = AreaTypeIds.Ccg.ToString()},
                new SelectListItem {Text = "County/UA", Value = AreaTypeIds.CountyAndUnitaryAuthority.ToString()},
                new SelectListItem
                {
                    Text = "County/UA/District",
                    Value = AreaTypeIds.CountyAndUnitaryAuthority + "," + AreaTypeIds.DistrictAndUnitaryAuthority
                }
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