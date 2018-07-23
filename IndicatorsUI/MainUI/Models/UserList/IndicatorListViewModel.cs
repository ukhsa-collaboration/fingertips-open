using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndicatorsUI.MainUI.Models.UserList
{
    public class IndicatorListViewModel
    {
        public int Id { get; set; }
        public string ListName { get; set; }
        public string UserId { get; set; }
        public List<IndicatorListItemViewModel> IndicatorListItems { get; set; }
        public string PublicId { get; set; }
    }
}