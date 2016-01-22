using Fpm.ProfileData.Entities.Core;

namespace Fpm.MainUI.Models
{
    public class AreaDetail
    {
        public int SearchAreaTypeId { get; set; }
        public string SearchText { get; set; }
        public Area AreaDetails { get; set; }
    }
}