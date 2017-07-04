using Fpm.ProfileData.Entities.Core;

namespace Fpm.MainUI.ViewModels.Areas
{
    public class AreaDetailViewModel
    {
        public string InitialAreaCode { get; set; }
        public int SearchAreaTypeId { get; set; }
        public string SearchText { get; set; }
        public Area AreaDetails { get; set; }
    }
}