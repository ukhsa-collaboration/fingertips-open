using Fpm.ProfileData.Entities.LookUps;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Fpm.MainUI.Helpers
{
    /// <summary>
    /// Builds a list of sexes drop down menu. The appropriate item is selected.
    /// </summary>
    public class SexSelectListBuilder
    {

        public const int UndefinedSexId = -1;

        public int SelectedSexId { get; set; }
        public List<SelectListItem> SelectListItems { get; set; }

        public SexSelectListBuilder(IList<Sex> sexes, int selectedSexId)
        {
            if (AreSexesOk(sexes))
            {
                SetSexSelectListItems(sexes);
                SetSelectedOnSelectedSex(selectedSexId);
                SelectedSexId = selectedSexId;
            }
            else
            {
                SelectedSexId = UndefinedSexId;
            }
        }

        private void SetSelectedOnSelectedSex(int selectedSexId)
        {
            SelectListItem sex = null;
            if (selectedSexId > 0)
            {
                string sexIdString = selectedSexId.ToString();
                sex = SelectListItems.FirstOrDefault(x => x.Value == sexIdString);
            }

            if (sex != null)
            {
                sex.Selected = true;
            }
        }

        private void SetSexSelectListItems(IList<Sex> sexes)
        {
            SelectListItems = sexes
                .Select(x => new SelectListItem { Text = x.Description, Value = x.SexID.ToString() })
                .ToList();
        }

        private static bool AreSexesOk(IList<Sex> sexes)
        {
            return sexes != null && sexes.Count > 0;
        }
    }
}