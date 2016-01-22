using System.Collections.Generic;
using Fpm.ProfileData.Entities.Core;

namespace Fpm.MainUI.Models
{
    public class AreaGridModel
    {
        public int SearchAreaTypeId { get; set; }
        public string SearchText { get; set; }
        public IList<Area> AreaGrid { get; set; }
    }
}