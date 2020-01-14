using System.Collections.Generic;
using Fpm.ProfileData;
namespace Fpm.MainUI.Models
{
    public class ApproveIndicatorsModel : BaseDataModel
    {
        public int AreaTypeId { get; set; }
        public IList<GroupingPlusName> IndicatorsToApprove { get; set; }
    }
}