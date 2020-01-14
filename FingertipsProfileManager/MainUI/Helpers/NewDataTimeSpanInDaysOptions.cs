using System.Collections.Generic;
using System.Web.Mvc;

namespace Fpm.MainUI.Helpers
{
    public class NewDataDeploymentCount : INewDataDeploymentCount
    {
        public const string None = "NONE";

        public IList<SelectListItem> GetOptions(string selectedValue)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Text = None, Value = "0"},
                new SelectListItem { Text = "1", Value = "1" },
                new SelectListItem { Text = "2", Value = "2" },
                new SelectListItem { Text = "3", Value = "3" },
                new SelectListItem { Text = "4", Value = "4" },
                new SelectListItem { Text = "6", Value = "6" },
                new SelectListItem { Text = "9", Value = "9" },
                new SelectListItem { Text = "12", Value = "12" }
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
