using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fpm.ProfileData.Entities.LookUps;

namespace Fpm.MainUI.Helpers
{
    /// <summary>
    /// Builds a list of ages drop down menu. The appropriate item is selected.
    /// </summary>
    public class AgeSelectListBuilder
    {

        public const int UndefinedAgeId = -1;

        public int SelectedAgeId { get; set; }
        public List<SelectListItem> SelectListItems { get; set; }

        public AgeSelectListBuilder(IList<Age> ages, int selectedAgeId)
        {
            if (AreAgesOk(ages))
            {
                SetAgeSelectListItems(ages);
                SetSelectedOnSelectedAge(selectedAgeId);
                SelectedAgeId = selectedAgeId;
            }
            else
            {
                SelectedAgeId = UndefinedAgeId;
            }
        }

        private void SetSelectedOnSelectedAge(int selectedAgeId)
        {
            SelectListItem age = null;
            if (selectedAgeId > 0)
            {
                string ageIdString = selectedAgeId.ToString();
                age = SelectListItems.FirstOrDefault(x => x.Value == ageIdString);
            }

            if (age != null)
            {
                age.Selected = true;
            }
        }

        private void SetAgeSelectListItems(IList<Age> ages)
        {
            SelectListItems = ages
                .Select(x => new SelectListItem { Text = x.Description, Value = x.AgeID.ToString() })
                .ToList();
        }

        private static bool AreAgesOk(IList<Age> ages)
        {
            return ages != null && ages.Count > 0;
        }
    }
}