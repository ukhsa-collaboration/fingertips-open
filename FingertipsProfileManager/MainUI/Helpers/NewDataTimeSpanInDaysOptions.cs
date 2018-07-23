using System.Collections.Generic;
using System.Web.Mvc;

namespace Fpm.MainUI.Helpers
{
    public interface INewDataTimeSpanInDaysOptions
    {
        IList<SelectListItem> GetOptions(string selectedValue);
    }

    public class NewDataTimeSpanInDaysOptions : INewDataTimeSpanInDaysOptions
    {
        public const string None = "NONE";

        public IList<SelectListItem> GetOptions(string selectedValue)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Text = None, Value = "0"},
                new SelectListItem { Text = "1 month", Value = "30" },
                new SelectListItem { Text = "2 months", Value = "60" },
                new SelectListItem { Text = "3 months", Value = "90" },
                new SelectListItem { Text = "4 months", Value = "120" },
                new SelectListItem { Text = "6 months", Value = "180" },
                new SelectListItem { Text = "9 months", Value = "270" },
                new SelectListItem { Text = "1 year", Value = "365" }
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
