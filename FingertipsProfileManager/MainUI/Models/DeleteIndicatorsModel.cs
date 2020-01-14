
using System.Collections.Generic;
using Fpm.ProfileData;

namespace Fpm.MainUI.Models
{
    public class DeleteIndicatorsModel : BaseDataModel
    {
        public IList<GroupingPlusName> IndicatorsToDelete { get; set; }
    }
}