using Fpm.ProfileData;
using System.Collections.Generic;

namespace Fpm.MainUI.Models
{
    public class IndicatorsAwaitingRevisionModel : BaseDataModel
    {
        public int FromGroupId { get; set; }
        public int ToGroupId { get; set; }
        public int AreaTypeId { get; set; }
        public IList<GroupingPlusName> IndicatorsAwaitingRevision { get; set; }
    }
}