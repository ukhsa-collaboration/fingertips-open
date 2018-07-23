using System.Collections.Generic;
using System.Web.Mvc;
using Fpm.ProfileData;

namespace Fpm.MainUI.Models
{
    public class MoveIndicatorsModel : BaseDataModel
    {
        // Filtering-related properties
        public string ProfileName { get; set; }
        public string DomainName { get; set; }
        public int DomainIndex { get; set; }
        public int AreaTypeId { get; set; }

        public IEnumerable<GroupingPlusName> IndicatorsThatCantBeTransferred { get; set; }
        public IList<GroupingPlusName> IndicatorsToTransfer { get; set; }
        public IEnumerable<SelectListItem> ListOfDomains { get; set; }
    }
}