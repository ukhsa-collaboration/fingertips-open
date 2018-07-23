using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndicatorsUI.MainUI.Models.UserList
{
    public class IndicatorListsViewModel
    {
        public IEnumerable<IndicatorListViewModel> IndicatorLists { get; set; }
    }
}