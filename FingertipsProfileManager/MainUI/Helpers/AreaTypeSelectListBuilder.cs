using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.LookUps;

namespace Fpm.MainUI.Helpers
{
    /// <summary>
    ///    Builds a list of items for an area type drop down menu. The appropriate item
    /// is selected.
    /// </summary>
    public class AreaTypeSelectListBuilder
    {
        public const int UndefinedAreaTypeId = -1;

        public int SelectedAreaTypeId { get; set; }
        public List<SelectListItem> SelectListItems { get; set; }

        public AreaTypeSelectListBuilder(IList<AreaType> areaTypes, int selectedAreaTypeId)
        {
            if (AreAreaTypesOk(areaTypes))
            {
                SetAreaTypeSelectListItems(areaTypes);
                SetSelectedOnSelectedAreaType(selectedAreaTypeId);
                SelectedAreaTypeId = selectedAreaTypeId;
            }
            else
            {
                SelectedAreaTypeId = UndefinedAreaTypeId;
            }
        }

        private void SetSelectedOnSelectedAreaType(int selectedAreaTypeId)
        {
            SelectListItem areaType = null;
            if (selectedAreaTypeId > 0)
            {
                string areaTypeIdString = selectedAreaTypeId.ToString();
                areaType = SelectListItems.FirstOrDefault(x => x.Value == areaTypeIdString);
            }

            if (areaType != null)
            {
                areaType.Selected = true;
            }
        }

        private void SetAreaTypeSelectListItems(IList<AreaType> areaTypes)
        {
            SelectListItems = areaTypes
                .Select(x => new SelectListItem { Text = x.ShortName, Value = x.Id.ToString() })
                .ToList();
        }

        private static bool AreAreaTypesOk(IList<AreaType> areaTypes)
        {
            return areaTypes != null && areaTypes.Count > 0;
        }
    }
}