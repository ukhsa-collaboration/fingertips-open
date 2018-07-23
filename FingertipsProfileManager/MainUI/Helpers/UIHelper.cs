using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fpm.ProfileData.Entities.LookUps;

namespace Fpm.MainUI.Helpers
{
    public static class UIHelper
    {
        public const int ShowAll = -99;

        public static SelectList SetSelectedItem(this SelectList list, string value)
        {
            if (value != null)
            {
                var selected = list.First(x => x.Value == value);
                selected.Selected = true;
                return list;
            }
            return list;
        }

        public static SelectList SetSelectedItem(this SelectList list, int value)
        {
            return SetSelectedItem(list, value.ToString());
        }

        private static void SetSelectedValue(IEnumerable<SelectListItem> items, string idString)
        {
            var selectedItem = items.FirstOrDefault(x => x.Value == idString);
            if (selectedItem != null)
            {
                selectedItem.Selected = true;
            }
        }

        public static IEnumerable<SelectListItem> GetSexes(this IEnumerable<Sex> sexList, int? sexId)
        {
            var items = sexList.Select(sex => new SelectListItem
            {
                Value = sex.SexID.ToString(),
                Text = sex.Description
            }).ToList();

            if (sexId.HasValue) SetSelectedValue(items, sexId.Value.ToString());


            return items;
        }

        public static IEnumerable<SelectListItem> GetAges(this IEnumerable<Age> ageList, int? ageId)
        {
            var items = ageList.Select(age => new SelectListItem
            {
                Value = age.AgeID.ToString(),
                Text = age.Description
            }).ToList();

            if (ageId.HasValue) SetSelectedValue(items, ageId.Value.ToString());


            return items;
        }

        public static SelectList AddAnyOption(this SelectList list)
        {
            return GetSelectList(list.ToList());
        }

        public static SelectList ConvertToSelectListWithAnyOption(this IEnumerable<int> list)
        {
            var newList = list.Select(x => new SelectListItem()
            {
                Text = x == -1 ? "n/a" : x.ToString(),
                Value = x.ToString()
            }).ToList();
            return GetSelectList(newList);
        }

        private static SelectList GetSelectList(IList<SelectListItem> newList)
        {
            if (newList.Count == 0)
            {
                newList = new List<SelectListItem>();
                newList.Add(new SelectListItem { Text = "n/a", Value = "-1" });
            }
            else if (newList.Count > 1)
            {
                newList.Insert(0, new SelectListItem() { Value = ShowAll.ToString(), Text = "Any" });
            }
            return new SelectList(newList, "Value", "Text");
        }

        public static string Round(this double value, int decimalPlaces = 2)
        {
            return RoundDouble(value, decimalPlaces);
        }

        public static string Round(this double? value, int decimalPlaces = 2)
        {
            if (value.HasValue == false) return string.Empty;

            return RoundDouble(value.Value, decimalPlaces);
        }


        private static string RoundDouble(double value, int decimalPlaces)
        {
            if (value == -1) return value.ToString();

            return value.ToString("0." + new string('0', decimalPlaces));
        }

    }
}