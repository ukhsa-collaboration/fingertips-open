using Fpm.ProfileData;

namespace Fpm.MainUI.Models
{
    public class RemoveIndicatorModel : BaseDataModel
    {
        public GroupingPlusName Indicator { get; set; }
        public string GroupName { get; set; }
        public bool IndicatorCanBeRemoved { get; set; }
        public bool UserHasPermissionToProfile { get; set; }
    }
}