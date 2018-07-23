using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IndicatorsUI.MainUI.Models.UserList
{
    public class ViewIndicatorListViewModel
    {
        public int Id { get; set; }
        public string ListName { get; set; }
        public string UserId { get; set; }
        public string IndicatorIds { get; set; }
    }
}