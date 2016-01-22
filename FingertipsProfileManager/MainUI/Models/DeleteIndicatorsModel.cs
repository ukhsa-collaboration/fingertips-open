using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fpm.ProfileData;

namespace Fpm.MainUI.Models
{
    public class DeleteIndicatorsModel : BaseDataModel
    {
        // Filtering-related properties
        public string ProfileName { get; set; }
        public string DomainName { get; set; }
        public int DomainId { get; set; }
        public int SelectedAreaTypeId { get; set; }

        public IEnumerable<GroupingPlusName> IndicatorsThatCantBeRemoved { get; set; }
        public IList<GroupingPlusName> IndicatorsToDelete { get; set; }

    }
}